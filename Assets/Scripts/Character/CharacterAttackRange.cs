using UnityEngine;

public class CharacterAttackRange : MonoBehaviour
{
	// Private Variables
	private Character character;

	// Private Methods
	private void Awake()
	{
		character = GetComponentInParent<Character>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.TryGetComponent<Character>(out var targetCharacter))
		{
			if (character.targetTeams.Contains(targetCharacter.team))
			{
				if (!character.IsAlive)
					return;

				character.AddTarget(targetCharacter);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.TryGetComponent<Character>(out var targetCharacter))
		{
			if (character.targetTeams.Contains(targetCharacter.team))
			{
				character.RemoveTarget(targetCharacter);
			}
		}
	}
}
