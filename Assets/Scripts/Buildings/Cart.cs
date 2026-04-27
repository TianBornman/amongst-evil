public class Cart : Building
{
	// Override Methods
	public override void Interact()
	{
		HubUiManager.Instance.ShowMissionBoardUI();
	}
}
