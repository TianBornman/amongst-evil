using Midevil.Ability;
using Midevil.Effect;
using Midevil.Helpers;
using Midevil.Item;
using Midevil.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Guid = System.Guid;

public class Character : StateMachine<CharacterState>, IInteractable
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

	// Editor variables
	[HideInInspector] public Stats stats;
	public Stats baseStats;
	public Team team;
	public List<Team> targetTeams;
	public List<Buff> buffs = new();
	public List<Effect> effects = new();
	public List<Ability> abilities = new();
	public List<Item> drops = new();
	public Identity identity;

	[Header("References")]
	public Transform weaponPos;
	public SkinnedMeshRenderer helmetSkin;
	public SkinnedMeshRenderer armourSkin;
	public SkinnedMeshRenderer glovesSkin;
	public SkinnedMeshRenderer leggingsSKin;
	public SkinnedMeshRenderer bootsSkin;
	public Transform idlePos;

	// Protected Variables
	protected Character target;
	protected Character killer;

	protected NavMeshAgent agent;
	protected Animator animator;
	protected Outline outline;

	// Private Variables
	private bool isAttacking;
	private bool isCasting;
	private Ability currentAbility;

	// Public Properties
	public bool IsAlive => State != CharacterState.Dead;

	// Private Properties
	public bool IsAttacking
	{
		get => isAttacking;
		set
		{
			isAttacking = value;
			animator.SetBool("Attack", isAttacking);
		}
	}

	// Override Methods
	protected override void SetState(CharacterState state)
	{
		if (State == CharacterState.Dead)
			return;

		base.SetState(state);

		switch (State)
		{
			case CharacterState.Moving: Moving(); break;
			case CharacterState.Attacking: Attacking(); break;
			case CharacterState.Dead: Die(); break;
		}
	}

	// State Methods
	private void Moving()
	{
		agent.isStopped = false;
		IsAttacking = false;
	}

	private void Attacking()
	{
		agent.isStopped = true;
		IsAttacking = true;
	}

	private void Die()
	{
		agent.isStopped = true;

		animator.SetTrigger("Die");
		SpawnManager.Instance.RemoveCharacter(this);

		DropItems();

		// TODO: Make sure only players get XP, but together with party XP sharing
		//if (target is PartyCharacter player)
		//{
		//	player.AddXp(stats.xpValue);
		//	PlayerManager.Instance.player.Results.kills++;
		//}

		foreach (var buff in killer.effects)
			if (buff is IOnKill onKill)
				onKill.OnKill(this, killer);

		foreach (var buff in effects)
			if (buff is IOnDeath onDeath)
				onDeath.OnDeath(this, killer);

		PartyManager.Instance.playerParty.RemoveEnemyInRange(this);
	}

	// Public Methods
	public void SetTarget(Character character)
	{
		target = character;
	}

	public void Damage(float damage, float critChance = 0, float critDamage = 0)
	{
		var isBlock = Random.value < stats.blockChance;

		if (isBlock)
			return;

		var isDodge = Random.value < stats.dodgeChance;

		if (isDodge)
			return;

		var isCrit = Random.value < critChance;

		if (isCrit)
			damage *= critDamage;

		DamageNumberManager.Instance.ShowDamage(transform.position + Vector3.up * 1.5f, Mathf.Abs(damage), damage > 0 ? Color.red : Color.green);

		stats.health = Mathf.Clamp(stats.health - damage, 0, stats.maxHealth);

		foreach (var buff in effects)
			if (buff is IOnTakeHit onTakeHit)
				onTakeHit.OnTakeHit(this, target, damage);

		// TODO: Reimplement stats tracking
		//if (target is PartyCharacter player)
		//{
		//	PlayerManager.Instance.player.Results.damageDealt += damage;

		//	if (isCrit)
		//		PlayerManager.Instance.player.Results.criticalHits++;
		//	else
		//		PlayerManager.Instance.player.Results.hits++;
		//}
		//else
		//{
		//	if (damage > 0)
		//		PlayerManager.Instance.player.Results.damageTaken += damage;
		//	else
		//		PlayerManager.Instance.player.Results.healed += damage;
		//}

		if (stats.health == 0)
		{
			killer = target;
			SetState(CharacterState.Dead);
		}
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
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponentInChildren<Animator>();
		outline = GetComponent<Outline>();
		outline.enabled = false;

		CharacterAnimAPI animAPI = GetComponentInChildren<CharacterAnimAPI>();
		animAPI.Attack = Attack;
		animAPI.Ability = CastAbility;
		animAPI.AbilityFinished = CastAbilityFinished;
		animAPI.Disappear = () => gameObject.SetActive(false);
	}

	protected virtual void Start()
	{
		// TODO: Reimplement level setting
		//if (PlayerManager.Instance != null && PlayerManager.Instance.player != null)
		//	stats.level = PlayerManager.Instance.player.stats.level;
		//else
		//	stats.level = 1;

		SetupIdentity();

		if (idlePos == null)
			idlePos = transform;

		RecalculateStats();

		SetState(CharacterState.Moving);
	}

	protected virtual void Update()
	{
		for (int i = effects.Count - 1; i >= 0; i--)
		{
			var effect = effects[i];
			effect.TickTimer(Time.deltaTime);

			if (effect is IOnTick tick)
				tick.Tick(this, Time.deltaTime);

			if (effect.IsExpired)
				RemoveEffect(effect);
		}

		foreach (var ability in abilities)
			ability?.Update(Time.deltaTime);

		if (agent.velocity.magnitude > 0)
			animator.SetFloat("Speed", 1);
		else
			animator.SetFloat("Speed", 0);

		if (State == CharacterState.Moving)
			Move();
	}

	public void TryUseAbility(Guid id)
	{
		if (currentAbility != null)
			return;

		var ability = abilities.FirstOrDefault(ab => ab.id == id);

		if (ability == null || !ability.IsReady)
			return;

		currentAbility = ability;
		animator.SetTrigger("Ability");
		isCasting = true;
	}

	public virtual void AddAbility(Ability ability)
	{
		abilities.Add(ability);
	}

	public virtual void RemoveAbility(Ability ability)
	{
		abilities.Remove(ability);
	}

	public virtual void AddEffect(Effect effect)
	{
		var sameEffect = effects.FirstOrDefault(b => b.effectType == effect.effectType);
		if (sameEffect != null)
		{
			if (sameEffect.RefreshOrStack(effect))
				return;
		}

		effect.OnApply(this);
		effects.Add(effect);
		RecalculateStats();
	}

	public virtual void RemoveEffect(Effect effect)
	{
		effect.OnRemove(this);

		effects.Remove(effect);
		RecalculateStats();
	}

	public virtual void EquipItem(ItemStats item)
	{
		item.buff.id = item.id;
		AddBuff(item.buff);

		foreach (var effectData in item.effects)
		{
			var effect = effectData.CreateRuntime();
			effect.id = item.id;
			AddEffect(effect);
		}

		if (item.abilityIndex != AbilityReferenceIndex.None)
		{
			var abilityData = RefManager.Instance.GetAbility(item.abilityIndex);

			var ability = abilityData.CreateRuntime(this);
			ability.id = item.id;

			AddAbility(ability);
		}

		switch (item.type)
		{
			case ItemType.Helmet:
				break;
			case ItemType.Armour:
				if (identity.armour != null)
					UnequipItem(identity.armour);

				identity.armour = item;
				identity.armourConfig.Set(item);

				var skinnedRenderer = item.visual.GetComponentInChildren<SkinnedMeshRenderer>();
				armourSkin.sharedMesh = skinnedRenderer.sharedMesh;
				armourSkin.materials = skinnedRenderer.sharedMaterials;

				armourSkin.gameObject.SetActive(true);
				break;
			case ItemType.Weapon:
				if (identity.weapon != null)
					UnequipItem(identity.weapon);

				identity.weapon = item;
				identity.weaponConfig.Set(item);

				Instantiate(item.visual, weaponPos);
				UpdateAnimations(item.animationType);
				break;
			case ItemType.Offhand:
				break;
			default:
				break;
		}
	}

	public virtual void UnequipItem(ItemStats item)
	{
		var buff = buffs.Where(buff => buff.id == item.id).FirstOrDefault();
		RemoveBuff(buff);

		List<Effect> effectsToRemove = new();

		foreach (var effect in effects.Where(ef => ef.id == item.id))
			effectsToRemove.Add(effect);

		foreach (var effect in effectsToRemove)
			RemoveEffect(effect);

		var ability = abilities.FirstOrDefault(ability => ability.id == item.id);

		if (ability != null)
			RemoveAbility(ability);

		switch (item.type)
		{
			case ItemType.Helmet:
				break;
			case ItemType.Armour:
				identity.armour = null;
				identity.armourConfig.Clear();
				armourSkin.gameObject.SetActive(false);
				break;
			case ItemType.Weapon:
				identity.weapon = null;
				identity.weaponConfig.Clear();
				RemoveChildren(weaponPos);
				UpdateAnimations(ItemAnimationType.Unarmed);
				break;
			case ItemType.Offhand:
				break;
			default:
				break;
		}
	}

	// Private Methods
	private void SetupIdentity()
	{
		if (identity.level > 0)
			stats.level = identity.level;

		if (identity.armourConfig.index > 0)
		{
			EquipItemConfig(identity.armourConfig);
		}

		if (identity.weaponConfig.index > 0)
		{
			EquipItemConfig(identity.weaponConfig);
		}
	}

	private void EquipItemConfig(ItemConfig config)
	{
		var item = RefManager.Instance.GetItem(config.index);
		var itemStats = item.stats.Clone();
		itemStats.effectIndex = config.effectIndex;
		itemStats.shouldRoll = false;

		ItemHelper.Setup(itemStats);

		EquipItem(itemStats);
	}

	private void Move()
	{
		if (isCasting)
		{
			agent.isStopped = true;
			return;
		}
		else 
			agent.isStopped = false;

		if (target == null && Vector3.Distance(transform.position, idlePos.position) < 0.2)
			return;

		if (target == null || !target.IsAlive)
		{
			agent.SetDestination(idlePos.position);
			SetState(CharacterState.Moving);
			return;
		}
		else
			agent.SetDestination(target.transform.position);

		if (stats.range >= Vector3.Distance(transform.position, target.transform.position))
			SetState(CharacterState.Attacking);
	}

	private void Attack()
	{
		if (!target.IsAlive) 
			SetState(CharacterState.Moving);

		foreach (var buff in effects)
			if (buff is IOnHit onHit)
				onHit.OnHit(this, target, stats.damage);

		transform.LookAt(target.transform);
		target.Damage(stats.damage, stats.critChance, stats.critDamage);
	}

	private void CastAbility()
	{
		if (currentAbility != null)

		currentAbility.TryUse();
	}

	private void CastAbilityFinished()
	{
		currentAbility = null;
		isCasting = false;
	}

	private void RecalculateStats()
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

				if (itemRb != null)
				{
					Vector3 forceDirection = (Vector3.up + Random.insideUnitSphere).normalized;
					itemRb.AddForce(forceDirection * 5f, ForceMode.Impulse);
				}
			}
		}
	}

	private void RemoveChildren(Transform transform)
	{
		foreach (Transform child in transform)
			Destroy(child.gameObject);
	}

	private void UpdateAnimations(ItemAnimationType type)
	{
		switch (type)
		{
			case ItemAnimationType.Unarmed:
				animator.runtimeAnimatorController = RefManager.Instance.unarmed;
				break;
			case ItemAnimationType.Sword1H:
				animator.runtimeAnimatorController = RefManager.Instance.sword1H;
				break;
			case ItemAnimationType.Sword2H:
				break;
			case ItemAnimationType.Shield:
				break;
			default:
				break;
		}
	}
}