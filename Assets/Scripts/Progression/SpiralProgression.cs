using System.Collections.Generic;
using UnityEngine;

namespace Midevil.Progression
{
	[CreateAssetMenu(fileName = "Spiral Progression", menuName = "Progression/Spiral Progression")]
	public class SpiralProgression : ScriptableObject
	{
		[Tooltip("Ordered list of ranks from Whispering Ashes (1) to Clockless (10).")]
		public List<SectRankData> ranks = new();

		public SectRankData GetRank(int rankNumber)
		{
			if (rankNumber <= 0) return null;
			foreach (var r in ranks)
				if (r != null && r.rankNumber == rankNumber)
					return r;
			return null;
		}

		public SectRankData NextRankAfter(int rankNumber) => GetRank(rankNumber + 1);

		public int MaxRank
		{
			get
			{
				int max = 1;
				foreach (var r in ranks) if (r != null && r.rankNumber > max) max = r.rankNumber;
				return max;
			}
		}
	}
}
