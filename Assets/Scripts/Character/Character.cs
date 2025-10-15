using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : StateMachine<CharacterState>
{
	// Editor variables
	[HideInInspector] public Stats stats;
	public Stats baseStats;
	public List<Buff> buffs = new();
	public float XpValue;

	// Protected Variables
	protected Character target;

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

		if (target is Player player)
			player.AddXp(XpValue);
	}

	// Public Methods
	public void SetTarget(Character character)
	{
		target = character;
		UiManager.Instance.BindEnemyStats(stats);
		SetState(CharacterState.Attacking);
	}

	public void Damage(float damage)
	{
		stats.health = Mathf.Clamp(stats.health - damage, 0, stats.maxHealth);

		if (stats.health == 0)
			SetState(CharacterState.Dead);
	}

	public void AddBuff(Buff buff)
	{
		buffs.Add(buff);
		stats.Recalculate(baseStats, buffs);
	}

	// Protected Methods
	protected virtual void Start()
	{
		stats.Recalculate(baseStats, buffs);

		SetState(CharacterState.Idle);
	}

	// Private Methods
	private void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponentInChildren<Animator>();

		CharacterAnimAPI animAPI = GetComponentInChildren<CharacterAnimAPI>();
		animAPI.Attack = Attack;
	}

	private void Attack()
	{
		if (target == null || !target.IsAlive)
		{
			SetState(CharacterState.Idle);
			return;
		}

		target.Damage(stats.damage);
	}
}