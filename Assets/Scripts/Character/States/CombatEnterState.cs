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
		character.combatPositionIntent = Object.Instantiate(RefManager.Instance.emptyObject, character.transform.position, Quaternion.identity).transform;
		character.SetState(new CombatMoveState(character));
	}

	public void Exit()
	{
	}

	public void Update()
	{
	}
}