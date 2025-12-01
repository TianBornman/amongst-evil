using UnityEngine;

public class AttackState : IState
{
	private Character character;
	private float timeRemaining;

	public AttackState(Character character)
	{
		this.character = character;
		timeRemaining = 1 / character.stats.attackSpeed;
	}

	public void Enter()
	{
		character.animator.SetTrigger("Attack");
	}

	public void Exit()
	{
	}

	public void Update()
	{
		timeRemaining -= Time.deltaTime;

		if (timeRemaining < 0)
			LeaveState();
	}

	private void LeaveState()
	{
		if (character.target != null && character.target.IsAlive)
			character.SetState(new AttackState(character));
		else
			character.SetState(new MoveState(character));
	}
}