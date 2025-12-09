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
	public UIDocument armouryUiPrefab;

	private UIDocument mainMenuUi;
	private UIDocument recruitmentUi;
	private UIDocument bloodVaultUi;
	private UIDocument armouryUi;

	// Override Methods
	protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (!GameManager.Instance.AtHub)
			return;

		// Spawn UIs
		mainMenuUi = Instantiate(mainMenuUiPrefab).GetComponent<UIDocument>();
		recruitmentUi = Instantiate(recruitmentUiPrefab).GetComponent<UIDocument>();
		bloodVaultUi = Instantiate(bloodVaultUiPrefab).GetComponent<UIDocument>();
		armouryUi = Instantiate(armouryUiPrefab).GetComponent<UIDocument>();

		// Config
		mainMenuUi.rootVisualElement.visible = true;
		recruitmentUi.rootVisualElement.visible = false;
		bloodVaultUi.rootVisualElement.visible = false;
		armouryUi.rootVisualElement.visible = false;

		mainMenuUi.rootVisualElement.Q<Button>("Play").clicked += HubManager.Instance.StartGame;
	}

	// Public Methods
	public void ShowRecruitmentUI()
	{
		mainMenuUi.rootVisualElement.visible = false;
		recruitmentUi.rootVisualElement.visible = true;
	}

	public void UpdateRecruitmentUI()
	{
		var recruitmentElements = recruitmentUi.rootVisualElement.Q<VisualElement>("Profiles")
																 .Query<VisualElement>("Profile").ToList();
		var identities = PartyManager.Instance.partyIdentities;

		for (int i = 0; i < identities.Count; i++)
		{
			var iconTexture = RefManager.Instance.GetIcon(identities[i].profileIcon);
			recruitmentElements[i].Q<VisualElement>("Image").style.backgroundImage = new StyleBackground(iconTexture);
			recruitmentElements[i].Q<Label>().text = identities[i].characterName;
		}
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

	public void ShowArmouryUI()
	{
		var itemElements = armouryUi.rootVisualElement.Q<VisualElement>("unity-content-container").Query<ItemElement>().ToList();
		var items = InventoryManager.Instance.armouryInventory;

		for (int i = 0; i < itemElements.Count; i++)
		{
			var itemElement = itemElements[i];

			if (i < items.Count)
				itemElement.SetItem(items[i]);
			else
				itemElement.ClearItem();
		}

		armouryUi.rootVisualElement.visible = true;
	}

	public void HideArmouryUI()
	{
		armouryUi.rootVisualElement.visible = false;
	}
}
