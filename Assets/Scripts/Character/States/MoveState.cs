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
			character.target = FindNearestPartyMember();

			if (character.target == null)
				return;
		}

		character.agent.SetDestination(character.target.transform.position);
	}

	private Character FindNearestPartyMember()
	{
		var members = PartyManager.Instance.PartyMembers;

		if (members == null)
			return null;

		return members
			.Where(m => m != null && m.IsAlive)
			.OrderBy(m => Vector3.Distance(m.transform.position, character.transform.position))
			.FirstOrDefault();
	}

	private void UpdateHoldPosition()
	{
		character.agent.SetDestination(character.idlePos.position);
	}
}
