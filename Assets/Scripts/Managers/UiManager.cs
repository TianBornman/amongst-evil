using Midevil.Ability;
using Midevil.Effect;
using Midevil.Item;
using Midevil.UI.Elements;
using Midevil.UpgradeCard;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UiManager : Singleton<UiManager>
{
	// Editor Variables
	[Header("References")]
	public UIDocument gameUiPrefab;
	public UIDocument statsUiPrefab;
	public UIDocument levelUpUiPrefab;
	public UIDocument itemPickupUiPrefab;
	public UIDocument resultsUiPrefab;

	[HideInInspector] public UIDocument gameUi;
	[HideInInspector] public UIDocument statsUi;
	[HideInInspector] public UIDocument levelUpUi;
	[HideInInspector] public UIDocument itemPickupUi;
	[HideInInspector] public UIDocument resultsUi;

	// Private Variables
	private List<ClickableElement> upgradeCards = new();
	private bool canToggleMenu = true;

	// Override Methods
	protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (GameManager.Instance.AtHub)
			return;

		// Spawn UIs
		gameUi = Instantiate(gameUiPrefab).GetComponent<UIDocument>();
		statsUi = Instantiate(statsUiPrefab).GetComponent<UIDocument>();
		levelUpUi = Instantiate(levelUpUiPrefab).GetComponent<UIDocument>();
		itemPickupUi = Instantiate(itemPickupUiPrefab).GetComponent<UIDocument>();
		resultsUi = Instantiate(resultsUiPrefab).GetComponent<UIDocument>();

		// Config
		statsUi.rootVisualElement.visible = false;
		levelUpUi.rootVisualElement.visible = false;
		upgradeCards = levelUpUi.rootVisualElement.Q<VisualElement>("UpgradeCards").Query<ClickableElement>().ToList();
		itemPickupUi.rootVisualElement.visible = false;

		// Setup character panels
		var characterPanels = statsUi.rootVisualElement.Q<VisualElement>("CharacterPanels")
													   .Query<VisualElement>("CharacterPanel").ToList();
		var characterStats = statsUi.rootVisualElement.Q<VisualElement>("CharacterPanels")
													  .Query<VisualElement>("Stats").ToList();
		var characterMiniDashboards = gameUi.rootVisualElement.Q<VisualElement>("MiniDashboards")
														   .Query<TemplateContainer>().ToList();

		var identities = PartyManager.Instance.partyIdentities;

		for (int i = 0; i < identities.Count; i++)
		{
			var panel = characterPanels[i];
			var stats = characterStats[i];
			var miniDashboard = characterMiniDashboards[i];
			var identity = identities[i];

			panel.name = identity.id.ToString();

			panel.Q<VisualElement>("ShowStats").RegisterCallback<ClickEvent>(evt =>
			{
				panel.AddToClassList("hidden");
				stats.RemoveFromClassList("hidden");
			});

			stats.name = $"{identity.id}-stats";

			stats.Q<VisualElement>("HideStats").RegisterCallback<ClickEvent>(evt =>
			{
				stats.AddToClassList("hidden");
				panel.RemoveFromClassList("hidden");
			});

			miniDashboard.name = identity.id.ToString();

			foreach (var item in panel.Query<ItemElement>().ToList())
				item.CharacterId = identity.id;
		}

		// Results
		resultsUi.rootVisualElement.visible = false;
		resultsUi.rootVisualElement.Q<VisualElement>("Results").dataSource = PartyManager.Instance.partyResults;
		resultsUi.rootVisualElement.Q<Button>("Flee").clicked += Flee;
		resultsUi.rootVisualElement.Q<Button>("FightOn").clicked += SpawnWave;

		var continueButton = resultsUi.rootVisualElement.Q<Button>("Continue");
		continueButton.clicked += Die;
		continueButton.visible = false;
	}

	// Public Methods
	public void BindPartyMemberStats(PartyCharacter character)
	{
		//gameUi.rootVisualElement.Q<ProgressBar>("PlayerHealth").dataSource = player;
		//gameUi.rootVisualElement.Q<Label>("Level").dataSource = player;
		//gameUi.rootVisualElement.Q<ProgressBar>("XpBar").dataSource = player;

		statsUi.rootVisualElement.Q<VisualElement>($"{character.identity.id}").dataSource = character;
		statsUi.rootVisualElement.Q<VisualElement>($"{character.identity.id}-stats").dataSource = character;

		var miniDashboard = gameUi.rootVisualElement.Q<TemplateContainer>($"{character.identity.id}");
        miniDashboard.dataSource = character;
		miniDashboard.style.unityBackgroundImageTintColor = character.color;

		foreach (var abilityElement in miniDashboard.Query<AbilityElement>().ToList())
            abilityElement.style.unityBackgroundImageTintColor = character.color;
		
        var bars = statsUi.rootVisualElement.Q<VisualElement>(character.identity.id.ToString()).Q<VisualElement>("Bars");
		bars.dataSource = character;
	}

	//public void BindEnemyStats(Character character)
	//{
	//	gameUi.rootVisualElement.Q<ProgressBar>("EnemyHealth").dataSource = character;
	//}

	public void BindUpgradeCard(int index, UpgradeCard card, Action<UpgradeCard> onClick)
	{
		if (index < 0 || index >= upgradeCards.Count)
			return;

		upgradeCards[index].dataSource = card;
		upgradeCards[index].SetClickHandler(evt => onClick(card));
	}

	public void BindAbility(int index, Ability ability)
	{
		var abilityElement = gameUi.rootVisualElement.Query<AbilityElement>().ToList();

        if (index < 0 || index >= abilityElement.Count)
            return;

        abilityElement[index].dataSource = ability;

		//var borderColor = new StyleColor(((PartyCharacter)ability.owner).color);
		//abilityElement[index].style.borderTopColor = borderColor;
		//abilityElement[index].style.borderRightColor = borderColor;
		//abilityElement[index].style.borderLeftColor = borderColor;
		//abilityElement[index].style.borderBottomColor = borderColor;
	}

	public void ClearAbility(int index)
	{
        var abilityElement = gameUi.rootVisualElement.Query<AbilityElement>().ToList();

        if (index < 0 || index >= abilityElement.Count)
			return;

		abilityElement[index].dataSource = null;
		abilityElement[index].ClearAbility();
	}

	public void AddEffect(Effect effect)
	{
		var effectsElement = gameUi.rootVisualElement.Q<VisualElement>("ActiveEffects");

		var newEffect = new EffectElement();
		newEffect.name = effect.id.ToString();
		newEffect.SetEffect(effect);

		effectsElement.Add(newEffect);
	}

	public void RemoveEffect(Effect effect)
	{
		var effectElement = gameUi.rootVisualElement.Q<VisualElement>("ActiveEffects").Q<EffectElement>(effect.id.ToString());
		gameUi.rootVisualElement.Q<VisualElement>("ActiveEffects").Remove(effectElement);
	}

	public void ShowLevelUp()
	{
		levelUpUi.rootVisualElement.visible = true;
		Pause();
	}

	public void HideLevelUp()
	{
		levelUpUi.rootVisualElement.visible = false;
		Resume();
	}

	public void UpdateInventory()
	{
		var itemElements = statsUi.rootVisualElement.Q<VisualElement>("Items").Query<ItemElement>().ToList();
		var items = InventoryManager.Instance.runInventory;

		for (int i = 0; i < itemElements.Count; i++)
		{
			var itemElement = itemElements[i];

			if (i < items.Count)
				itemElement.SetItem(items[i]);
			else
				itemElement.ClearItem();
		}
	}

	public void UpdateCharacterPanels()
	{
		var identities = PartyManager.Instance.partyIdentities;

		for (int i = 0; i < identities.Count; i++)
		{
			var identity = identities[i];
			var panel = statsUi.rootVisualElement.Q<VisualElement>(identity.id.ToString());

			if (identity.weapon != null)
				EquipItem(panel, identity.weapon);
			else
				UnequipItem(panel, ItemType.Weapon);

			if (identity.armour != null)
				EquipItem(panel, identity.armour);
			else
				UnequipItem(panel, ItemType.Armour);
		}
	}

	public void EquipItem(VisualElement panel, ItemStats item)
	{
		var equipSlot = panel.Q<ItemElement>(item.type.ToString());
		equipSlot.SetItem(item);
	}

	public void UnequipItem(VisualElement panel, ItemType type)
	{
		var equipSlot = panel.Q<ItemElement>(type.ToString());
		equipSlot?.ClearItem();
	}

	public void ShowResults()
	{
		resultsUi.rootVisualElement.visible = true;
	}

	public void ShowDeathScreen()
	{
		ShowResults();

		resultsUi.rootVisualElement.Q<Button>("Continue").visible = true;
		resultsUi.rootVisualElement.Q<Button>("Flee").visible = false;
		resultsUi.rootVisualElement.Q<Button>("FightOn").visible = false;
	}

	// Private Methods
	private void Start()
	{
		InputManager.Instance.MenuToggleAction = MenuToggle;
	}

	private void MenuToggle()
	{
		if (GameManager.Instance.AtHub)
			return;

		if (!canToggleMenu) 
			return;

		statsUi.rootVisualElement.visible = !statsUi.rootVisualElement.visible;

		if (statsUi.rootVisualElement.visible)
			Time.timeScale = 0;
		else
			Time.timeScale = 1;
	}

	private void Pause()
	{
		GameManager.Instance.PauseGame();
		canToggleMenu = false;
	}

	private void Resume()
	{
		GameManager.Instance.ResumeGame();
		canToggleMenu = true;
	}

	private void Flee()
	{
		Resume();
		GameManager.Instance.EnterHub();
		PartyManager.Instance.EndRun();
		SceneManager.LoadScene("Sect");
	}

	private void Die()
	{
		Resume();
		GameManager.Instance.EnterHub();
		PartyManager.Instance.EndRun();
		SceneManager.LoadScene("Sect");
	}

	private void SpawnWave()
	{
		resultsUi.rootVisualElement.visible = false;
		SpawnManager.Instance.SpawnWave();
		//PartyManager.Instance.SetPartyTarget(SpawnManager.Instance.currentSegment.connectionPoint.position);
		//PartyManager.Instance.UpdateExploreView(SpawnManager.Instance.currentSegment.transform.position);
	}
}
