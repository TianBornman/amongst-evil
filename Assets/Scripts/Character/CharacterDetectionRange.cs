using UnityEngine;

public class CharacterDetectionRange : MonoBehaviour
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
		if (other.gameObject.TryGetComponent<Character>(out var enemy))
		{
			if (character.targetTeams.Contains(enemy.team))
			{
				character.AddTarget(enemy);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.TryGetComponent<Character>(out var enemy))
		{
			if (character.targetTeams.Contains(enemy.team))
			{
				character.RemoveTarget(enemy);
			}
		}
	}
}
