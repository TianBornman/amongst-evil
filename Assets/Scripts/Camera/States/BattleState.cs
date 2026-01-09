using UnityEngine;


namespace Midevil.Camera.States
{
	internal class BattleState : IState
	{
		public bool CanExit { get; private set; } = true;

		private CameraMovement camera;

		public BattleState(CameraMovement camera)
		{
			this.camera = camera;
		}

		public void Enter()
		{
			camera.battleCamera.enabled = true;
		}

		public void Exit() 
		{ 
			camera.battleCamera.enabled = false;
		}

		public void Update()
		{

		}
	}
}