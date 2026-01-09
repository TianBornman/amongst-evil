namespace Midevil.Party.States
{
	internal class BattleState : IState
	{
		public bool CanExit { get; private set; } = true;

		private Party party;

		public BattleState(Party party)
		{
			this.party = party;
		}

		public void Enter()
		{
			party.cameraMovement.SetBattleState();

			foreach (var member in party.members)
			{
				var lane = BattleManager.Instance.GetStartingLane(member);
				member.EnterCombat(lane);
			}
		}

		public void Exit()
		{
		}

		public void Update()
		{
		}
	}
}