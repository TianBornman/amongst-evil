using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
	// Editor Variables
	public Transform hubPosition;

	// Private Variables
	private bool atStartMenu = true;

	// Public Methods
	public void MoveToHub()
	{
		atStartMenu = false;
	}

	// Private Methods
	private void Update()
	{
		if (atStartMenu)
			transform.Rotate(Vector3.up, 10f * Time.deltaTime);
		else
		{
			transform.SetPositionAndRotation(
				Vector3.Lerp(transform.position, hubPosition.position, Time.deltaTime), 
				Quaternion.Slerp(transform.rotation, hubPosition.rotation, Time.deltaTime));
		}
	}
}
