using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
	// Editor Variables
	public Transform hubPosition;

	// Public Variables
	[HideInInspector] public Transform targetPosition;

	// Private Methods
	private void Update()
	{
		if (GameManager.Instance.AtMenu)
			transform.Rotate(Vector3.up, 10f * Time.deltaTime);
		else
		{
			if (targetPosition == null)
				return;

			transform.SetPositionAndRotation(
				Vector3.Lerp(transform.position, targetPosition.position, Time.deltaTime), 
				Quaternion.Slerp(transform.rotation, targetPosition.rotation, Time.deltaTime));
		}
	}
}
