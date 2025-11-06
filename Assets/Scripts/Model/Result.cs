using System;

namespace Midevil.Model
{
	[Serializable]
	public class Result
	{
		// Character
		public float xpGained;

		// Combat
		public int kills;
		public float damageDealt;
		public float damageTaken;
		public float healed;
		public int hits;
		public int criticalHits;
		public int abilitiesUsed;
	}
}