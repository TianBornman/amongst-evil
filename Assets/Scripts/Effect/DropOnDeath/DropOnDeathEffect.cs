using System.Collections.Generic;
using UnityEngine;

namespace Midevil.Effect
{
	public class DropOnDeathEffect : Effect, IOnDeath
	{
		public List<Item.Item> items;
		public float chance;

		public void OnDeath(Character owner, Character killer)
		{
			if (items == null || items.Count == 0) return;
			if (Random.value >= chance) return;

			var item = items[Random.Range(0, items.Count)];
			if (item == null) return;

			Object.Instantiate(item, owner.transform.position + Vector3.up * 1.5f, Quaternion.identity);
		}
	}
}
