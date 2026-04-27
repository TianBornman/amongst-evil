using System.Collections.Generic;
using UnityEngine;

namespace Midevil.Effect
{
	[CreateAssetMenu(menuName = "Effects/Application Group")]
	public class EffectApplicationGroup : ScriptableObject
	{
		[Tooltip("Designer label — purely for organisation.")]
		public string label;

		[Tooltip("If true, at most one entry from this group is applied (weighted pick using each entry's weight, gated by its chance). If false, every entry rolls independently.")]
		public bool pickOnlyOne;

		[Tooltip("Only used when pickOnlyOne is true — chance the group rolls anything at all. Set to 0 to always roll.")]
		[Range(0f, 1f)] public float skipChance;

		public List<EffectApplication> entries = new();

		public void Apply(Character target)
		{
			if (target == null || entries == null || entries.Count == 0) return;

			if (pickOnlyOne)
				ApplyOne(target);
			else
				ApplyEach(target);
		}

		private void ApplyEach(Character target)
		{
			for (int i = 0; i < entries.Count; i++)
			{
				var entry = entries[i];
				if (!entry.RollChance()) continue;
				target.effects.AddEffect(entry.effect.CreateRuntime());
			}
		}

		private void ApplyOne(Character target)
		{
			if (skipChance > 0f && Random.value < skipChance) return;

			float total = 0f;
			for (int i = 0; i < entries.Count; i++)
				if (entries[i].effect != null)
					total += Mathf.Max(0f, entries[i].weight);

			if (total <= 0f) return;

			float pick = Random.value * total;
			float cursor = 0f;

			for (int i = 0; i < entries.Count; i++)
			{
				var entry = entries[i];
				if (entry.effect == null) continue;

				cursor += Mathf.Max(0f, entry.weight);
				if (pick > cursor) continue;

				if (entry.chance > 0f && Random.value >= entry.chance) return;
				target.effects.AddEffect(entry.effect.CreateRuntime());
				return;
			}
		}
	}
}
