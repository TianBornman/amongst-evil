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
		character.agent.isStopped = true;
	}

	public void Update()
	{
		float normalized = Mathf.Clamp01(character.agent.velocity.magnitude / character.agent.speed);
		character.animator.SetFloat("Speed", normalized);
		character.animator.SetFloat("Direction", normalized);

		if (Vector3.Distance(character.transform.position, character.idlePos.position) < 0.1)
			return;
		else
			character.agent.SetDestination(character.idlePos.position);
	}
}