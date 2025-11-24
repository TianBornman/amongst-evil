using UnityEngine;
using UnityEngine.SceneManagement;

public class Cart : MonoBehaviour, IInteractable
{
	#region Interactable

	public void Interact()
	{
		HubManager.Instance.StartRun();
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
