using UnityEngine;

namespace Midevil.Models
{
	[CreateAssetMenu(menuName = "Brotherhood/Class Data")]
	public class ClassData : ScriptableObject
	{
		[Header("Identity")]
		public BrotherClass classType;
		public string className;
		[TextArea] public string description;
		public Texture2D icon;
		public Color themeColor = Color.white;

		[Header("Combat Profile")]
		[Tooltip("Per-class baseStats. Overrides the prefab's baseStats when this class is assigned to an Identity.")]
		public Stats baseStats;

		[Header("Loadout")]
		[Tooltip("Auto-equipped on the recruit's identity.weaponConfig at recruit time.")]
		public ItemReferenceIndex starterWeapon = ItemReferenceIndex.None;
	}
}
