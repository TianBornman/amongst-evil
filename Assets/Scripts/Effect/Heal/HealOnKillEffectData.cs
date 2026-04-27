using UnityEngine;

namespace Midevil.Effect
{
	[CreateAssetMenu(menuName = "Effects/Heal On Kill")]
	public class HealOnKillEffectData : EffectData
	{
		public float healAmount = 2f;

		public override Effect CreateRuntime()
		{
			var effect = new HealOnKillEffect { healAmount = healAmount };
			PopulateBase(effect);
			return effect;
		}
	}
}
