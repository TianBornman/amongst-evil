using UnityEngine;

namespace Midevil.Camera
{
	public static class CameraHelpers
	{
		public static CameraSettings LerpSettings(CameraSettings a, CameraSettings b, float t)
		{
			a.distance = Mathf.Lerp(a.distance, b.distance, t);
			a.height = Mathf.Lerp(a.height, b.height, t);
			a.followSpeed = Mathf.Lerp(a.followSpeed, b.followSpeed, t);
			a.rotateSpeed = Mathf.Lerp(a.rotateSpeed, b.rotateSpeed, t);
			a.offset = Vector3.Lerp(a.offset, b.offset, t);
			a.fov = Mathf.Lerp(a.fov, b.fov, t);
			return a;
		}

		public static void MoveCamera(CameraMovement cam)
		{
			if (!cam.followTarget) return;

			var pos = cam.followTarget.position
					  - cam.followTarget.forward * cam.active.distance
					  + Vector3.up * cam.active.height
					  + cam.active.offset;
		
			cam.transform.SetPositionAndRotation(Vector3.Lerp(cam.transform.position, pos, Time.deltaTime * cam.active.followSpeed), 
				Quaternion.Slerp(cam.transform.rotation,
				Quaternion.LookRotation(cam.followTarget.position - cam.transform.position),
				Time.deltaTime * cam.active.rotateSpeed
			));
		}
	}
}
