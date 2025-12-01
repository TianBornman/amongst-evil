public class ExploreState : IState
{
	private Party party;

	public ExploreState(Party party)
	{
		this.party = party;
	}

	public void Enter()
	{
		party.cameraMovement.Explore();
	}

	public void Exit()
	{
	}

	public void Update()
	{
	}
}
