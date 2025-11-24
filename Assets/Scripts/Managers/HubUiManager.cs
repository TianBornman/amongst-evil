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
	public UIDocument recruitmentUiPrefab;
	public UIDocument bloodVaultUiPrefab;

	[HideInInspector] public UIDocument mainMenuUi;
	[HideInInspector] public UIDocument recruitmentUi;
	[HideInInspector] public UIDocument bloodVaultUi;

	// Override Methods
	protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (!GameManager.Instance.AtHub)
			return;

		// Spawn UIs
		mainMenuUi = Instantiate(mainMenuUiPrefab).GetComponent<UIDocument>();
		recruitmentUi = Instantiate(recruitmentUiPrefab).GetComponent<UIDocument>();
		bloodVaultUi = Instantiate(bloodVaultUiPrefab).GetComponent<UIDocument>();

		// Config
		mainMenuUi.rootVisualElement.visible = true;
		recruitmentUi.rootVisualElement.visible = false;
		bloodVaultUi.rootVisualElement.visible = false;

		mainMenuUi.rootVisualElement.Q<Button>("Play").clicked += HubManager.Instance.StartGame;
	}

	// Public Methods
	public void ShowRecruitmentUI()
	{
		mainMenuUi.rootVisualElement.visible = false;
		recruitmentUi.rootVisualElement.visible = true;
	}

	public void UpdateRecruitmentUI(Identity identity)
	{
		var recruitmentElement = recruitmentUi.rootVisualElement.Q<VisualElement>("RecuitmentProfile");
		recruitmentElement.Q<Label>().text = identity.characterName;
	}

	public void ShowBloodVaultUI()
	{
		var data = BloodvaultManager.Load();
		var list = bloodVaultUi.rootVisualElement.Q<ScrollView>("List");

		list.Clear();

		foreach (var entry in data.entries)
		{
			var item = new BloodVaultEntryElement();
			item.SetEntry(entry);
			list.Add(item);
		}

		bloodVaultUi.rootVisualElement.visible = true;
	}

	public void HideBloodVaultUI()
	{
		bloodVaultUi.rootVisualElement.visible = false;
	}
}
