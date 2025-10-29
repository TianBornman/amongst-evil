using System;

namespace Midevil.Effect
{
	[Serializable]
	public class HealOnKillEffect : Effect, IOnKill
	{
		public string effectId;
		public float healAmount = 2;

		public void OnKill(Character owner, Character killer)
		{
			killer.Damage(-healAmount);
		}
	}
}