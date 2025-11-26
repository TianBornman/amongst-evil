using Midevil.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Midevil.Item
{
	public class Item : MonoBehaviour, IInteractable
	{
		#region Interactable

		public void Interact()
		{
			if (InventoryManager.Instance.AddItem(stats))
				Destroy(gameObject);
		}

		public void OnHoverEnter()
		{
			outline.enabled = true;
		}

		public void OnHoverExit()
		{
			if (outline != null)
				outline.enabled = false;
		}

		#endregion

		// Editor Variables
		public ItemStats stats;
		public float noEffectWeight;
		public List<ItemEffect> possibleEffects = new();

		// Private Variables
		private Outline outline;

		// Private Methods
		private void Awake()
		{
			outline = GetComponent<Outline>();
			outline.enabled = false;

			ItemHelper.Setup(stats, noEffectWeight, possibleEffects);
		}
		
	}
}