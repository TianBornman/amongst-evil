using Midevil.Models;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class HubManager : Singleton<HubManager>
{
	// Private Variables
	private MainMenuCamera mainMenuCamera;

	// Override Methods
	protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (scene.buildIndex == 0)
			GameManager.Instance.EnterHub();

		if (!GameManager.Instance.AtHub)
			return;

		mainMenuCamera = FindFirstObjectByType<MainMenuCamera>();

		if (!GameManager.Instance.AtMenu)
			StartGame();
	}

	// Public Methods
	public void StartGame()
	{
		HubUiManager.Instance.ShowRecruitmentUI();
		GameManager.Instance.LeaveMenu();

		mainMenuCamera.targetPosition = mainMenuCamera.hubPosition;
	}

	public void StartRun()
	{
		if (PartyManager.Instance.partyIdentities.Count == 0)
			return;

		PartyManager.Instance.StartRun();

		GameManager.Instance.LeaveHub();
		SceneManager.LoadScene("Level");
	}

	public void FocusCharacter(RecruitCharacter recruitCharacter)
	{
		PartyManager.Instance.RecruitPartyMember(recruitCharacter.identity);
		HubUiManager.Instance.UpdateRecruitmentUI(recruitCharacter);

		ZoomOnTransform(recruitCharacter.cameraPos);
	}

	// Private Methods
	private void Start()
	{
		GameManager.Instance.EnterHub();
		InputManager.Instance.EscapeAction = Escape;
	}

	private void Escape()
	{
		if (!GameManager.Instance.AtHub)
			return;

		mainMenuCamera.targetPosition = mainMenuCamera.hubPosition;
		HubUiManager.Instance.HideBloodVaultUI();
		HubUiManager.Instance.HideRecruitArmouryUI();
		HubUiManager.Instance.HideRecruitGearUI();
	}

	private void ZoomOnTransform(Transform transform)
	{
		mainMenuCamera.targetPosition = transform;
	}
}
