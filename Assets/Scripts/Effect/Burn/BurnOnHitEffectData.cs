using UnityEngine;

namespace Midevil.Effect
{
	[CreateAssetMenu(menuName = "Effects/Burn On Hit")]
	public class BurnOnHitEffectData : EffectData
	{
		public string childEffectType = "burning";
		public float effectDps = 3f;
		public float effectDuration = 3f;

		public override Effect CreateRuntime()
		{
			return new BurnOnHitEffect
			{
				effectType = effectType,
				icon = icon,
				duration = duration,
				childEffectType = childEffectType,
				effectDps = effectDps,
				effectDuration = effectDuration
			};
		}
	}
}
