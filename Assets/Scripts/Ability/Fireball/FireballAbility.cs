using UnityEngine;

namespace Midevil.Ability
{
	public class FireballAbility : Ability
	{
		private FireballAbilityData Fireball => (FireballAbilityData)data;

		public FireballAbility(Character owner, FireballAbilityData data) : base(owner, data) { }

		protected override void Execute()
		{
			Debug.Log("Launch Fireball!");
			//var fireball = GameObject.Instantiate(Fireball.fireballPrefab, owner.transform.position + owner.transform.forward, Quaternion.identity);
			//fireball.GetComponent<Projectile>().Init(owner, Fireball.damage, Fireball.speed);
		}
	}
}