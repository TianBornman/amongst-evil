using Midevil.Models;
using UnityEngine;

namespace Midevil.Progression
{
	[CreateAssetMenu(fileName = "Milestone Requirement", menuName = "Progression/Requirements/Milestone")]
	public class MilestoneRequirement : RankRequirement
	{
		[Tooltip("Generic milestone id incremented from gameplay code via SectProgressManager.IncrementMilestone(id).")]
		public string milestoneId;
		public int count = 1;

		public override bool IsMet(SectProgressData data) => data.GetMilestone(milestoneId) >= count;
		public override float Progress01(SectProgressData data) =>
			count <= 0 ? 1f : Mathf.Clamp01(data.GetMilestone(milestoneId) / (float)count);
		public override string ProgressText(SectProgressData data) => $"{data.GetMilestone(milestoneId)} / {count}";
	}
}
