using UnityEngine;

namespace Midevil.Ability
{
    public abstract class AbilityData : ScriptableObject
    {
		public string abilityName;
		public Texture2D icon;
		public float cooldown = 1f;
		public float manaCost = 0f;
		public bool isConsumable = false;
		public int maxCharges = 1;

		public abstract Ability CreateRuntime(Character owner);
	}
}