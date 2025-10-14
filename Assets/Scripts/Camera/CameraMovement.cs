using UnityEngine;

public class CameraMovement : MonoBehaviour
{
	// Private Variables
	private Player player;

	// Private Methods
	private void Start()
	{
		player = FindFirstObjectByType<Player>();
		SetBattleView();
	}

	private void SetBattleView()
	{
		transform.position = player.cameraPosition.position;
		transform.rotation = player.cameraPosition.rotation;

		transform.parent = player.cameraPosition;
	}
}
