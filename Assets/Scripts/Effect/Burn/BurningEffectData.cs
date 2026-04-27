using UnityEngine;

namespace Midevil.Effect
{
	[CreateAssetMenu(menuName = "Effects/Burning (DoT)")]
	public class BurningEffectData : EffectData
	{
		public float dps = 3f;

		public override Effect CreateRuntime()
		{
			var effect = new BurningEffect { dps = dps };
			PopulateBase(effect);
			return effect;
		}
	}
}
