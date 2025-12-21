using UnityEngine;
using UnityEngine.InputSystem;

namespace Midevil.Camera.States
{
	internal class ExploreState : IState
	{
		public bool CanExit { get; private set; } = true;

		private CameraMovement camera;

		public ExploreState(CameraMovement camera)
		{
			this.camera = camera;
		}

		public void Enter()
		{
			InputManager.Instance.MapViewInputActionStarted = EnableMapView;
			InputManager.Instance.MapViewInputActionCancelled = DisableMapView;
		}

		public void Exit() 
		{
            InputManager.Instance.MapViewInputActionStarted = null;
            InputManager.Instance.MapViewInputActionCancelled = null;
        }

		public void Update()
		{
			if (Mouse.current.middleButton.isPressed)
			{
				camera.axisController.enabled = true;
			}
			else
			{
				camera.axisController.enabled = false;
			}
		}

		// Private Methods
		private void EnableMapView()
		{
			camera.partyCamera.enabled = false;
			camera.mapCamera.enabled = true;
		}

		private void DisableMapView() 
		{
            camera.mapCamera.enabled = false;
            camera.partyCamera.enabled = true;
        }
	}
}