using UnityEngine;
using UnityEngine.Serialization;

namespace Midevil.Effect
{
	public enum EffectStackPolicy
	{
		Refresh,
		Replace,
		Reject,
		Allow
	}

	public abstract class EffectData : ScriptableObject
	{
		[FormerlySerializedAs("effectType")]
		[Tooltip("Effects sharing the same group cannot coexist on the same character. Empty = no group, multiple instances allowed.")]
		public string group;

		[Tooltip("How to resolve a conflict when an effect with this group is already applied. Refresh (extend duration), Replace (kick the existing), Reject (existing wins), Allow (both coexist).")]
		public EffectStackPolicy stackPolicy = EffectStackPolicy.Refresh;

		public Texture2D icon;
		public string itemPrefix;
		public float duration = -1;

		public abstract Effect CreateRuntime();

		protected void PopulateBase(Effect effect)
		{
			effect.source = this;
			effect.group = group;
			effect.stackPolicy = stackPolicy;
			effect.icon = icon;
			effect.duration = duration;
		}
	}
}
