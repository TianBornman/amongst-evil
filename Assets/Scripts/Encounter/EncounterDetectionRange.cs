using System.Collections;
using UnityEngine;

public class EncounterDetectionRange : MonoBehaviour
{
	private static WaitForSeconds waitForSeconds = new WaitForSeconds(0.5f);

	// Private Variables
	private Encounter encounter;

	// Private Methods
	private void Awake()
	{
		encounter = GetComponentInParent<Encounter>();
		StartCoroutine(CheckTargets());
	}

	private IEnumerator CheckTargets()
	{
		while (!CombatManager.Instance.inBattle)
		{
			var targets = Physics.OverlapSphere(transform.position, encounter.detectionRadius, RefManager.Instance.targetableMask);

			foreach (var target in targets)
			{
				if (target.gameObject.TryGetComponent<PartyCharacter>(out var character) && !CombatManager.Instance.inBattle)
				{
					CombatManager.Instance.StartBattle(encounter);
				}
			}

			yield return waitForSeconds;
		}
	}
}
