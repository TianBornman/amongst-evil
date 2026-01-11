using UnityEngine;

public class CombatMoveState : IState
{
	public bool CanExit { get; private set; } = true;

	private Character character;

	private Vector3 velocity = Vector3.zero;

	public CombatMoveState(Character character)
	{
		this.character = character;
	}

	public void Enter()
	{
	}

	public void Exit()
	{
	}

	public void Update()
	{
		character.transform.position = Vector3.SmoothDamp(character.transform.position,
			character.combatPositionIntent.position,
			ref velocity,
			0.15f);
	}
}