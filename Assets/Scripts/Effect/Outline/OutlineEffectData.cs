using UnityEngine;

namespace Midevil.Effect
{
	[CreateAssetMenu(menuName = "Effects/Outline")]
	public class OutlineEffectData : EffectData
	{
		public Color color = Color.white;
		public float width = 4f;

		public override Effect CreateRuntime()
		{
			var effect = new OutlineEffect
			{
				color = color,
				width = width
			};
			PopulateBase(effect);
			return effect;
		}
	}
}
