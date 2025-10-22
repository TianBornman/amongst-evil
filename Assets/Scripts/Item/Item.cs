using System.Collections;
using UnityEngine;

namespace Midevil.Item
{
	public class Item : MonoBehaviour
	{
		// Editor Variables
		public ItemStats stats;

		// Private Methods
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