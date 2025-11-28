using UnityEngine;

public class AbilityState : ICharacterState
{
	private Character character;
	private float timeRemaining;

	public AbilityState(Character character)
	{
		this.character = character;
		timeRemaining = 1 / character.stats.castSpeed;
	}

	public void Enter()
	{
		character.animator.SetTrigger("Ability");
	}

	public void Exit()
	{
		character.abilities.currentAbility = null;
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