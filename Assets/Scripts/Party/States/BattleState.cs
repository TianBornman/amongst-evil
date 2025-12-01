public class BattleState : IState
{
	private Party party;

	public BattleState(Party party)
	{
		this.party = party;
	}

	public void Enter()
	{
		party.cameraMovement.Battle();
		party.cameraMovement.partyView.position = party.GetGroupCenter();
	}

	public void Exit()
	{
	}

	public void Update()
	{
	}
}
