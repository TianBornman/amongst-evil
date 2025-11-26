using Midevil.Models;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class HubManager : Singleton<HubManager>
{
	#region Input

	private InputSystem_Actions inputActions;
	private InputAction escapeAction;

	protected override void OnEnable()
	{
		if (Instance != this)
			return;

		base.OnEnable();

		inputActions = new InputSystem_Actions();
		escapeAction = inputActions.Player.Escape;

		inputActions.Enable();
		escapeAction.performed += OnEscape;
	}

	protected override void OnDisable()
	{
		if (Instance != this)
			return;

		base.OnDisable();

		escapeAction.performed -= OnEscape;
		inputActions.Disable();
	}

	private void OnEscape(InputAction.CallbackContext context)
	{
		if (!GameManager.Instance.AtHub)
			return;

		mainMenuCamera.targetPosition = mainMenuCamera.hubPosition;
		HubUiManager.Instance.HideBloodVaultUI();
	}

	#endregion

	// Private Variables
	private MainMenuCamera mainMenuCamera;

	// Override Methods
	protected override void Awake()
	{
		GameManager.Instance.EnterHub();

		base.Awake();
	}

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
		HubUiManager.Instance.UpdateRecruitmentUI();

		ZoomOnTransform(recruitCharacter.cameraPos);
	}

	// Private Methods
	private void ZoomOnTransform(Transform transform)
	{
		mainMenuCamera.targetPosition = transform;
	}
}
