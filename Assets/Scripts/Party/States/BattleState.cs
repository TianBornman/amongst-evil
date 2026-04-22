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
		}

		public void Exit()
		{
		}

		public void Update()
		{
		}
	}
}