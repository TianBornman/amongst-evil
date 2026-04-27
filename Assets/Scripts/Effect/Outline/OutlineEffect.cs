using UnityEngine;

namespace Midevil.Effect
{
	public class OutlineEffect : Effect
	{
		public Color color = Color.white;
		public float width = 4f;

		private Outline outline;
		private Color previousColor;
		private float previousWidth;
		private bool previousEnabled;
		private bool captured;

		public override void OnApply(Character owner)
		{
			outline = owner.GetComponent<Outline>();
			if (outline == null) return;

			previousColor = outline.OutlineColor;
			previousWidth = outline.OutlineWidth;
			previousEnabled = outline.enabled;
			captured = true;

			outline.OutlineColor = color;
			outline.OutlineWidth = width;
			outline.enabled = true;
		}

		public override void OnRemove(Character owner)
		{
			if (!captured || outline == null) return;
			outline.OutlineColor = previousColor;
			outline.OutlineWidth = previousWidth;
			outline.enabled = previousEnabled;
		}
	}
}
