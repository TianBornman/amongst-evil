using UnityEngine;

namespace Midevil.Effect
{
	public abstract class EffectData : ScriptableObject
	{
		public string id;
		public string itemPrefix;
		public float duration = -1;

		public abstract Effect CreateRuntime();
	}
}