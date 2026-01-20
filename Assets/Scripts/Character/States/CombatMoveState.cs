using Unity.VisualScripting;
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
		character.animator.SetTrigger("Move");
	}

	public void Exit()
	{
	}

	public void Update()
	{
		character.transform.position = Vector3.SmoothDamp(character.transform.position,
			character.combatPositionIntent.position, ref velocity, 0.15f);

		var normalizedSpeed = Mathf.Clamp01(velocity.magnitude / character.stats.moveSpeed);
		character.animator.SetFloat("Speed", normalizedSpeed);
		character.animator.SetFloat("Direction", GetDirection(normalizedSpeed));

		var targetRotation = character.facingRight ? Quaternion.Euler(0f, 90f, 0f) : Quaternion.Euler(0f, -90f, 0f);
		character.transform.rotation = Quaternion.RotateTowards(character.transform.rotation,
			targetRotation, 720 * Time.deltaTime);
	}

	// Private Methods
	private float GetDirection(float value)
	{
		if (character.facingRight)
		{
			if (character.combatPositionIntent.position.x < character.transform.position.x)
				return -value;
			else
				return value;
		}
		else
		{
			if (character.combatPositionIntent.position.x > character.transform.position.x)
				return -value;
			else
				return value;
		}
	} 
}