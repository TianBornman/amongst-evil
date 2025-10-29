using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
	// Private Methods
	private void Update()
	{
		transform.Rotate(Vector3.up, 10f * Time.deltaTime);
	}
}
