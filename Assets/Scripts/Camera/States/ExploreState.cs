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

		}

		public void Exit() { }

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
	}
}