using UnityEngine;

public class BloodVault : MonoBehaviour, IInteractable
{
	#region Interactable

	public void Interact()
	{
		HubUiManager.Instance.ShowBloodVaultUI();
	}

	public void OnHoverEnter()
	{
		outline.enabled = true;
	}

	public void OnHoverExit()
	{
		outline.enabled = false;
	}

	#endregion

	// Private Variables
	private Outline outline;

	// Private Methods
	private void Awake()
	{
		outline = GetComponent<Outline>();
		outline.enabled = false;
	}
}
