using Midevil.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RecruitManager : Singleton<RecruitManager>
{
	// Editor Variables
	[Header("References")]
	public List<RecruitCharacter> recruitPrefabs;

	// Override Methods
	protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (!GameManager.Instance.AtHub)
			return;

		var recruitmentCount = Random.Range(2, 5);
		var allBuildings = FindObjectsByType<Building>(FindObjectsSortMode.None);
		var spawnPoints = new List<IdlePosition>();

		foreach (var building in allBuildings)
			spawnPoints.AddRange(building.idlePositions);

		// Spawn existing characters from Bloodvault
		var existingCharaters = BloodvaultManager.Load().entries.Where(entry => entry.status == BloodVaultStatus.Alive).ToList();
		foreach (var character in existingCharaters)
		{
			var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
			spawnPoints.Remove(spawnPoint);

			SpawnCharacter(spawnPoint, recruitPrefabs[Random.Range(0, recruitPrefabs.Count)], character.identity);
			recruitmentCount--;
		}

		// Spawn new random characters if there is still space
		for (int i = 0; i < recruitmentCount; i++)
		{
			var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
			spawnPoints.Remove(spawnPoint);

			var recruitData = new Identity();
			recruitData.Randomize();

			SpawnCharacter(spawnPoint, recruitPrefabs[Random.Range(0, recruitPrefabs.Count)], recruitData);
		}
	}

	// Private Methods
	private void SpawnCharacter(IdlePosition idlePosition, RecruitCharacter prefab, Identity identity)
	{
		var recruitInstance = Instantiate(prefab, idlePosition.position);

		recruitInstance.identity = identity;
		recruitInstance.startIdle = true;
		recruitInstance.animationIndex = idlePosition.animationIndex;
	}
}