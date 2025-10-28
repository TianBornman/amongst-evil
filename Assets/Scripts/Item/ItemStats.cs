using System;
using System.Collections.Generic;
using UnityEngine;

namespace Midevil.Item
{
	[Serializable]
	public struct ItemStats
	{
		public string name;
		[TextArea] public string description;
		public ItemType type;
		public Texture2D icon;
		public GameObject visual;
		public ItemAnimationType animationType;
		public Buff buff;
		public List<BuffData> effects;
		public float dropChance;
	}
}