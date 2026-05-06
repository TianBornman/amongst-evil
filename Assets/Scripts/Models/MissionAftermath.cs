using System.Collections.Generic;

namespace Midevil.Models
{
	public class MissionAftermath
	{
		public bool success;
		public Mission.Mission mission;

		public int standingBefore;
		public int standingAfter;
		public int standingDelta;

		public string rankBeforeName;
		public string rankAfterName;
		public bool ascended;
		public bool ascensionPending;
		public string nextRankName;

		public List<string> fallen = new();
		public List<string> survivors = new();
	}
}
