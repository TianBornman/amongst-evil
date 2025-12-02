using Midevil.Camera.States;
using UnityEngine;

namespace Midevil.Camera
{
	public class CameraMovement : StateMachine
	{
		public Transform partyCenter;

		public CameraSettings exploreSettings;
		public CameraSettings combatSettings;

		[HideInInspector] public CameraSettings active;
		[HideInInspector] public CameraSettings targetSettings;

		public Transform followTarget;

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