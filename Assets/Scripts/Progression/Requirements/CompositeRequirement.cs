using Midevil.Models;
using System.Collections.Generic;
using UnityEngine;

namespace Midevil.Progression
{
	[CreateAssetMenu(fileName = "Composite Requirement", menuName = "Progression/Requirements/Composite")]
	public class CompositeRequirement : RankRequirement
	{
		public enum Mode { AllOf, AnyOf, NofM }

		public Mode mode = Mode.AllOf;
		[Tooltip("Only used when mode is NofM. How many child requirements must be met.")]
		public int n = 1;

		public List<RankRequirement> children = new();

		public override bool IsMet(SectProgressData data)
		{
			int met = MetCount(data);
			return mode switch
			{
				Mode.AllOf => met >= children.Count,
				Mode.AnyOf => met >= 1,
				Mode.NofM => met >= n,
				_ => false
			};
		}

		public override float Progress01(SectProgressData data)
		{
			if (children == null || children.Count == 0) return 1f;

			if (mode == Mode.AnyOf)
			{
				float best = 0f;
				foreach (var c in children) best = Mathf.Max(best, c.Progress01(data));
				return best;
			}

			float total = 0f;
			foreach (var c in children) total += c.Progress01(data);
			float denom = mode == Mode.NofM ? Mathf.Max(1, n) : children.Count;
			return Mathf.Clamp01(total / denom);
		}

		public override string ProgressText(SectProgressData data)
		{
			int met = MetCount(data);
			return mode switch
			{
				Mode.AllOf => $"{met} / {children.Count}",
				Mode.AnyOf => met >= 1 ? "Done" : "0 / 1",
				Mode.NofM => $"{met} / {n}",
				_ => ""
			};
		}

		private int MetCount(SectProgressData data)
		{
			int met = 0;
			foreach (var c in children) if (c != null && c.IsMet(data)) met++;
			return met;
		}
	}
}
