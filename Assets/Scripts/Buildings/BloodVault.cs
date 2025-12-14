public class BloodVault : Building
{
	// Override Methods
	public override void Interact()
	{
		HubUiManager.Instance.ShowBloodVaultUI();
	}
}
