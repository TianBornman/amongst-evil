using UnityEngine;

public class CameraMovement : StateMachine<CameraState>
{
	// Editor Variables
	public Transform mapView;
	public Transform partyView;

	// Private Variables
	private Transform target;

	// Override Methods
	protected override void SetState(CameraState state)
	{
		base.SetState(state);

		switch (state)
		{
			case CameraState.MapView:
				MapView();
				break;
			case CameraState.BattleView:
				BattleView();
				break;
		}
	}

	// State Methods
	private void MapView()
	{
		target = mapView;
	}

	private void BattleView()
	{
		target = partyView;
	}

	// Public Methods
	public void SetMapView()
	{
		SetState(CameraState.MapView);
	}

	public void SetBattleView()
	{
		SetState(CameraState.BattleView);
	}

	// Private Methods
	private void Start()
	{
		SetMapView();

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
		if (State == CameraState.MapView)
			SetBattleView();
		else
			SetMapView();
	}
}
