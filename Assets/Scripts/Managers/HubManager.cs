using Midevil.Item;
using Midevil.Mission;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubManager : Singleton<HubManager>
{
	// Public Variables
	[HideInInspector] public Character currentCharacter;

	// Private Variables
	private CinemachineCamera currentCamera;

	// Override Methods
	protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (scene.buildIndex == 0)
			GameManager.Instance.EnterHub();

		if (!GameManager.Instance.AtHub)
			return;

		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		if (!GameManager.Instance.AtMainMenu)
			StartGame();
	}

	// Public Methods
	public void StartGame()
	{
		HubUiManager.Instance.ShowRecruitmentUI();
		GameManager.Instance.LeaveMenu();
	}

	public void StartRun(Mission mission)
	{
		if (PartyManager.Instance.partyIdentities.Count == 0)
			return;

		PartyManager.Instance.StartRun(mission);

		GameManager.Instance.LeaveHub();
		SceneManager.LoadScene("Level");
	}

	public void FocusCharacter(RecruitCharacter recruitCharacter)
	{
		currentCharacter = recruitCharacter;

		PartyManager.Instance.RecruitPartyMember(recruitCharacter.identity);
		HubUiManager.Instance.UpdateRecruitmentUI(recruitCharacter);

		ZoomOnTransform(recruitCharacter.cam);
	}

	public void EquipItem(ItemStats item)
	{
		if (currentCharacter == null)
			return;

		currentCharacter.equipment.EquipItem(item);
	}

	public void UnequipItem(ItemStats item)
	{
		if (currentCharacter == null)
			return;

		currentCharacter.equipment.UnequipItem(item);
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

		if (currentCamera != null)
			currentCamera.enabled = false;

		currentCharacter = null;

		if (HubUiManager.Instance.MissionBoardOpen)
			HubUiManager.Instance.HideMissionBoardUI();
		else if (HubUiManager.Instance.BloodVaultOpen)
			HubUiManager.Instance.HideBloodVaultUI();
		else if (HubUiManager.Instance.ArmouryOpen)
		{
			HubUiManager.Instance.HideRecruitArmouryUI();
			HubUiManager.Instance.HideRecruitGearUI();
		}
		else if (HubUiManager.Instance.SettingsOpen)
			HubUiManager.Instance.HideSettingsUI();
		else
			HubUiManager.Instance.ShowSettingsUI();
	}

	private void ZoomOnTransform(CinemachineCamera camera)
	{
		if (currentCamera != null) 
			currentCamera.enabled = false;

		camera.enabled = true;
		currentCamera = camera;
	}
}
