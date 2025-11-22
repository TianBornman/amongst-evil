using Midevil.Ability;
using Midevil.Effect;
using Midevil.Item;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

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
	public List<Buff> buffs = new();
	public List<Effect> effects = new();
	public List<Ability> abilities = new();
	public List<Item> drops = new();

	[Header("References")]
	public Transform weaponPos;
	public SkinnedMeshRenderer helmetSkin;
	public SkinnedMeshRenderer armourSkin;
	public SkinnedMeshRenderer glovesSkin;
	public SkinnedMeshRenderer leggingsSKin;
	public SkinnedMeshRenderer bootsSkin;

	// Protected Variables
	protected Character target;
	protected Character killer;

	protected NavMeshAgent agent;
	protected Animator animator;
	protected Outline outline;

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
			case CharacterState.Blocking:
				Block();
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
		animator.SetBool("Blocking", false);
		animator.SetBool("Attacking", true);
	}

	private void Block()
	{
		animator.SetBool("Blocking", true);
	}

	private void Die()
	{
		agent.isStopped = true;

		animator.SetTrigger("Die");
		SpawnManager.Instance.RemoveCharacter(this);

		DropItems();

		if (target is Player player)
		{
			player.AddXp(stats.xpValue);
			ResultManager.Instance.results.kills++;
		}

		foreach (var buff in killer.effects)
			if (buff is IOnKill onKill)
				onKill.OnKill(this, killer);

		foreach (var buff in effects)
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

		if (target is Player player)
		{
			ResultManager.Instance.results.damageDealt += damage;

			if (isCrit)
				ResultManager.Instance.results.criticalHits++;
			else
				ResultManager.Instance.results.hits++;
		}
		else
		{
			if (damage > 0)
				ResultManager.Instance.results.damageTaken += damage;
			else
				ResultManager.Instance.results.healed += damage;
		}

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
		animAPI.CheckValidTarget = () => CheckValidTarget();
		animAPI.Attack = Attack;
		animAPI.Disappear = () => Destroy(gameObject);
	}

	protected virtual void Start()
	{
		if (PlayerManager.Instance != null && PlayerManager.Instance.player != null)
			stats.level = PlayerManager.Instance.player.stats.level;
		else
			stats.level = 1;

		RecalculateStats();

		SetState(CharacterState.Idle);
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

		if (item.ability != null)
		{
			var ability = item.ability.CreateRuntime(this);
			ability.id = item.id;

			AddAbility(ability);
		}

		switch (item.type)
		{
			case ItemType.Helmet:
				break;
			case ItemType.Armour:
				var skinnedRenderer = item.visual.GetComponentInChildren<SkinnedMeshRenderer>();
				armourSkin.sharedMesh = skinnedRenderer.sharedMesh;
				armourSkin.materials = skinnedRenderer.sharedMaterials;

				armourSkin.gameObject.SetActive(true);
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
				armourSkin.gameObject.SetActive(false);
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

	// Private Methods
	private void Attack()
	{
		if (!CheckValidTarget() && State == CharacterState.Attacking) return;

		foreach (var buff in effects)
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