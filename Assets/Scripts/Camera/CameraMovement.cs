using UnityEngine;

public class CameraMovement : MonoBehaviour
{
	// Editor Variables
	public Transform mapView;
	public Transform partyView;

	// Private Variables
	private Vector3 mapViewOffset;
	private Transform target;
	bool explore;

	// Public Methods
	public void Explore()
	{
		explore = true;
		target = mapView;
	}

	public void Battle()
	{
		explore = false;
		target = partyView;
	}

	public void UpdateMapView(Vector3 position)
	{
		mapView.position = position + mapViewOffset;
	}

	// Private Methods
	private void Start()
	{
		mapViewOffset = mapView.position;

		Explore();

		InputManager.Instance.CameraToggleAction = CameraToggle;
	}

	private void Update()
	{
		if (target == null)
			return;

		transform.SetPositionAndRotation(
			Vector3.Lerp(transform.position, target.position, Time.deltaTime * 2),
			Quaternion.Slerp(transform.rotation, target.rotation, Time.deltaTime * 2));
	}

	private void CameraToggle()
	{
		if (explore)
			Battle();
		else
			Explore();
	}
}
