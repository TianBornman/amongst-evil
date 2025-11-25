using UnityEngine;

public class CameraMovement : MonoBehaviour
{
	// Private Variables
	private PartyCharacter player;

	// Private Methods
	private void Start()
	{
		player = FindFirstObjectByType<PartyCharacter>();
		SetBattleView();
	}

	private void SetBattleView()
	{
		transform.position = player.cameraPosition.position;
		transform.rotation = player.cameraPosition.rotation;

		transform.parent = player.cameraPosition;
	}
}
