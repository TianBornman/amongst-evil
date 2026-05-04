using Midevil.Models;
using UnityEngine;

namespace Midevil.Progression
{
	[CreateAssetMenu(fileName = "Standing Requirement", menuName = "Progression/Requirements/Standing")]
	public class StandingRequirement : RankRequirement
	{
		public int amount;

		public override bool IsMet(SectProgressData data) => data.standing >= amount;
		public override float Progress01(SectProgressData data) =>
			amount <= 0 ? 1f : Mathf.Clamp01(data.standing / (float)amount);
		public override string ProgressText(SectProgressData data) => $"{data.standing} / {amount}";
	}
}
