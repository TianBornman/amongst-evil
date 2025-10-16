using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
	// Editor Variables
	[Header("References")]
	public List<Character> spawnCharacters;

	[Header("Settings")]
	public float width;
	public float length;
	public float minDistance;

	// Private Methods
	private void Start()
	{
		SpawnCharacters();
	}

	private void SpawnCharacters() // Uses Rejection Sampling to avoid clustering
	{
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
				newPos = new Vector3(x, 0f, z);

				attempts++;
				if (attempts > maxAttempts)
					break;

			} while (TooClose(newPos, positions));

			positions.Add(newPos);

			var characterPrefab = spawnCharacters[Random.Range(0, spawnCharacters.Count)];
			Instantiate(characterPrefab, newPos, Quaternion.identity);
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
}
