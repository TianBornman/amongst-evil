using Midevil.Mission;
using System;
using System.Collections.Generic;

namespace Midevil.Models
{
	[Serializable]
	public class SectProgressData
	{
		public int currentRank = 1;
		public int standing = 0;
		public bool ascensionPending = false;

		public List<KillCountEntry> killCounts = new();
		public List<MissionCompletedEntry> missionsCompleted = new();
		public List<MilestoneEntry> milestones = new();

		public int GetKills(string enemyId)
		{
			if (string.IsNullOrEmpty(enemyId)) return 0;

			if (enemyId == "*")
			{
				int total = 0;
				foreach (var k in killCounts) total += k.count;
				return total;
			}

			var entry = killCounts.Find(k => k.enemyId == enemyId);
			return entry != null ? entry.count : 0;
		}

		public void AddKill(string enemyId, int amount = 1)
		{
			if (string.IsNullOrEmpty(enemyId)) return;
			var entry = killCounts.Find(k => k.enemyId == enemyId);
			if (entry == null)
			{
				entry = new KillCountEntry { enemyId = enemyId, count = 0 };
				killCounts.Add(entry);
			}
			entry.count += amount;
		}

		public int GetMilestone(string id)
		{
			if (string.IsNullOrEmpty(id)) return 0;
			var entry = milestones.Find(m => m.id == id);
			return entry != null ? entry.count : 0;
		}

		public void AddMilestone(string id, int amount = 1)
		{
			if (string.IsNullOrEmpty(id)) return;
			var entry = milestones.Find(m => m.id == id);
			if (entry == null)
			{
				entry = new MilestoneEntry { id = id, count = 0 };
				milestones.Add(entry);
			}
			entry.count += amount;
		}

		public int CountMissions(MissionType? type, MissionDifficulty? minDifficulty, bool successOnly = true)
		{
			int n = 0;
			foreach (var m in missionsCompleted)
			{
				if (successOnly && !m.success) continue;
				if (type.HasValue && m.type != type.Value) continue;
				if (minDifficulty.HasValue && (int)m.difficulty < (int)minDifficulty.Value) continue;
				n++;
			}
			return n;
		}
	}

	[Serializable]
	public class KillCountEntry
	{
		public string enemyId;
		public int count;
	}

	[Serializable]
	public class MissionCompletedEntry
	{
		public MissionType type;
		public MissionDifficulty difficulty;
		public bool success;
	}

	[Serializable]
	public class MilestoneEntry
	{
		public string id;
		public int count;
	}
}
