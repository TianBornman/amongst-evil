using UnityEngine;

namespace Midevil.Ability
{
	[CreateAssetMenu(menuName = "Abilities/Heal Potion")]
	public class HealPotionAbilityData : AbilityData
	{
		public float healAmount = 3f;

		public override Ability CreateRuntime(Character owner)
		{
			return new HealPotionAbility(owner, this);
		}
	}
}
