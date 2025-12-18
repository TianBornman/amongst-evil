using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
	// Editor Variables
	[Header("References")]
	public Transform hubPosition;

	// Private Variables
	private Quaternion initalRotation;

	// Private Methods
	private void Start()
	{
		initalRotation = hubPosition.rotation;
	}

	private void Update()
	{
		if (GameManager.Instance.AtMainMenu)
			hubPosition.Rotate(Vector3.up, 10f * Time.deltaTime);
		else
			hubPosition.rotation = Quaternion.Slerp(hubPosition.rotation, initalRotation, Time.deltaTime);
	}
}
