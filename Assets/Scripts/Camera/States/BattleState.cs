using UnityEngine;


namespace Midevil.Camera.States
{
	internal class BattleState : IState
	{
		private CameraMovement camera;

		public BattleState(CameraMovement camera)
		{
			this.camera = camera;
		}

		public void Enter()
		{
			camera.targetSettings = camera.combatSettings;
		}

		public void Exit() { }

		public void Update()
		{
			camera.active = CameraHelpers.LerpSettings(camera.active, camera.targetSettings, Time.deltaTime * 2);
			CameraHelpers.MoveCamera(camera);
		}
	}
}