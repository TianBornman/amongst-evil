using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Encounter/Basic")]
public class Encounter : ScriptableObject
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

	// Public Methods
	private Vector3 FindNavMeshPosition(Vector3 center)
	{
		for (int attempt = 0; attempt < 10; attempt++)
		{
			var r = Random.insideUnitCircle * radius;
			var candidate = center + new Vector3(r.x, 0f, r.y);

			if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 50f, NavMesh.AllAreas))
				return hit.position;
		}
		return Vector3.zero;
	}

	public void Spawn(Vector3 center)
	{
		Transform scenery = null;

		if (sceneryPrefab != null)
			scenery = Instantiate(sceneryPrefab, center, Quaternion.identity).transform;

		int count = Random.Range(minEnemies, maxEnemies + 1);

		for (int i = 0; i < count; i++)
		{
			Vector3 pos = FindNavMeshPosition(center);

			if (pos == Vector3.zero)
			{
				Debug.LogWarning($"[Encounter] Could not find NavMesh position near {center} for enemy {i}");
				continue;
			}

			var rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
			var enemy = encounterCharacters[Random.Range(0, encounterCharacters.Count)];
			var spawnedEnemy = Instantiate(enemy, pos, rotation);

			SpawnManager.Instance.spawnedCharacters.Add(spawnedEnemy);

			spawnedEnemy.startIdle = startIdle;

			if (scenery && faceScenery)
				spawnedEnemy.transform.LookAt(scenery);
		}
	}
}
