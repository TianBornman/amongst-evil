public class Tent : Building
{
	public override void Interact()
	{
		HubUiManager.Instance.ShowSpiralUI();
	}
}
