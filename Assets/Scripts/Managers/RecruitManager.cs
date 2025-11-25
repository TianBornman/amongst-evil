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

	[HideInInspector] public Identity playerIdentity;

	// Override Methods
	override protected void Awake()
	{
		base.Awake();

		playerIdentity = null;
	}

	protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (!GameManager.Instance.AtHub)
			return;

		var recruitmentCount = Random.Range(2, 5);

		// Spawn existing characters from Bloodvault
		var existingCharaters = BloodvaultManager.Load().entries.Where(entry => entry.status == BloodVaultStatus.Alive).ToList();
		foreach (var character in existingCharaters)
		{
			SpawnCharacter(recruitPrefabs[Random.Range(0, recruitPrefabs.Count)], character.identity);
			recruitmentCount--;
		}

		// Spawn new random characters if there is still space
		for (int i = 0; i < recruitmentCount; i++)
		{
			var recruitData = new Identity();
			recruitData.Randomize();

			SpawnCharacter(recruitPrefabs[Random.Range(0, recruitPrefabs.Count)], recruitData);
		}
	}

	// Public Methods
	public void RecruitPlayer(Identity identity)
	{
		playerIdentity = identity;
		identity.currentResult = new();
		ClearGear(identity);
	}

	// Private Methods
	private void SpawnCharacter(RecruitCharacter prefab, Identity identity)
	{
		var spawnPosition = transform.position + (Vector3)(Random.insideUnitCircle * 5f);
		var recruitInstance = Instantiate(prefab, spawnPosition, Quaternion.identity);

		recruitInstance.identity = identity;
	}

	private void ClearGear(Identity identity)
	{
		identity.weapon = null;
		identity.armour = null;
	}
}