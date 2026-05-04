using Midevil.Ability;
using Midevil.Effect;
using Midevil.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterAbilities))]
[RequireComponent(typeof(CharacterEffects))]
[RequireComponent(typeof(CharacterEquipment))]
public class Character : StateMachine, IInteractable
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
	public List<Effect> currentEffects = new();
	public List<Ability> currentAbilities = new();
	public Identity identity;
	[Tooltip("Stable id used by Sect progression to count kills (e.g. \"zombie\", \"slime\", \"barbarian-boss\"). Empty = not tracked.")]
	public string enemyId;
	public bool startIdle;
	public bool chaseTarget = true;

	public Character target;
	public List<Character> targets = new();

	[Header("On-Spawn Effects")]
	[Tooltip("Effect application groups rolled when this character spawns. Each group can be exclusive (pickOnlyOne — e.g. one of Cursed/Blighted/Corrupted/Forsaken) or independent (each rolled on its own chance).")]
	public List<EffectApplicationGroup> spawnEffects = new();

	[Header("References")]
	public Transform idlePos;
	public Transform targetPos;

	// Public Variables
	public CharacterEquipment equipment;
	public CharacterAbilities abilities;
	public CharacterEffects effects;
	public HealthbarVisual healthBar;
	public NavMeshAgent agent;
	public Animator animator;
	public int animationIndex;

	// Protected Variables
	protected Character killer;
	protected Outline outline;

	// Public Properties
	public bool IsAlive => stats.health > 0;
	public bool InBattle => target != null;

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

	public virtual void AddXp(float amount, bool shareXp = false)
	{
		stats.currentXp += amount;

		identity.currentResult.xpGained += amount;

		while (stats.currentXp >= stats.neededXp)
		{
			stats.currentXp -= stats.neededXp;
			LevelUp();
		}
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

		if (healthBar)
			healthBar.SetHealth(stats.health, stats.maxHealth);

		foreach (var buff in currentEffects)
			if (buff is IOnTakeHit onTakeHit)
				onTakeHit.OnTakeHit(this, target, damage);

		// Stat Tracking
		attacker.identity.currentResult.damageDealt += damage;

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

		if (healthBar)
			healthBar.SetHealth(stats.health, stats.maxHealth);

		identity.currentResult.healed += amount;
	}

	public void Attack()
	{
		var executor = AttackExecutorResolver.Resolve(identity.weapon);

		executor.ExecuteAttack(new AttackContext
		{
			attacker = this,
			target = target,
		});
	}

	public virtual void ReevaluateTarget()
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

		var size = new Vector3(stats.size, stats.size, stats.size);
		transform.localScale = size;

		if (healthBar)
			healthBar.SetHealth(stats.health, stats.maxHealth);

		animator.SetFloat("AttackSpeed", stats.attackSpeed);
	}


	public virtual void Die()
	{
		SpawnManager.Instance.RemoveCharacter(this);

		foreach (var buff in killer.currentEffects)
			if (buff is IOnKill onKill)
				onKill.OnKill(this, killer);

		foreach (var buff in currentEffects)
			if (buff is IOnDeath onDeath)
				onDeath.OnDeath(this, killer);

		killer.AddXp(stats.xpValue);
		killer.identity.currentResult.kills++;

		if (!string.IsNullOrEmpty(enemyId) && team != killer.team)
		{
			var sect = Midevil.Progression.SectProgressManager.Instance;
			if (sect != null) sect.RecordKill(enemyId);
		}

		var pooled = GetComponent<PooledEnemy>();
		if (pooled != null) pooled.ScheduleReturn(2f);
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
		healthBar = GetComponent<HealthbarVisual>();
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

		if (startIdle)
			SetState(new IdleState(this));
		else
			SetState(new MoveState(this));

		SetupIdentity();

		if (idlePos == null)
			idlePos = transform;

		RecalculateStats();

		ApplySpawnEffects();
	}

	private void ApplySpawnEffects()
	{
		if (spawnEffects == null) return;
		for (int i = 0; i < spawnEffects.Count; i++)
			if (spawnEffects[i] != null)
				spawnEffects[i].Apply(this);
	}

	public virtual void OnPooledRespawn()
	{
		if (startIdle)
			ForceState(new IdleState(this));
		else
			ForceState(new MoveState(this));

		RecalculateStats();
		ApplySpawnEffects();
	}

	// Private Methods
	private void SetupIdentity()
	{
		if (identity.level > 0)
			stats.level = identity.level;

		stats.currentXp = identity.xp;
		stats.neededXp = GetNeededXp(stats.level);

		equipment.SetupIdentity();
	}

	private void LevelUp()
	{
		stats.level++;
		stats.neededXp = GetNeededXp(stats.level);
		Heal(stats.maxHealth * stats.levelHeal);
	}

	private float GetNeededXp(int level)
	{
		return 10f * Mathf.Pow(level, 1.3f) + 5f * level;
	}
}