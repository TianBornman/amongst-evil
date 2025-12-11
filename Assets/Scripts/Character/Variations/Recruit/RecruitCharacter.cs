using UnityEngine;

public class RecruitCharacter : Character
{
	#region Interactable
	public override void Interact()
	{
		base.Interact();

		HubManager.Instance.FocusCharacter(this);
	}

	#endregion

	// Editor Variables
	[Header("Recruit Character")]
	public Transform cameraPos;
}