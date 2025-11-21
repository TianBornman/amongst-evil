using Midevil.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecruitManager : MonoBehaviour
{
	// Editor Variables
	[Header("References")]
	public List<RecruitCharacter> recruitPrefabs;

	// Private Methods
	private void Start()
	{
		var recruitmentCount = Random.Range(2, 5);

		for (int i = 0; i < recruitmentCount; i++)
		{
			var recruitData = new RecruitData();
			recruitData.Randomize();

			var recruitPrefab = recruitPrefabs[Random.Range(0, recruitPrefabs.Count)];
			var spawnPosition = transform.position + (Vector3)(Random.insideUnitCircle * 5f);
			var recruitInstance = Instantiate(recruitPrefab, spawnPosition, Quaternion.identity);

			recruitInstance.recruitData = recruitData;
		}
	}
}