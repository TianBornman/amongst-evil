using Midevil.Camera.States;
using Unity.Cinemachine;
using UnityEngine;

namespace Midevil.Camera
{
	public class CameraMovement : StateMachine
	{
		// Editor Variables
		[Header("References")]
		public CinemachineCamera partyCamera;
		public CinemachineCamera mapCamera;
		public CinemachineCamera battleCamera;

		// Public Variables
		public Transform partyCenter;
		public CinemachineInputAxisController axisController;

		private void Awake()
		{
			axisController = FindFirstObjectByType<CinemachineInputAxisController>();
		}

		private void Start()
		{
			partyCenter = PartyManager.Instance.Center;
			SetState(new ExploreState(this));
		}

		public void SetBattleState()
		{
			SetState(new BattleState(this));
		}

		public void SetExploreState()
		{
			SetState(new ExploreState(this));
		}
	}
}