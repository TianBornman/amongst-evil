using System;

namespace Midevil.Models
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

		public void Add(Result other)
		{
			xpGained += other.xpGained;
			kills += other.kills;
			damageDealt += other.damageDealt;
			damageTaken += other.damageTaken;
			healed += other.healed;
			hits += other.hits;
			criticalHits += other.criticalHits;
			abilitiesUsed += other.abilitiesUsed;
		}

		public void Clear()
		{
			xpGained = 0;
			kills = 0;
			damageDealt = 0;
			damageTaken = 0;
			healed = 0;
			hits = 0;
			criticalHits = 0;
			abilitiesUsed = 0;
		}
	}
}