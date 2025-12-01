using UnityEngine;
using UnityEngine.UI;

public class HealthbarVisual : MonoBehaviour
{
	// Editor Variables
	[Header("References")]
	public Transform healthBar;
	public Image healthBarFront;

	// Public Methods
	public void SetHealth(float current, float max)
	{
		healthBarFront.fillAmount = current / max;
	}

	// Private Methods
	private void LateUpdate()
	{
		Vector3 forward = Camera.main.transform.forward;
		forward.y = 0f;
		healthBar.transform.forward = forward;
	}
}
