using UnityEngine;

namespace Midevil.Ability
{
	[CreateAssetMenu(menuName = "Abilities/Fireball")]
	public class FireballAbilityData : AbilityData
    {
		public GameObject fireballPrefab;
		public float damage;
		public float speed;

		public override Ability CreateRuntime(Character owner)
		{
			return new FireballAbility(owner, this);
		}
	}
}