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
	private VisualElement recruitmentArmouryUi;
	private VisualElement recruitmentGearUi;
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
		recruitmentArmouryUi = recruitmentUi.rootVisualElement.Q<TemplateContainer>("Armoury");
		recruitmentGearUi = recruitmentUi.rootVisualElement.Q<TemplateContainer>("Gear");
		bloodVaultUi = Instantiate(bloodVaultUiPrefab).GetComponent<UIDocument>();
		armouryUi = Instantiate(armouryUiPrefab).GetComponent<UIDocument>();

		// Config
		mainMenuUi.rootVisualElement.visible = true;
		recruitmentUi.rootVisualElement.visible = false;
		recruitmentArmouryUi.visible = false;
		recruitmentGearUi.visible = false;
		bloodVaultUi.rootVisualElement.visible = false;
		armouryUi.rootVisualElement.visible = false;

		mainMenuUi.rootVisualElement.Q<Button>("Play").clicked += HubManager.Instance.StartGame;

		var gearPanel = recruitmentGearUi.Q<VisualElement>("CharacterPanel");
		var statsPanel = recruitmentGearUi.Q<VisualElement>("Stats");

		recruitmentGearUi.Q<VisualElement>("ShowStats").RegisterCallback<ClickEvent>(evt =>
		{
			gearPanel.AddToClassList("hidden");
			statsPanel.RemoveFromClassList("hidden");
		});

		recruitmentGearUi.Q<VisualElement>("HideStats").RegisterCallback<ClickEvent>(evt =>
		{
			statsPanel.AddToClassList("hidden");
			gearPanel.RemoveFromClassList("hidden");
		});
	}

	// Public Methods
	public void ShowRecruitmentUI()
	{
		mainMenuUi.rootVisualElement.visible = false;
		recruitmentUi.rootVisualElement.visible = true;
	}

	public void UpdateRecruitmentUI(Character character)
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

		var armoury = recruitmentUi.rootVisualElement.Q<TemplateContainer>("Armoury");
		ShowArmouryUI(armoury);

		ShowRecruitGearUI(character);
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

	public void ShowArmouryUI(VisualElement rootElement)
	{
		var itemElements = rootElement.Q<VisualElement>("unity-content-container").Query<ItemElement>().ToList();
		var items = InventoryManager.Instance.armouryInventory;

		for (int i = 0; i < itemElements.Count; i++)
		{
			var itemElement = itemElements[i];

			if (i < items.Count)
				itemElement.SetItem(items[i]);
			else
				itemElement.ClearItem();
		}

		rootElement.visible = true;
	}

	public void HideArmouryUI()
	{
		armouryUi.rootVisualElement.visible = false;
	}

	public void HideRecruitArmouryUI()
	{
		recruitmentArmouryUi.visible = false;
	}

	public void ShowRecruitGearUI(Character character)
	{
		recruitmentGearUi.dataSource = character;

		foreach (var item in recruitmentGearUi.Query<ItemElement>().ToList())
			item.CharacterId = character.identity.id;

		recruitmentGearUi.visible = true;
	}

	public void HideRecruitGearUI()
	{
		recruitmentGearUi.visible = false;
	}
}
