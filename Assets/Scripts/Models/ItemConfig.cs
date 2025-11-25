using Midevil.Item;
using System;

namespace Midevil.Models
{
	[Serializable]
	public class ItemConfig
	{
		public ItemReferenceIndex index;
		public EffectReferenceIndex effectIndex;

		public void Set(ItemStats stats)
		{
			index = stats.index;
			effectIndex = stats.effectIndex;
		}

		public void Clear()
		{
			index = default;
			effectIndex = default;
		}
	}
}