using UnityEngine;

namespace Midevil.Effect
{
	[CreateAssetMenu(menuName = "Effects/Stat Multiplier")]
	public class StatMultiplierEffectData : EffectData
	{
		[Tooltip("Multiplied directly into the owner's baseStats on apply, divided back on remove. Use this when a stat change should scale with the enemy's base values (e.g. variant tiers).")]
		public float maxHealthMultiplier = 1f;
		public float damageMultiplier = 1f;
		public float moveSpeedMultiplier = 1f;
		public float attackSpeedMultiplier = 1f;
		public float sizeMultiplier = 1f;
		public float xpValueMultiplier = 1f;

		public override Effect CreateRuntime()
		{
			var effect = new StatMultiplierEffect
			{
				maxHealthMultiplier = maxHealthMultiplier,
				damageMultiplier = damageMultiplier,
				moveSpeedMultiplier = moveSpeedMultiplier,
				attackSpeedMultiplier = attackSpeedMultiplier,
				sizeMultiplier = sizeMultiplier,
				xpValueMultiplier = xpValueMultiplier
			};
			PopulateBase(effect);
			return effect;
		}
	}
}
