using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Encounter/Basic")]
public class EncounterData : ScriptableObject
{
	// Public Variables
	public string encounterName;

	public List<Character> encounterCharacters;
	public int minEnemies = 2;
	public int maxEnemies = 5;

	public GameObject sceneryPrefab;
	public bool startIdle;
	public bool faceScenery;
	public float radius = 5f;
	public float detectionRadius = 10f;

	// Public Methods
	public void Spawn(Vector3 center)
	{
		var encounter = Instantiate(RefManager.Instance.baseEncounter, center, Quaternion.identity);

		Transform scenery = null;

		if (sceneryPrefab != null)
			scenery = Instantiate(sceneryPrefab, encounter.transform).transform;

		int count = Random.Range(minEnemies, maxEnemies + 1);

		for (int i = 0; i < count; i++)
		{
			var r = Random.insideUnitCircle * radius;
			var pos = center + new Vector3(r.x, 0f, r.y);
			var rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

			var enemy = encounterCharacters[Random.Range(0, encounterCharacters.Count)];
			var spawnedEnemy = Instantiate(enemy, pos, rotation);
			spawnedEnemy.transform.parent = encounter.transform;

			SpawnManager.Instance.spawnedCharacters.Add(spawnedEnemy);

			// Set Encounter
			encounter.encounterName = encounterName;
			encounter.encounterCharacters.Add(spawnedEnemy);
			encounter.detectionRadius = detectionRadius;

			spawnedEnemy.startIdle = startIdle;

			if (scenery && faceScenery)
				spawnedEnemy.transform.LookAt(scenery);
		}
	}
}
