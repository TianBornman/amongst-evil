using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : StateMachine<CameraState>
{
	#region Input

	private InputSystem_Actions inputActions;
	private InputAction cameraToggleAction;

	private void OnEnable()
	{
		inputActions = new InputSystem_Actions();
		cameraToggleAction = inputActions.Player.CameraToggle;

		inputActions.Enable();
		cameraToggleAction.performed += OnCameraToggle;
	}

	private void OnDisable()
	{
		cameraToggleAction.performed -= OnCameraToggle;
		inputActions.Disable();
	}

	private void OnCameraToggle(InputAction.CallbackContext context)
	{
		CameraToggle();
	}

	#endregion

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
