using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Guid = System.Guid;

namespace Midevil.Item
{
	public class Item : MonoBehaviour
	{
		// Editor Variables
		public ItemStats stats;
		public float noEffectWeight;
		public List<ItemEffect> possibleEffects = new();

		// Private Methods
		private void Awake()
		{
			stats.id = Guid.NewGuid();

			var totalWeight = noEffectWeight + possibleEffects.Sum(e => e.weight);
			var randomValue = Random.Range(0, totalWeight);

			float cumulative = noEffectWeight;

			if (randomValue <= cumulative) return;

			foreach (var effect in possibleEffects)
			{
				cumulative += effect.weight;
				if (randomValue <= cumulative)
				{
					stats.name = $"{effect.effect.itemPrefix} {stats.name}";
					stats.effects.Add(effect.effect);
					return;
				}
			}
		}

		private void Start()
		{
			StartCoroutine(ShowItemUI());
		}

		private IEnumerator ShowItemUI()
		{
			yield return new WaitForSeconds(1.5f);

			UiManager.Instance.BindItemPickUp(this);
			UiManager.Instance.ShowItemPickUp();
		}
	}
}