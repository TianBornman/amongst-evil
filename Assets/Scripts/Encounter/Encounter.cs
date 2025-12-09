using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Encounter/Basic")]
public class Encounter : ScriptableObject
{
	// Public Variables
	public string encounterName;

	public List<Character> encounterCharacters;
	public int minEnemies = 2;
	public int maxEnemies = 5;

	public GameObject sceneryPrefab;          
	public float radius = 5f;

	// Public Methods
	public void Spawn(Vector3 center)
	{
		if (sceneryPrefab != null)
			Instantiate(sceneryPrefab, center, Quaternion.identity);

		int count = Random.Range(minEnemies, maxEnemies + 1);

		for (int i = 0; i < count; i++)
		{
			Vector2 r = Random.insideUnitCircle * radius;
			Vector3 pos = center + new Vector3(r.x, 0f, r.y);

			var enemy = encounterCharacters[Random.Range(0, encounterCharacters.Count)];
			var spawnedEnemy = Instantiate(enemy, pos, Quaternion.identity);

			SpawnManager.Instance.spawnedCharacters.Add(spawnedEnemy);
			// Later: assign patrol routes, aggro delays, tasks, etc.
		}
	}
}
