using Midevil.Models;
using System.Collections.Generic;
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

		for (int i = 0; i < recruitmentCount; i++)
		{
			var recruitData = new Identity();

			if (playerIdentity != null)
			{
				recruitData = playerIdentity;
				playerIdentity = null;
			}
			else
				recruitData.Randomize();

			var recruitPrefab = recruitPrefabs[Random.Range(0, recruitPrefabs.Count)];
			var spawnPosition = transform.position + (Vector3)(Random.insideUnitCircle * 5f);
			var recruitInstance = Instantiate(recruitPrefab, spawnPosition, Quaternion.identity);

			recruitInstance.identity = recruitData;
		}
	}

	// Public Methods
	public void RecruitPlayer(Identity identity)
	{
		playerIdentity = identity;
		identity.currentResult = new();
	}
}