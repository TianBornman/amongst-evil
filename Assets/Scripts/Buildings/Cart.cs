public class Cart : Building
{
	// Override Methods
	public override void Interact()
	{
		HubManager.Instance.StartRun();
	}
}
