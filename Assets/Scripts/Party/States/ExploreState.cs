namespace Midevil.Party.States
{
	internal class ExploreState : IState
	{
		public bool CanExit { get; private set; } = true;

		private Party party;

		public ExploreState(Party party)
		{
			this.party = party;
		}

		public void Enter()
		{
			party.cameraMovement.SetExploreState();
		}

		public void Exit()
		{
		}

		public void Update()
		{
		}
	}
}