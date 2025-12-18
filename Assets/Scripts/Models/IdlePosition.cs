using System;
using Unity.Cinemachine;
using UnityEngine;

namespace Midevil.Models
{
	[Serializable]
	public class IdlePosition
	{
		public Transform position;
		public CinemachineCamera camera;
		public int animationIndex;
	}
}