using UnityEngine;

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
			camera.targetSettings = camera.exploreSettings;
			camera.followTarget = camera.partyCenter;
		}

		public void Exit() { }

		public void Update()
		{
			camera.active = LerpSettings(camera.active, camera.targetSettings, Time.deltaTime * 2);
			CameraHelpers.MoveCamera(camera);
		}

		private CameraSettings LerpSettings(CameraSettings a, CameraSettings b, float t)
		{
			a.distance = Mathf.Lerp(a.distance, b.distance, t);
			a.height = Mathf.Lerp(a.height, b.height, t);
			a.followSpeed = Mathf.Lerp(a.followSpeed, b.followSpeed, t);
			a.rotateSpeed = Mathf.Lerp(a.rotateSpeed, b.rotateSpeed, t);
			a.offset = Vector3.Lerp(a.offset, b.offset, t);
			a.fov = Mathf.Lerp(a.fov, b.fov, t);
			return a;
		}
	}
}