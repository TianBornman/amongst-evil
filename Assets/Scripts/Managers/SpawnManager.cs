using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : Singleton<SpawnManager>
{
	// Editor Variables
	[Header("References")]
	public List<EncounterData> encounters;
	public List<EncounterData> bossEncounters;
	public SpawnPoint partySpawnPoint;
	public List<SpawnPoint> encounterSpawnPoints;
	public List<SpawnPoint> bossSpawnPoints;

	[Header("Settings")]
	public float width;
	public float length;
	public float minDistance;
	public int minEncounters = 2;
	public int maxEncounters = 4;
	public bool bossSpawned = false;

	// Public Variables
	[HideInInspector] public List<Character> spawnedCharacters = new();

	// Private Variables
	private int totalEnemies;

	// Public Methods
	public void RemoveCharacter(Character character)
	{
		spawnedCharacters.Remove(character);

		UiManager.Instance.SetEnemiesText(spawnedCharacters.Count, totalEnemies);

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

		var spawnPoints = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);

		partySpawnPoint = spawnPoints.FirstOrDefault(spawn => spawn.partySpawnPoint);
		encounterSpawnPoints = spawnPoints.Where(spawn => !spawn.partySpawnPoint && !spawn.bossSpawnPoint).ToList();
		bossSpawnPoints = spawnPoints.Where(spawn => spawn.bossSpawnPoint).ToList();

		spawnedCharacters.Clear();
		SpawnWave();
	}

	public void SpawnWave() // Uses Rejection Sampling to avoid clustering
	{
		bossSpawned = false;
		spawnedCharacters.Clear();

		int encounterCount = Random.Range(minEncounters, minEncounters + 1);

		for (int i = 0; i < encounterCount; i++)
		{
			var spawnPoint = encounterSpawnPoints[Random.Range(0, encounterSpawnPoints.Count)];
			encounterSpawnPoints.Remove(spawnPoint);

			var encounter = encounters[Random.Range(0, encounters.Count)];
			encounter.Spawn(spawnPoint.GetSpawnPoint());
		}

		totalEnemies = spawnedCharacters.Count;
	}

	private void SpawnBoss()
	{
        var spawnPoint = bossSpawnPoints[Random.Range(0, bossSpawnPoints.Count)];

        var bossEncounter = bossEncounters[Random.Range(0, bossEncounters.Count)];
		bossEncounter.Spawn(spawnPoint.GetSpawnPoint());

		UiManager.Instance.SetBossText("Roaming");
		bossSpawned = true;
	}
}
