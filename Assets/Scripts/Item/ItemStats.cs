using Midevil.Effect;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Midevil.Item
{
	[Serializable]
	public class ItemStats
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
		public AbilityReferenceIndex abilityIndex;
		public List<EffectData> effects;
		[HideInInspector] public EffectReferenceIndex effectIndex;
		public float dropChance;
		public bool shouldRoll;
	}
}