using System;
using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
	// Editor Variables
	[Header("Animation Settings")]
	public float lifetime = 1.2f;
	public float riseAmount = 1f;
	public AnimationCurve riseCurve;
	public AnimationCurve fadeCurve;

	// Public Variables
	[Header("References")]
	public TextMeshProUGUI text;

	[HideInInspector] public Camera cam;

	// Private Variables
	private float timer;
	private Vector3 startPos;
	private Action onFinish;

	// Public Methods
	public void Setup(float amount, Color color, Action finished)
	{
		timer = 0f;
		startPos = transform.position;
		onFinish = finished;

		text.text = amount.ToString();
		text.color = color;
	}

	// Private Methods
	private void Update()
	{
		timer += Time.deltaTime;
		float t = timer / lifetime;

		if (t >= 1f)
		{
			gameObject.SetActive(false);
			onFinish?.Invoke();
			return;
		}

		// Rise
		float yOffset = riseCurve.Evaluate(t) * riseAmount;
		transform.position = startPos + Vector3.up * yOffset;

		// Fade
		Color c = text.color;
		c.a = fadeCurve.Evaluate(t);
		text.color = c;
	}

	private void LateUpdate()
	{
		if (cam == null)
			cam = Camera.main;

		transform.forward = cam.transform.forward;
	}
}
