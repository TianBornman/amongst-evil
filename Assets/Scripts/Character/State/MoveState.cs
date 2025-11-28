using UnityEngine;

public class MoveState : ICharacterState
{
	private Character character;

	public MoveState(Character character)
	{
		this.character = character;
	}

	public void Enter()
	{
		character.agent.isStopped = false;
		character.animator.SetTrigger("Move");
	}

	public void Exit()
	{
		character.agent.isStopped = true;
	}

	public void Update()
	{
		if (character.agent.velocity.magnitude > 0)
			character.animator.SetFloat("Speed", 1);
		else
			character.animator.SetFloat("Speed", 0);

		if (character.target == null && Vector3.Distance(character.transform.position, character.idlePos.position) < 0.2)
		{
			return;
		}

		if (character.target == null || !character.target.IsAlive)
		{
			character.agent.SetDestination(character.idlePos.position);
			character.ReevaluateTarget();
			return;
		}
		else
			character.agent.SetDestination(character.target.transform.position);

		if (character.stats.range >= Vector3.Distance(character.transform.position, character.target.transform.position))
			character.SetState(new AttackState(character));
	}
}