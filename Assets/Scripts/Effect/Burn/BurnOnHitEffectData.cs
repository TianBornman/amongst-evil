using UnityEngine;

namespace Midevil.Effect
{
	[CreateAssetMenu(menuName = "Effects/Burn On Hit")]
	public class BurnOnHitEffectData : EffectData
	{
		[Tooltip("EffectData that gets applied to whoever this character hits. Typically a BurningEffectData or similar DoT.")]
		public EffectData burningEffect;

		public override Effect CreateRuntime()
		{
			var effect = new BurnOnHitEffect { burningEffect = burningEffect };
			PopulateBase(effect);
			return effect;
		}
	}
}
