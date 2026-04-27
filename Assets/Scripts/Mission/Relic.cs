using System;
using UnityEngine;

namespace Midevil.Mission
{
	[RequireComponent(typeof(Collider))]
	public class Relic : MonoBehaviour
	{
		public Action onTouched;

		private bool consumed;

		private void OnTriggerEnter(Collider other)
		{
			if (consumed) return;
			if (!other.GetComponentInParent<PartyCharacter>()) return;

			consumed = true;
			onTouched?.Invoke();
			Destroy(gameObject);
		}
	}
}
