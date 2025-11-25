using Midevil.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
			ItemHelper.Setup(stats);
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