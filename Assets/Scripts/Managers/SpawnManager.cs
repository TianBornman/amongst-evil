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
	public List<MapSegment> segments;

	[Header("Settings")]
	public float width;
	public float length;
	public float minDistance;
	public int minEncounters = 2;
	public int maxEncounters = 4;
	public bool bossSpawned = false;

	// Public Variables
	[HideInInspector] public List<Character> spawnedCharacters = new();
	[HideInInspector] public MapSegment currentSegment;

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
				SpawnMapSegment();
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

		currentSegment = FindFirstObjectByType<MapSegment>();

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
				center = new Vector3(x, 0, z) + currentSegment.transform.position;

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
		bossEncounter.Spawn(currentSegment.transform.position);

		bossSpawned = true;
	}

	private void SpawnMapSegment()
	{
		MapSegment mapSegment = segments[Random.Range(0, segments.Count)];
		float randomYRotation = 90f * Random.Range(1, 4);

		Vector3 spawnPos = currentSegment.transform.forward * 110;
		MapSegment newSegment = Instantiate(mapSegment, spawnPos, Quaternion.Euler(0, randomYRotation, 0));

		CreateNavLink(currentSegment.connectionPoint.position, 
			currentSegment.connectionPoint.position + currentSegment.connectionPoint.forward * 5f);
		currentSegment = newSegment;
	}

	private void CreateNavLink(Vector3 start, Vector3 end)
	{
		GameObject linkObj = new("NavLink");
		var link = linkObj.AddComponent<NavMeshLink>();

		link.startPoint = linkObj.transform.InverseTransformPoint(start);
		link.endPoint = linkObj.transform.InverseTransformPoint(end);
		link.width = 2.0f;
		link.bidirectional = true;

		//linkObj.transform.position = (start + end) / 2f;

		link.UpdateLink();
	}
}
