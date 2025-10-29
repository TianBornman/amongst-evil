using UnityEngine;

namespace Midevil.Effect
{
	[CreateAssetMenu(menuName = "Effects/Burn On Hit")]
	public class BurnOnHitEffectData : EffectData
	{
		public string effectId = "burning";
		public float effectDps = 3f;
		public float effectDuration = 3f;

		public override Effect CreateRuntime()
		{
			return new BurnOnHitEffect
			{
				id = id,
				duration = duration,
				effectId = effectId,
				effectDps = effectDps,
				effectDuration = effectDuration
			};
		}
	}
}
