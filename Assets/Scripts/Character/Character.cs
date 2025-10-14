using UnityEngine;
using UnityEngine.AI;

public class Character : StateMachine<CharacterState>
{
	// Editor variables
	public Stats stats;

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
	}

	// Public Methods
	public void SetTarget(Character character)
	{
		target = character;
		SetState(CharacterState.Attacking);
	}

	public void Damage(float damage)
	{
		stats.health -= damage;

		if (stats.health <= 0)
		{
			stats.health = 0;
			SetState(CharacterState.Dead);
		}
	}

	// Private Methods
	private void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponentInChildren<Animator>();

		CharacterAnimAPI animAPI = GetComponentInChildren<CharacterAnimAPI>();
		animAPI.Attack = Attack;
	}

	private void Start()
	{
		stats.health = stats.maxHealth;

		SetState(CharacterState.Idle);
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