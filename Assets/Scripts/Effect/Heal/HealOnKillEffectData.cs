using UnityEngine;

namespace Midevil.Effect
{
	[CreateAssetMenu(menuName = "Effects/Heal On Kill")]
	public class HealOnKillEffectData : EffectData
	{
		public string effectId = "healonkill";
		public float healAmount;

		public override Effect CreateRuntime()
		{
			return new HealOnKillEffect
			{
				id = id,
				duration = duration,
				effectId = effectId,
				healAmount = healAmount
			};
		}
	}
}