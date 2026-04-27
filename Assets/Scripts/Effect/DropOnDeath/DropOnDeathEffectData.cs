using System.Collections.Generic;
using UnityEngine;

namespace Midevil.Effect
{
	[CreateAssetMenu(menuName = "Effects/Drop On Death")]
	public class DropOnDeathEffectData : EffectData
	{
		[Tooltip("On death, rolls 'chance' once. If it passes, one item from this pool is picked uniformly and instantiated. Empty pool = nothing drops.")]
		public List<Item.Item> items = new();

		[Range(0f, 1f)] public float chance = 1f;

		public override Effect CreateRuntime()
		{
			var effect = new DropOnDeathEffect
			{
				items = items,
				chance = chance
			};
			PopulateBase(effect);
			return effect;
		}
	}
}
