using Midevil.Mission;
using UnityEngine;

namespace Midevil.Progression
{
	[CreateAssetMenu(fileName = "Standing Reward Table", menuName = "Progression/Standing Reward Table")]
	public class StandingRewardTable : ScriptableObject
	{
		[Header("Mission Outcomes")]
		[Tooltip("Base standing for any successful mission.")]
		public int missionCompleteBase = 25;

		[Tooltip("Extra standing per Threat Rating (added on top of the base on success). e.g. Threat III = base + 3 * perThreat.")]
		public int perThreatBonus = 10;

		[Tooltip("Multiplier applied to mission reward for Relic Recovery.")]
		public float relicRecoveryMult = 1.5f;

		[Tooltip("Multiplier applied to mission reward for Chaos.")]
		public float chaosSurvivalMult = 0.8f;

		[Tooltip("Standing penalty for a wipe (all party dead).")]
		public int wipePenalty = 15;

		public int CalculateMissionReward(Mission.Mission mission, bool success)
		{
			if (mission == null) return 0;

			if (!success) return -wipePenalty;

			int reward = missionCompleteBase + perThreatBonus * (int)mission.difficulty;
			float mult = mission.type switch
			{
				MissionType.RelicRecovery => relicRecoveryMult,
				MissionType.Chaos => chaosSurvivalMult,
				_ => 1f
			};
			return Mathf.RoundToInt(reward * mult);
		}
	}
}
