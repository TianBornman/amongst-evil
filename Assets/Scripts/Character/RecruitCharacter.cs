using Midevil.Models;

public class RecruitCharacter : Character
{
	#region Interactable
	public override void Interact()
	{
		base.Interact();

		HubUiManager.Instance.UpdateRecruitmentUI(identity);
		RecruitManager.Instance.playerIdentity = identity;
	}

	#endregion

	// Public Variables
	public Identity identity;
}