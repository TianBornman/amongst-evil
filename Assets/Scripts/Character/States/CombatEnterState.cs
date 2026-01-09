using UnityEngine;

public class CombatEnterState : IState
{
	public bool CanExit { get; private set; } = true;

	private Character character;

	public CombatEnterState(Character character)
	{
		this.character = character;
	}

	public void Enter()
	{
		character.agent.enabled = false;
		character.animator.SetTrigger("Move");
		character.animator.SetFloat("Speed", 0);

		character.transform.position = character.lane.transform.position;
		character.transform.rotation = Quaternion.Euler(0, 90, 0);
	}

	public void Exit()
	{
	}

	public void Update()
	{
	}
}