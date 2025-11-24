using Midevil.Ability;
using Midevil.Effect;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Midevil.Item
{
	[Serializable]
	public struct ItemStats
	{
		public Guid id;
		public ItemReferenceIndex index;
		public string name;
		[TextArea] public string description;
		public ItemType type;
		public Texture2D icon;
		public GameObject visual;
		public ItemAnimationType animationType;
		public Buff buff;
		public AbilityData ability;
		public List<EffectData> effects;
		public float dropChance;
	}
}