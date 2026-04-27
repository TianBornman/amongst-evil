using UnityEngine;

namespace Midevil.Effect
{
	[CreateAssetMenu(menuName = "Effects/Stat Modifier")]
	public class StatModifierEffectData : EffectData
	{
		[Tooltip("Stat values are added directly to the owner's buffs (positive = buff, negative = debuff).")]
		public Stats statDelta;

		public override Effect CreateRuntime()
		{
			var effect = new StatModifierEffect { statDelta = statDelta };
			PopulateBase(effect);
			return effect;
		}
	}
}
