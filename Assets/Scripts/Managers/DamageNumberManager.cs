using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DamageNumberManager : Singleton<DamageNumberManager>
{
	// Editor Variables
	[Header("References")]
	[HideInInspector] public UIDocument uiDocument;
	public UIDocument uiDocumentPrefab;
	public VisualTreeAsset damageNumberTemplate;
	public int initialPoolSize = 20;

	// Private Variables
	private VisualElement root;
	private Camera mainCamera;

	private readonly Queue<DamageNumber> pool = new();
	private readonly List<DamageNumber> active = new();

	// Override Methods
	protected override void Awake()
	{
		base.Awake();

		mainCamera = Camera.main;

		uiDocument = Instantiate(uiDocumentPrefab).GetComponent<UIDocument>();
		root = uiDocument.rootVisualElement;

		for (int i = 0; i < initialPoolSize; i++)
			pool.Enqueue(CreateNewElement());
	}

	// Public Methods
	public void ShowDamage(Vector3 worldPos, float amount, Color? color = null)
	{
		DamageNumber dmg = pool.Count > 0 ? pool.Dequeue() : CreateNewElement();

		dmg.worldPosition = worldPos;
		dmg.remainingTime = dmg.lifetime = 1.2f;
		dmg.active = true;

		var label = dmg.element.Q<Label>("Text");
		label.text = amount.ToString();
		label.style.color = color ?? Color.red;

		dmg.element.style.display = DisplayStyle.Flex;
		dmg.element.style.opacity = 1f;

		active.Add(dmg);
	}

	// Private Methods
	private DamageNumber CreateNewElement()
	{
		var ve = damageNumberTemplate.Instantiate();
		ve.AddToClassList("damage-number");

		var dmg = new DamageNumber
		{
			element = ve,
			lifetime = 0f,
			active = false
		};

		root.Add(ve);
		ve.style.display = DisplayStyle.None;
		return dmg;
	}

	private void Update()
	{
		for (int i = active.Count - 1; i >= 0; i--)
		{
			var dmg = active[i];
			dmg.remainingTime -= Time.deltaTime;

			if (dmg.remainingTime <= 0f)
			{
				Deactivate(dmg);
				active.RemoveAt(i);
				continue;
			}

			// Animate upward and fade
			float t = 1f - (dmg.remainingTime / dmg.lifetime);
			Vector3 offset = Vector3.up * (1f * t);
			Vector3 screenPos = mainCamera.WorldToScreenPoint(dmg.worldPosition + offset);

			dmg.element.style.left = screenPos.x;
			dmg.element.style.top = Screen.height - screenPos.y;
			dmg.element.style.opacity = dmg.remainingTime / dmg.lifetime;
		}
	}

	private void Deactivate(DamageNumber dmg)
	{
		dmg.active = false;
		dmg.element.style.display = DisplayStyle.None;
		pool.Enqueue(dmg);
	}

	private class DamageNumber
	{
		public VisualElement element;
		public Vector3 worldPosition;
		public float lifetime;
		public float remainingTime;
		public bool active;
	}
}