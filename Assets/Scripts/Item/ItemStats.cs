using System;
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
		public Buff buff;
		public float dropChance;
	}
}