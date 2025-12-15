using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : Singleton<SpawnManager>
{
	// Editor Variables
	[Header("References")]
	public List<Encounter> encounters;
	public List<Encounter> bossEncounters;

	[Header("Settings")]
	public float width;
	public float length;
	public float minDistance;
	public int minEncounters = 2;
	public int maxEncounters = 4;
	public bool bossSpawned = false;

	// Public Variables
	[HideInInspector] public List<Character> spawnedCharacters = new();

	// Public Methods
	public void RemoveCharacter(Character character)
	{
		spawnedCharacters.Remove(character);

		if (spawnedCharacters.Count <= 0)
		{
			if (bossSpawned)
			{
				PartyManager.Instance.CalculateStats();
				UiManager.Instance.ShowResults();
			}
			else
				SpawnBoss();
		}
	}

	// Private Methods
	protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (GameManager.Instance.AtHub)
			return;

		spawnedCharacters.Clear();
		SpawnWave();
	}

	public void SpawnWave() // Uses Rejection Sampling to avoid clustering
	{
		bossSpawned = false;
		spawnedCharacters.Clear();

		int encounterCount = Random.Range(minEncounters, minEncounters + 1);
		List<Vector3> encounterCenters = new();

		for (int i = 0; i < encounterCount; i++)
		{
			Vector3 center;
			int attempts = 0;

			do
			{
				float x = Random.Range(-width / 2f, width / 2f);
				float z = Random.Range(-length / 2f, length / 2f);
				center = new Vector3(x, 0, z);

				attempts++;
			}
			while (attempts < 100 && TooClose(center, encounterCenters));

			encounterCenters.Add(center);

			var pack = encounters[Random.Range(0, encounters.Count)];
			pack.Spawn(center);
		}
	}

	private bool TooClose(Vector3 pos, List<Vector3> existing)
	{
		foreach (var other in existing)
		{
			if (Vector3.Distance(pos, other) < minDistance)
				return true;
		}
		return false;
	}

	private void SpawnBoss()
	{
		var bossEncounter = bossEncounters[Random.Range(0, bossEncounters.Count)];
		bossEncounter.Spawn(Vector3.zero);

		bossSpawned = true;
	}
}
