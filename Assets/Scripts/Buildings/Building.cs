using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour, IInteractable
{
	#region Interactable

	public virtual void Interact() { }

	public void OnHoverEnter()
	{
		outline.enabled = true;
	}

	public void OnHoverExit()
	{
		outline.enabled = false;
	}

	#endregion

	// Editor Variables
	[Header("References")]
	public List<Transform> idlePositions;

	// Private Variables
	private Outline outline;

	// Private Methods
	private void Awake()
	{
		outline = GetComponent<Outline>();
		outline.enabled = false;
	}
}