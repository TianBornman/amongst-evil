using UnityEngine;

namespace Midevil.Ability
{
	public class HealPotionAbility : Ability
	{
		private HealPotionAbilityData Potion => (HealPotionAbilityData)data;

		public HealPotionAbility(Character owner, HealPotionAbilityData data) : base(owner, data) { }

		protected override void Execute()
		{
			owner.Heal(Potion.healAmount);
			Debug.Log($"Used {Potion.abilityName}, healed {Potion.healAmount}");
		}
	}
}
