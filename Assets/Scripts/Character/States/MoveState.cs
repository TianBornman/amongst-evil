using System.Linq;
using UnityEngine;

public class MoveState : IState
{
	public bool CanExit { get; private set; } = true;

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
		if (character.chaseTarget)
			character.agent.isStopped = true;
	}

	public void Update()
	{
		float normalized = character.agent.velocity.magnitude / character.agent.speed;
		character.animator.SetFloat("Speed", normalized);

		if (character.chaseTarget)
			UpdateChasing();
		else
			UpdateHoldPosition();
	}

	private void UpdateChasing()
	{
		if (character.target == null || !character.target.IsAlive)
		{
			character.agent.SetDestination(character.idlePos.position);
			character.ReevaluateTarget();
			return;
		}

		character.agent.SetDestination(character.target.transform.position);

		if (character.stats.range >= Vector3.Distance(character.transform.position, character.target.transform.position))
			character.SetState(new AttackState(character));
	}

	private void UpdateHoldPosition()
	{
		character.agent.SetDestination(character.idlePos.position);

		var attackTarget = character.targets
			.Where(t => t != null && t.IsAlive &&
				character.stats.range >= Vector3.Distance(t.transform.position, character.transform.position))
			.OrderBy(t => Vector3.Distance(t.transform.position, character.transform.position))
			.FirstOrDefault();

		if (attackTarget != null)
		{
			character.target = attackTarget;
			character.SetState(new AttackState(character));
		}
	}
}
