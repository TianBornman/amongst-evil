using Midevil.Mission;
using Midevil.Models;
using UnityEngine;

namespace Midevil.Progression
{
	[CreateAssetMenu(fileName = "Mission Completed Requirement", menuName = "Progression/Requirements/Mission Completed")]
	public class MissionCompletedRequirement : RankRequirement
	{
		public bool filterByType;
		public MissionType type;

		public bool filterByMinDifficulty;
		public MissionDifficulty minDifficulty = MissionDifficulty.I;

		public int count = 1;

		public override bool IsMet(SectProgressData data) => CurrentCount(data) >= count;
		public override float Progress01(SectProgressData data) =>
			count <= 0 ? 1f : Mathf.Clamp01(CurrentCount(data) / (float)count);
		public override string ProgressText(SectProgressData data) => $"{CurrentCount(data)} / {count}";

		private int CurrentCount(SectProgressData data) => data.CountMissions(
			filterByType ? type : (MissionType?)null,
			filterByMinDifficulty ? minDifficulty : (MissionDifficulty?)null);
	}
}
