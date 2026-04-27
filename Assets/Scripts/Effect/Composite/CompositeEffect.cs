using System.Collections.Generic;

namespace Midevil.Effect
{
	public class CompositeEffect : Effect
	{
		public List<Effect> children = new();

		public override void OnApply(Character owner)
		{
			for (int i = 0; i < children.Count; i++)
			{
				var child = children[i];
				child.parent = this;
				owner.effects.AddEffect(child);
			}
		}

		public override void OnRemove(Character owner)
		{
			for (int i = owner.currentEffects.Count - 1; i >= 0; i--)
				if (owner.currentEffects[i].parent == this)
					owner.effects.RemoveEffect(owner.currentEffects[i]);
		}
	}
}
