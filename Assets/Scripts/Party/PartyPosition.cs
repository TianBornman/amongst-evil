using UnityEngine;

public class PartyPosition : MonoBehaviour
{
	// Editor Variables
	[Header("Settings")]


	// Public Variables
	public Vector3 offset;
	public bool isOccupied = false;

	// Public Methods
	public void SetPosition(Vector3 position)
	{
		transform.position = position + offset;
	}

	// Private Methods
	private void Start()
	{
		offset = transform.position - transform.parent.position;
	}
}
