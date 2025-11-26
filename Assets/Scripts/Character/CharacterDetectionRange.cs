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
				if (character is PartyCharacter)
				{
					PartyManager.Instance.playerParty.AddEnemyInRange(enemy);
					enemy.SetTarget(character);
				}
				else
				{
					PartyManager.Instance.playerParty.AddEnemyInRange(character);
					character.SetTarget(enemy);
				}
			}
		}
	}
}
