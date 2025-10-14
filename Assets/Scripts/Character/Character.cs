using UnityEngine;

public class Character : StateMachine<CharacterState>
{
	// Editor variables
	public Stats stats;

	// Protected Variables
	protected Character target;

	// Private Variables
	private Animator animator;

	// Override Methods
	protected override void SetState(CharacterState state)
	{
		base.SetState(state);

		switch (State)
		{
			case CharacterState.Idle:
				animator.SetFloat("Speed", 0);
				animator.SetBool("Attacking", false);
				break;
			case CharacterState.Moving:
				animator.SetFloat("Speed", 1);
				animator.SetBool("Attacking", false);
				break;
			case CharacterState.Attacking:
				animator.SetBool("Attacking", true);
				break;
			case CharacterState.Dead:
				animator.SetTrigger("Die");
				break;
			default:
				break;
		}
	}

	// Private Methods
	private void Awake()
	{
		animator = GetComponentInChildren<Animator>();
	}

	private void Start()
	{
		SetState(CharacterState.Idle);
	}
}