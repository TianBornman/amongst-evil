using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class DamageNumber : MonoBehaviour
{
	// Editor Variables
	[Header("Animation Settings")]
	public float lifetime = 1.2f;

	[Tooltip("Base height of the rise")]
	public float baseRise = 1f;

	[Tooltip("How much random +/- variance applied to rise")]
	public float riseVariance = 0.35f;

	[Tooltip("Horizontal drift strength")]
	public float horizontalDrift = 0.5f;

	[Tooltip("Randomness multiplier for drift amount")]
	public float driftVariance = 0.4f;

	public AnimationCurve riseCurve;
	public AnimationCurve fadeCurve;

	// Public Variables
	[Header("References")]
	public TextMeshProUGUI text;

	[HideInInspector] public Camera cam;

	// Private Variables
	private float timer;
	private Vector3 startPos;
	private float randomRise;
	private Vector2 randomDriftDir;   // 2D drift direction
	private float randomCurveOffset;  // shifts curve phase slightly
	private Action onFinish;

	// Public Methods
	public void Setup(float amount, Color color, Action finished)
	{
		timer = 0f;
		startPos = transform.position;
		onFinish = finished;

		text.text = amount.ToString();
		text.color = color;

		// Randomize rise amount
		randomRise = baseRise + Random.Range(-riseVariance, riseVariance);

		// Random horizontal drift direction & magnitude
		Vector2 dir = Random.insideUnitCircle.normalized;
		float magnitude = horizontalDrift + Random.Range(-driftVariance, driftVariance);
		randomDriftDir = dir * magnitude;

		// Random time offset so curves don’t sync perfectly
		randomCurveOffset = Random.Range(-0.15f, 0.15f);
	}

	// Private Methods
	private void Update()
	{
		timer += Time.deltaTime;
		float rawT = timer / lifetime;

		if (rawT >= 1f)
		{
			gameObject.SetActive(false);
			onFinish?.Invoke();
			return;
		}

		// Apply offset to t but clamp to 0–1
		float t = Mathf.Clamp01(rawT + randomCurveOffset);

		// Vertical rise using curve
		float yOffset = riseCurve.Evaluate(t) * randomRise;

		// Horizontal drift
		Vector3 horizontalOffset = new Vector3(randomDriftDir.x, 0f, randomDriftDir.y) * t;

		transform.position = startPos + Vector3.up * yOffset + horizontalOffset;

		// Fade out
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
