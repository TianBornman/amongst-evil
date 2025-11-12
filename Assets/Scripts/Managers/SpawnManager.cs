using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
	// Editor Variables
	[Header("References")]
	public List<Character> characters;
	public List<Character> bossCharacters;
	public List<MapSegment> segments;

	[Header("Settings")]
	public float width;
	public float length;
	public float minDistance;
	public bool bossSpawned = false;

	// Public Variables
	[HideInInspector] public List<Character> spawnedCharacters = new();

	// Private Variables
	private MapSegment currentSegment;

	// Override Methods
	protected override void Awake()
	{
		base.Awake();

		currentSegment = FindFirstObjectByType<MapSegment>();
	}

	// Public Methods
	public void RemoveCharacter(Character character)
	{
		spawnedCharacters.Remove(character);

		if (spawnedCharacters.Count <= 0)
		{
			if (bossSpawned)
			{
				UiManager.Instance.ShowResults();
				SpawnMapSegment();
			}
			else
				SpawnBoss();
		}
	}

	// Private Methods
	private void Start()
	{
		SpawnWave();
	}

	public void SpawnWave() // Uses Rejection Sampling to avoid clustering
	{
		bossSpawned = false;

		List<Vector3> positions = new List<Vector3>();
		int randomCount = Random.Range(8, 12);

		for (int i = 0; i < randomCount; i++)
		{
			Vector3 newPos;
			int attempts = 0;
			const int maxAttempts = 100;

			do
			{
				float x = Random.Range(-width / 2f, width / 2f);
				float z = Random.Range(-length / 2f, length / 2f);
				newPos = new Vector3(x, 0f, z) + currentSegment.transform.position;

				attempts++;
				if (attempts > maxAttempts)
					break;

			} while (TooClose(newPos, positions));

			positions.Add(newPos);

			var characterPrefab = characters[Random.Range(0, characters.Count)];
			spawnedCharacters.Add(Instantiate(characterPrefab, newPos, Quaternion.identity));
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
		var bossPrefab = bossCharacters[Random.Range(0, bossCharacters.Count)];
		spawnedCharacters.Add(Instantiate(bossPrefab, currentSegment.transform.position, Quaternion.identity));

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
