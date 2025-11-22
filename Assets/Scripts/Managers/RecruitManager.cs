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
	protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (!GameManager.Instance.AtHub)
			return;

		var recruitmentCount = Random.Range(2, 5);

		for (int i = 0; i < recruitmentCount; i++)
		{
			var recruitData = new Identity();
			recruitData.Randomize();

			var recruitPrefab = recruitPrefabs[Random.Range(0, recruitPrefabs.Count)];
			var spawnPosition = transform.position + (Vector3)(Random.insideUnitCircle * 5f);
			var recruitInstance = Instantiate(recruitPrefab, spawnPosition, Quaternion.identity);

			recruitInstance.identity = recruitData;
		}
	}
}