using Midevil.Helpers;
using System.Collections.Generic;
using UnityEngine;

namespace Midevil.Item
{
	public class Item : MonoBehaviour, IInteractable
	{
		#region Interactable

		public void Interact()
		{
			TryPickup();
		}

		public void OnHoverEnter()
		{
			if (outline != null)
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

		[Header("Drop")]
		[SerializeField] private float dropDuration = 0.5f;
		[SerializeField] private float dropArcHeight = 1.5f;
		[SerializeField] private float scatterRadius = 1.25f;
		[SerializeField] private float groundRaycastDistance = 50f;

		[Header("Levitation")]
		[SerializeField] private float hoverHeight = 0.75f;
		[SerializeField] private float bobAmplitude = 0.15f;
		[SerializeField] private float bobFrequency = 2f;
		[SerializeField] private float spinSpeed = 60f;

		// Private Variables
		private Outline outline;
		private Rigidbody rb;
		private Collider col;
		private bool levitating;
		private bool pickupReady;
		private Vector3 startPos;
		private Vector3 landPos;
		private float dropTimer;
		private float bobPhase;

		// Private Methods
		private void Awake()
		{
			outline = GetComponent<Outline>();
			if (outline != null)
				outline.enabled = false;

			rb = GetComponent<Rigidbody>();
			col = GetComponent<Collider>();

			if (rb != null)
			{
				rb.linearVelocity = Vector3.zero;
				rb.angularVelocity = Vector3.zero;
				rb.isKinematic = true;
				rb.useGravity = false;
			}

			if (col != null)
				col.isTrigger = true;

			ItemHelper.Setup(stats, noEffectWeight, possibleEffects);
		}

		private void Start()
		{
			startPos = transform.position;

			var scatter = Random.insideUnitCircle * scatterRadius;
			var rayOrigin = startPos + new Vector3(scatter.x, 0f, scatter.y);

			if (Physics.Raycast(rayOrigin, Vector3.down, out var hit, groundRaycastDistance, RefManager.Instance.groundMask, QueryTriggerInteraction.Ignore))
				landPos = hit.point;
			else
				landPos = new Vector3(rayOrigin.x, startPos.y, rayOrigin.z);
		}

		private void Update()
		{
			if (!levitating)
			{
				dropTimer += Time.deltaTime;
				var t = dropDuration > 0f ? Mathf.Clamp01(dropTimer / dropDuration) : 1f;

				var basePos = Vector3.Lerp(startPos, landPos, t);
				var arc = Mathf.Sin(t * Mathf.PI) * dropArcHeight;
				transform.position = basePos + Vector3.up * arc;
				transform.Rotate(Vector3.up, spinSpeed * 2f * Time.deltaTime, Space.World);

				if (t >= 1f)
				{
					levitating = true;
					bobPhase = 0f;
					Invoke(nameof(EnablePickup), 0.5f);
				}
				return;
			}

			bobPhase += Time.deltaTime * bobFrequency;
			var y = landPos.y + hoverHeight + Mathf.Sin(bobPhase) * bobAmplitude;
			transform.position = new Vector3(landPos.x, y, landPos.z);
			transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
		}

		private void EnablePickup() => pickupReady = true;

		private void OnTriggerEnter(Collider other)
		{
			if (!pickupReady) return;

			if (other.TryGetComponent<Character>(out var character) && character.team == Team.Brotherhood && character.IsAlive)
				TryPickup();
		}

		private void TryPickup()
		{
			if (InventoryManager.Instance.AddItem(stats))
				Destroy(gameObject);
		}
	}
}
