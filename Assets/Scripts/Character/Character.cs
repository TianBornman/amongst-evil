using Midevil.Item;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Character : StateMachine<CharacterState>
{
	// Editor variables
	[HideInInspector] public Stats stats;
	public Stats baseStats;
	public List<Buff> buffs = new();
	public List<Item> drops = new();

	[Header("References")]
	public Transform weaponPos;

	// Protected Variables
	protected Character target;
	protected Character killer;

	protected NavMeshAgent agent;
	protected Animator animator;


	// Public Properties
	public bool IsAlive => State != CharacterState.Dead;

	// Override Methods
	protected override void SetState(CharacterState state)
	{
		if (State == CharacterState.Dead)
			return;

		base.SetState(state);

		switch (State)
		{
			case CharacterState.Idle:
				Idle();
				break;
			case CharacterState.Moving:
				Moving();
				break;
			case CharacterState.Attacking:
				Attacking();
				break;
			case CharacterState.Dead:
				Die();
				break;
			default:
				break;
		}
	}

	// State Methods
	private void Idle()
	{
		target = null;

		agent.isStopped = true;

		animator.SetFloat("Speed", 0);
		animator.SetBool("Attacking", false);
	}

	private void Moving()
	{
		agent.isStopped = false;

		animator.SetFloat("Speed", 1);
		animator.SetBool("Attacking", false);
	}

	private void Attacking()
	{
		agent.isStopped = true;

		transform.LookAt(target.transform);
		animator.SetBool("Attacking", true);
	}

	private void Die()
	{
		agent.isStopped = true;

		animator.SetTrigger("Die");
		SpawnManager.Instance.RemoveCharacter(this);

		DropItems();

		if (target is Player player)
			player.AddXp(stats.xpValue);

		foreach (var buff in killer.buffs)
			if (buff is IOnKill onKill)
				onKill.OnKill(this, killer);

		foreach (var buff in buffs)
			if (buff is IOnDeath onDeath)
				onDeath.OnDeath(this, killer);
	}

	// Public Methods
	public void SetTarget(Character character)
	{
		target = character;
		UiManager.Instance.BindEnemyStats(this);
		SetState(CharacterState.Attacking);
	}

	public void Damage(float damage, float critChance = 0, float critDamage = 0)
	{
		var isCrit = Random.value < critChance;

		if (isCrit)
			damage *= critDamage;

		DamageNumberManager.Instance.ShowDamage(transform.position + Vector3.up * 1.5f, Mathf.Abs(damage), damage > 0 ? Color.red : Color.green);

		stats.health = Mathf.Clamp(stats.health - damage, 0, stats.maxHealth);

		foreach (var buff in buffs)
			if (buff is IOnTakeHit onTakeHit)
				onTakeHit.OnTakeHit(this, target, damage);

		if (stats.health == 0)
		{
			killer = target;
			SetState(CharacterState.Dead);
		}
	}

	public void AddBuff(Buff buff)
	{
		var sameBuff = buffs.FirstOrDefault(b => b.id == buff.id);
		if (sameBuff != null)
		{
			if (sameBuff.RefreshOrStack(buff))
				return;
		}

		buff.OnApply(this);

		buffs.Add(buff);
		RecalculateStats();
	}

	public void RemoveBuff(Buff buff)
	{
		buff.OnRemove(this);

		buffs.Remove(buff);
		RecalculateStats();
	}

	public virtual void EquipItem(ItemStats item)
	{
		AddBuff(item.buff);

		foreach (var effect in item.effects)
			AddBuff(effect.CreateRuntime());

		switch (item.type)
		{
			case ItemType.Head:
				break;
			case ItemType.Weapon:
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
		RemoveBuff(item.buff);

		switch (item.type)
		{
			case ItemType.Head:
				break;
			case ItemType.Weapon:
				RemoveChildren(weaponPos);
				UpdateAnimations(ItemAnimationType.Unarmed);
				break;
			case ItemType.Offhand:
				break;
			default:
				break;
		}
	}

	// Protected Methods
	protected virtual void Start()
	{
		if (PlayerManager.Instance.player != null)
			stats.level = PlayerManager.Instance.player.stats.level;
		else
			stats.level = 1;

		RecalculateStats();

		SetState(CharacterState.Idle);
	}

	protected virtual void Update()
	{
		for (int i = buffs.Count - 1; i >= 0; i--)
		{
			var buff = buffs[i];
			buff.TickTimer(Time.deltaTime);

			if (buff is IOnTick tick)
				tick.Tick(this, Time.deltaTime);

			if (buff.IsExpired)
				RemoveBuff(buff);
		}
	}

	// Private Methods
	private void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponentInChildren<Animator>();

		CharacterAnimAPI animAPI = GetComponentInChildren<CharacterAnimAPI>();
		animAPI.CheckValidTarget = () => CheckValidTarget();
		animAPI.Attack = Attack;
		animAPI.Disappear = () => Destroy(gameObject);
	}

	private void Attack()
	{
		if (!CheckValidTarget() && IsAlive) return;

		foreach (var buff in buffs)
			if (buff is IOnHit onHit)
				onHit.OnHit(this, target, stats.damage);

		target.Damage(stats.damage, stats.critChance, stats.critDamage);
	}

	private void RecalculateStats()
	{
		stats.Recalculate(baseStats, buffs);

		animator.SetFloat("AttackSpeed", stats.attackSpeed);
	}

	private bool CheckValidTarget()
	{
		if (target == null || !target.IsAlive)
		{
			SetState(CharacterState.Idle);
			return false;
		}

		return true;
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