using UnityEngine;
using UnityEngine.UI;

public class HealthbarVisual : MonoBehaviour
{
	// Editor Variables
	[Header("References")]
	public CanvasGroup canvasGroup;
	public Transform healthBar;
	public Image healthBarFront;

	[Header("Settings")]
	public float visibleDuration = 2f;
	public float fadeSpeed = 5f;

	private float hideTimer;
	private bool visible;

	// Public Methods
	public void SetHealth(float current, float max)
	{
		healthBarFront.fillAmount = current / max;

		Show();
	}

	// Private Methods
	private void Awake()
	{
		canvasGroup.alpha = 0f;
		visible = false;
	}

	private void Update()
	{
		if (!visible)
			return;

		hideTimer -= Time.deltaTime;

		float targetAlpha = hideTimer > 0f ? 1f : 0f;
		canvasGroup.alpha = Mathf.MoveTowards(
			canvasGroup.alpha,
			targetAlpha,
			fadeSpeed * Time.deltaTime
		);

		if (canvasGroup.alpha <= 0f)
			visible = false;
	}

	private void LateUpdate()
	{
		if (Camera.main == null)
			return;

		Vector3 lookDir = Camera.main.transform.forward;
		lookDir.y = 0f;
		healthBar.forward = lookDir;
	}

	private void Show()
	{
		visible = true;
		hideTimer = visibleDuration;
	}
}
