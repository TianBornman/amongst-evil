using System;
using UnityEngine;

namespace Midevil.Camera
{
	[Serializable]
	public struct CameraSettings
	{
		public float distance;
		public float height;
		public float followSpeed;
		public float rotateSpeed;
		public float fov;
		public Vector3 offset;
	}
}