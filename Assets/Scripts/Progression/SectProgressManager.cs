using Midevil.Models;
using System;
using UnityEngine;

namespace Midevil.Progression
{
	public class SectProgressManager : Singleton<SectProgressManager>
	{
		[Header("References")]
		public SpiralProgression spiral;
		public StandingRewardTable rewardTable;

		[Tooltip("Lowest value Standing can be clamped to. 0 = never goes negative; negative = wipes can drag you below 0.")]
		public int standingFloor = 0;

		public event Action<SectRankData> OnAscensionAvailable;
		public event Action<SectRankData> OnAscended;
		public event Action<int> OnStandingChanged;

		public SectProgressData Data => BloodvaultManager.Load().sectProgress;

		public SectRankData CurrentRank => spiral != null ? spiral.GetRank(Data.currentRank) : null;
		public SectRankData NextRank => spiral != null ? spiral.NextRankAfter(Data.currentRank) : null;

		public bool AscensionPending => Data.ascensionPending;

		protected override void Awake()
		{
			base.Awake();
			BloodvaultManager.Load();
		}

		public void RecordKill(string enemyId)
		{
			if (string.IsNullOrEmpty(enemyId)) return;
			Data.AddKill(enemyId);
			BloodvaultManager.Save();
			EvaluateAscension();
		}

		public void RecordMissionCompleted(Mission.Mission mission, bool success)
		{
			if (mission == null) return;

			Data.missionsCompleted.Add(new MissionCompletedEntry
			{
				type = mission.type,
				difficulty = mission.difficulty,
				success = success
			});

			int delta = rewardTable != null ? rewardTable.CalculateMissionReward(mission, success) : 0;
			AddStandingInternal(delta);

			BloodvaultManager.Save();
			EvaluateAscension();
		}

		public void AddStanding(int amount)
		{
			AddStandingInternal(amount);
			BloodvaultManager.Save();
			EvaluateAscension();
		}

		public void IncrementMilestone(string id, int amount = 1)
		{
			if (string.IsNullOrEmpty(id)) return;
			Data.AddMilestone(id, amount);
			BloodvaultManager.Save();
			EvaluateAscension();
		}

		public bool CanAscend()
		{
			var next = NextRank;
			if (next == null) return false;
			foreach (var req in next.requirements)
				if (req == null || !req.IsMet(Data)) return false;
			return true;
		}

		public bool PerformAscension()
		{
			if (!CanAscend()) return false;

			Data.currentRank++;
			Data.ascensionPending = false;
			BloodvaultManager.Save();

			OnAscended?.Invoke(CurrentRank);
			EvaluateAscension();
			return true;
		}

		public int MissionDifficultyCap()
		{
			return Mathf.Max(1, Data.currentRank);
		}

		private void AddStandingInternal(int amount)
		{
			if (amount == 0) return;
			Data.standing = Mathf.Max(standingFloor, Data.standing + amount);
			OnStandingChanged?.Invoke(Data.standing);
		}

		private void EvaluateAscension()
		{
			if (Data.ascensionPending) return;
			if (!CanAscend()) return;

			Data.ascensionPending = true;
			BloodvaultManager.Save();
			OnAscensionAvailable?.Invoke(NextRank);
		}
	}
}
