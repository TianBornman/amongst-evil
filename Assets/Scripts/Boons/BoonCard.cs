using Midevil.Effect;
using Midevil.Models;
using UnityEngine;

namespace Midevil.Boons
{
	[CreateAssetMenu(menuName = "Brotherhood/Boon Card")]
	public class BoonCard : ScriptableObject
	{
		[Header("Identity")]
		public string cardName;
		[TextArea] public string description;
		public Texture2D icon;

		[Header("Filter")]
		public BoonCategory category;
		[Tooltip("None = universal — can appear for any class. Otherwise the boon only appears when at least one Brother of this class is alive.")]
		public BrotherClass requiredClass = BrotherClass.None;
		public BoonRarity rarity = BoonRarity.Common;
		public BoonTargeting targeting = BoonTargeting.Single;

		[Header("Effect")]
		[Tooltip("EffectData applied to the recipient when this boon is picked. Cleared at run end by RunBoonManager.")]
		public EffectData effect;
		[Min(1)] public int maxPicksPerRun = 3;
	}
}
