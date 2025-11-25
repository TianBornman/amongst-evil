using Midevil.Effect;
using Midevil.Item;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Guid = System.Guid;

namespace Midevil.Helpers
{
	public static class ItemHelper
	{
		public static ItemStats Clone(this ItemStats original)
		{
			return new ItemStats
			{
				id = original.id,
				index = original.index,
				name = original.name,
				description = original.description,
				type = original.type,
				icon = original.icon,
				visual = original.visual,
				animationType = original.animationType,
				buff = original.buff,
				abilityIndex = original.abilityIndex,
				effects = original.effects != null
					? new List<EffectData>(original.effects)
					: new List<EffectData>(),
				effectIndex = original.effectIndex,
				dropChance = original.dropChance,
				shouldRoll = original.shouldRoll
			};
		}

		public static void Setup(ItemStats stats, float noEffectWeight = 0, List<ItemEffect> possibleEffects = null)
		{
			stats.id = Guid.NewGuid();

			if (!stats.shouldRoll)
			{
				AddEffect(stats, stats.effectIndex);
				return;
			}

			// Roll Random Effect
			var totalWeight = noEffectWeight + possibleEffects.Sum(e => e.weight);
			var randomValue = Random.Range(0, totalWeight);

			float cumulative = noEffectWeight;

			if (randomValue <= cumulative) return;

			foreach (var effect in possibleEffects)
			{
				cumulative += effect.weight;
				if (randomValue <= cumulative)
				{
					AddEffect(stats, effect.effectIndex);
					return;
				}
			}
		}

		private static void AddEffect(ItemStats stats, EffectReferenceIndex index)
		{
			if (index == EffectReferenceIndex.None) return;

			var effectData = RefManager.Instance.GetEffect(index);

			stats.name = $"{effectData.itemPrefix} {stats.name}";
			stats.effects.Add(effectData);
			stats.effectIndex = index;
		}
	}
}
