namespace Midevil.Party.States
{
	internal class BattleState : IState
	{
		private Party party;

		public BattleState(Party party)
		{
			this.party = party;
		}

		public void Enter()
		{
			party.cameraMovement.SetBattleState();
		}

		public void Exit()
		{
		}

		public void Update()
		{
		}
	}
}