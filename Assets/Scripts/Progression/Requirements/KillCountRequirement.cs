using Midevil.Models;
using UnityEngine;

namespace Midevil.Progression
{
	[CreateAssetMenu(fileName = "Kill Count Requirement", menuName = "Progression/Requirements/Kill Count")]
	public class KillCountRequirement : RankRequirement
	{
		[Tooltip("Enemy id (matches Character.enemyId on the enemy prefab). Use \"*\" for any enemy.")]
		public string enemyId;
		public int count;

		public override bool IsMet(SectProgressData data) => data.GetKills(enemyId) >= count;
		public override float Progress01(SectProgressData data) =>
			count <= 0 ? 1f : Mathf.Clamp01(data.GetKills(enemyId) / (float)count);
		public override string ProgressText(SectProgressData data) => $"{data.GetKills(enemyId)} / {count}";
	}
}
