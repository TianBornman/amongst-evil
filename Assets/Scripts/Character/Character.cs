using Midevil.Ability;
using Midevil.Effect;
using Midevil.Item;
using Midevil.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterAbilities))]
[RequireComponent(typeof(CharacterEffects))]
[RequireComponent(typeof(CharacterEquipment))]
public class Character : MonoBehaviour, IInteractable
{
	#region Interactable

	public virtual void Interact() { }

	public void OnHoverEnter()
	{
		outline.enabled = true;
	}

	public void OnHoverExit()
	{
		outline.enabled = false;
	}

	#endregion

	#region State

	private ICharacterState state;

	public void SetState(ICharacterState state)
	{
		this.state?.Exit();
		this.state = state;
		state.Enter();
	}

	#endregion

	// Editor variables
	[HideInInspector] public Stats stats;
	public Stats baseStats;
	public Team team;
	public List<Team> targetTeams;
	public List<Buff> buffs = new();
	public List<Effect> currentEffects = new();
	public List<Ability> currentAbilities = new();
	public List<Item> drops = new();
	public Identity identity;

	public Character target;
	public List<Character> targets = new();

	[Header("References")]
	public Transform idlePos;

	// Public Variables
	public CharacterEquipment equipment;
	public CharacterAbilities abilities;
	public CharacterEffects effects;
	public NavMeshAgent agent;
	public Animator animator;

	// Protected Variables
	protected Character killer;
	protected Outline outline;

	// Public Properties
	public bool IsAlive => stats.health > 0;

	// Public Methods
	public void AddTarget(Character character)
	{
		if (targets.Contains(character)) return;

		targets.Add(character);
		ReevaluateTarget();
	}

	public void RemoveTarget(Character character)
	{
		targets.Remove(character);
		ReevaluateTarget();
	}

	public void Damage(Character attacker, float damage, float critChance = 0, float critDamage = 0)
	{
		if (!IsAlive)
			return;

		var isBlock = Random.value < stats.blockChance;

		if (isBlock)
			return;

		var isDodge = Random.value < stats.dodgeChance;

		if (isDodge)
			return;

		var isCrit = Random.value < critChance;

		if (isCrit)
			damage *= critDamage;

		damage = Mathf.Clamp(damage, 0, float.MaxValue);

		DamageNumberManager.Instance.ShowDamage(transform.position + Vector3.up * 1.5f, Mathf.Abs(damage), Color.red);

		stats.health = Mathf.Clamp(stats.health - damage, 0, stats.maxHealth);

		foreach (var buff in currentEffects)
			if (buff is IOnTakeHit onTakeHit)
				onTakeHit.OnTakeHit(this, target, damage);

		attacker.identity.currentResult.damageDealt += damage;

		// Stat Tracking
		if (isCrit)
			attacker.identity.currentResult.criticalHits++;
		else
			attacker.identity.currentResult.hits++;

		identity.currentResult.damageTaken += damage;

		if (stats.health == 0)
		{
			killer = attacker;
			SetState(new DeadState(this));
		}
	}

	public void Heal(float amount)
	{
		amount = Mathf.Clamp(amount, 0, float.MaxValue);

		DamageNumberManager.Instance.ShowDamage(transform.position + Vector3.up * 1.5f, Mathf.Abs(amount), Color.green);

		stats.health = Mathf.Clamp(stats.health + amount, 0, stats.maxHealth);

		identity.currentResult.healed += amount;
	}

	public virtual void Die()
	{
		SpawnManager.Instance.RemoveCharacter(this);

		DropItems();

		// TODO: Make sure only players get XP, but together with party XP sharing
		//if (target is PartyCharacter player)
		//{
		//	player.AddXp(stats.xpValue);
		//	PlayerManager.Instance.player.Results.kills++;
		//}

		killer.identity.currentResult.kills++;

		foreach (var buff in killer.currentEffects)
			if (buff is IOnKill onKill)
				onKill.OnKill(this, killer);

		foreach (var buff in currentEffects)
			if (buff is IOnDeath onDeath)
				onDeath.OnDeath(this, killer);
	}

	public void AddBuff(Buff buff)
	{
		buffs.Add(buff);
		RecalculateStats();
	}

	public void RemoveBuff(Buff buff)
	{
		buffs.Remove(buff);
		RecalculateStats();
	}

	// Protected Methods
	protected virtual void Awake()
	{
		equipment = GetComponent<CharacterEquipment>();
		abilities = GetComponent<CharacterAbilities>();
		effects = GetComponent<CharacterEffects>();
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponentInChildren<Animator>();
		outline = GetComponent<Outline>();
		outline.enabled = false;

		CharacterAnimAPI animAPI = GetComponentInChildren<CharacterAnimAPI>();
		animAPI.Attack = Attack;
		animAPI.Disappear = () => gameObject.SetActive(false);
	}

	protected virtual void Start()
	{
		// TODO: Reimplement level setting
		//if (PlayerManager.Instance != null && PlayerManager.Instance.player != null)
		//	stats.level = PlayerManager.Instance.player.stats.level;
		//else
		//	stats.level = 1;

		SetState(new MoveState(this));

		SetupIdentity();

		if (idlePos == null)
			idlePos = transform;

		RecalculateStats();
	}

	protected virtual void Update()
	{
		state.Update();
	}

	// Private Methods
	private void SetupIdentity()
	{
		if (identity.level > 0)
			stats.level = identity.level;

		equipment.SetupIdentity();
	}

	public void Attack()
	{
		if (target == null || !target.IsAlive)
			SetState(new MoveState(this));

		foreach (var buff in currentEffects)
			if (buff is IOnHit onHit)
				onHit.OnHit(this, target, stats.damage);

		transform.LookAt(target.transform);
		target.Damage(this, stats.damage, stats.critChance, stats.critDamage);
	}

	public void ReevaluateTarget()
	{
		if (targets.Count <= 0 || !targets.Where(target => target.IsAlive).Any())
		{
			target = null;
			targets.Clear();
			return;
		}

		target = targets.Where(target => target.IsAlive).OrderBy(target => Vector3.Distance(target.transform.position, transform.position)).FirstOrDefault();
	}

	public void RecalculateStats()
	{
		stats.Recalculate(baseStats, buffs);

		animator.SetFloat("AttackSpeed", stats.attackSpeed);
	}

	private void DropItems()
	{
		foreach (var item in drops)
		{
			var dropped = Random.value < item.stats.dropChance;

			if (dropped)
			{
				var itemRb = Instantiate(item, transform.position + transform.up * 1.5f, Quaternion.identity).GetComponent<Rigidbody>();

				Vector3 forceDirection = (Vector3.up + Random.insideUnitSphere).normalized;
				itemRb.AddForce(forceDirection * 5f, ForceMode.Impulse);
			}
		}
	}
}