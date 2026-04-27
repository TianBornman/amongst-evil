using System.Collections.Generic;
using UnityEngine;

namespace Midevil.Effect
{
	[CreateAssetMenu(menuName = "Effects/Composite")]
	public class CompositeEffectData : EffectData
	{
		[Tooltip("Each child EffectData is instantiated and applied alongside this composite. Removing the composite removes all children.")]
		public List<EffectData> children = new();

		public override Effect CreateRuntime()
		{
			var effect = new CompositeEffect();
			PopulateBase(effect);

			for (int i = 0; i < children.Count; i++)
				if (children[i] != null)
					effect.children.Add(children[i].CreateRuntime());

			return effect;
		}
	}
}
