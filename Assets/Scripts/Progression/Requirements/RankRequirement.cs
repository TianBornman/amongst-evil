using Midevil.Models;
using UnityEngine;

namespace Midevil.Progression
{
	public abstract class RankRequirement : ScriptableObject
	{
		[Tooltip("Short label shown to the player. e.g. \"Standing\", \"Slay Zombies\", \"Complete Threat III Purges\".")]
		public string label;

		[TextArea]
		[Tooltip("Optional flavour line.")]
		public string description;

		public abstract bool IsMet(SectProgressData data);
		public abstract float Progress01(SectProgressData data);
		public abstract string ProgressText(SectProgressData data);
	}
}
