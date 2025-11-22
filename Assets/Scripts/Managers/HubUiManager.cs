using Midevil.Models;
using Midevil.UI.Elements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class HubUiManager : Singleton<HubUiManager>
{
	// Editor Variables
	[Header("References")]
	public UIDocument mainMenuUiPrefab;
	public UIDocument recruitmentUIPrefab;

	[HideInInspector] public UIDocument mainMenuUi;
	[HideInInspector] public UIDocument recruitmentUI;

	// Override Methods
	protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (scene.buildIndex != 0)
			return;

		// Spawn UIs
		mainMenuUi = Instantiate(mainMenuUiPrefab).GetComponent<UIDocument>();
		recruitmentUI = Instantiate(recruitmentUIPrefab).GetComponent<UIDocument>();

		// Config
		mainMenuUi.rootVisualElement.visible = true;
		recruitmentUI.rootVisualElement.visible = false;

		mainMenuUi.rootVisualElement.Q<Button>("Play").clicked += HubManager.Instance.StartGame;
	}

	// Public Methods
	public void ShowRecruitmentUI()
	{
		mainMenuUi.rootVisualElement.visible = false;
		recruitmentUI.rootVisualElement.visible = true;
	}

	public void UpdateRecruitmentUI(Identity identity)
	{
		var recruitmentElement = recruitmentUI.rootVisualElement.Q<VisualElement>("RecuitmentProfile");
		recruitmentElement.Q<Label>().text = identity.name;
	}
}
