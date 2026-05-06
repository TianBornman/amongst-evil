using Midevil.Ability;
using Midevil.Boons;
using Midevil.Effect;
using Midevil.Item;
using Midevil.UI.Elements;
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
		var characterContainers = statsUi.rootVisualElement.Q<VisualElement>("CharacterPanels")
															.Query<TemplateContainer>().ToList();
		var characterPanels = statsUi.rootVisualElement.Q<VisualElement>("CharacterPanels")
													   .Query<VisualElement>("GearPanel").ToList();
		var characterStats = statsUi.rootVisualElement.Q<VisualElement>("CharacterPanels")
													  .Query<VisualElement>("StatsPanel").ToList();
		var characterMiniDashboards = gameUi.rootVisualElement.Q<VisualElement>("MiniDashboards")
														   .Query<TemplateContainer>().ToList();

		var identities = PartyManager.Instance.partyIdentities;

		for (int i = 0; i < identities.Count; i++)
		{
			var container = characterContainers[i];
			var panel = characterPanels[i];
			var stats = characterStats[i];
			var miniDashboard = characterMiniDashboards[i];
			var identity = identities[i];

			CharacterGearPanel.WireTabs(container);

			panel.name = identity.id.ToString();
			stats.name = $"{identity.id}-stats";

			miniDashboard.name = identity.id.ToString();

			foreach (var item in panel.Query<ItemElement>().ToList())
				item.CharacterId = identity.id;
		}

		// Results
		resultsUi.rootVisualElement.visible = false;

		var continueButton = resultsUi.rootVisualElement.Q<Button>("Continue");
		continueButton.clicked += ShowAftermath;
		continueButton.visible = false;

		var fleeButton = resultsUi.rootVisualElement.Q<Button>("Flee");
		fleeButton.clicked += ShowAftermath;

		var returnButton = resultsUi.rootVisualElement.Q<Button>("Return");
		if (returnButton != null)
			returnButton.clicked += ReturnToSect;

		// Game UI
		gameUi.rootVisualElement.Q<Label>("Map").text = GameManager.Instance.MapName;
		SetEnemiesText(0, 0);
		SetWaveText(0, SpawnManager.Instance.totalWaves);
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

	public void ShowBoonPicker(List<BoonOffer> offers, Action<BoonOffer> onPick)
	{
		if (levelUpUi == null || levelUpUi.rootVisualElement == null) return;
		if (offers == null || offers.Count == 0) return;

		for (int i = 0; i < upgradeCards.Count; i++)
		{
			var card = upgradeCards[i];
			if (i < offers.Count)
			{
				card.style.display = DisplayStyle.Flex;
				BindBoonCard(card, offers[i], onPick);
			}
			else
			{
				card.style.display = DisplayStyle.None;
				card.UnsetClickHandler();
			}
		}

		levelUpUi.rootVisualElement.visible = true;
		Pause();
	}

	public void HideBoonPicker()
	{
		if (levelUpUi == null || levelUpUi.rootVisualElement == null) return;
		levelUpUi.rootVisualElement.visible = false;
		foreach (var card in upgradeCards)
			card.UnsetClickHandler();
		Resume();
	}

	private void BindBoonCard(ClickableElement card, BoonOffer offer, Action<BoonOffer> onPick)
	{
		// Bypass any leftover UpgradeCard data bindings on this UI document.
		card.dataSource = null;

		var title = card.Q<Label>("Title");
		if (title != null) title.text = offer.card.cardName;

		var description = card.Q<Label>("Description");
		if (description != null) description.text = offer.card.description;

		var stats = card.Q<Label>("Stats");
		if (stats != null) stats.text = FormatBoonFooter(offer);

		// Rarity tint via class list.
		card.RemoveFromClassList("rarity-common");
		card.RemoveFromClassList("rarity-refined");
		card.RemoveFromClassList("rarity-rare");
		card.AddToClassList("rarity-" + offer.card.rarity.ToString().ToLowerInvariant());

		card.SetClickHandler(_ => onPick(offer));
	}

	private static string FormatBoonFooter(BoonOffer offer)
	{
		var rarityTag = offer.card.rarity.ToString();
		var categoryTag = offer.card.category.ToString();

		string recipientLine;
		if (offer.card.targeting == BoonTargeting.Creed)
		{
			recipientLine = "→ The Creed";
		}
		else if (offer.recipient == null || offer.recipient.identity == null)
		{
			recipientLine = "→ —";
		}
		else
		{
			var className = offer.recipient.identity.brotherClass.ToString();
			recipientLine = $"→ {offer.recipient.identity.characterName} ({className})";
		}

		return $"{rarityTag} {categoryTag}\n{recipientLine}";
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


	public void UpdateInventory()
	{
		if (statsUi == null || statsUi.rootVisualElement == null)
			return;

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
		PopulateResults();
		resultsUi.rootVisualElement.visible = true;

		var resultsPage = resultsUi.rootVisualElement.Q<VisualElement>("ResultsPage");
		var aftermathPage = resultsUi.rootVisualElement.Q<VisualElement>("AftermathPage");
		if (resultsPage != null) resultsPage.RemoveFromClassList("hidden");
		if (aftermathPage != null) aftermathPage.AddToClassList("hidden");

		ReleaseCursorAndCamera();
	}

	private void ShowAftermath()
	{
		PopulateAftermath();

		var resultsPage = resultsUi.rootVisualElement.Q<VisualElement>("ResultsPage");
		var aftermathPage = resultsUi.rootVisualElement.Q<VisualElement>("AftermathPage");
		if (resultsPage != null) resultsPage.AddToClassList("hidden");
		if (aftermathPage != null) aftermathPage.RemoveFromClassList("hidden");
	}

	private void PopulateAftermath()
	{
		var root = resultsUi.rootVisualElement.Q<VisualElement>("AftermathPage");
		if (root == null) return;

		var aftermath = PartyManager.Instance.lastAftermath;
		if (aftermath == null) return;

		root.Q<Label>("AftermathFlavor").text = aftermath.success
			? "The deed is done. The Spiral takes note."
			: "Blood for the Vault. The Veil is colder for it.";

		root.Q<Label>("StandingBefore").text = aftermath.standingBefore.ToString();
		root.Q<Label>("StandingAfter").text = aftermath.standingAfter.ToString();

		var delta = root.Q<Label>("StandingDelta");
		delta.RemoveFromClassList("gain");
		delta.RemoveFromClassList("loss");
		if (aftermath.standingDelta > 0)
		{
			delta.text = $"+{aftermath.standingDelta}";
			delta.AddToClassList("gain");
		}
		else if (aftermath.standingDelta < 0)
		{
			delta.text = aftermath.standingDelta.ToString();
			delta.AddToClassList("loss");
		}
		else
		{
			delta.text = "±0";
		}

		root.Q<Label>("RankName").text = string.IsNullOrEmpty(aftermath.rankAfterName)
			? "—"
			: aftermath.rankAfterName;

		var ascension = root.Q<Label>("AscensionBanner");
		ascension.RemoveFromClassList("hidden");
		if (aftermath.ascended)
		{
			ascension.text = $"You ascend the Spiral. {aftermath.rankBeforeName} → {aftermath.rankAfterName}";
		}
		else if (aftermath.ascensionPending && !string.IsNullOrEmpty(aftermath.nextRankName))
		{
			ascension.text = $"The Spiral beckons — ascension to {aftermath.nextRankName} awaits at the tent.";
		}
		else
		{
			ascension.AddToClassList("hidden");
		}

		var fallenList = root.Q<VisualElement>("FallenList");
		var survivorsList = root.Q<VisualElement>("SurvivorsList");
		fallenList.Clear();
		survivorsList.Clear();

		var fallenIntro = root.Q<Label>("FallenIntro");
		if (aftermath.fallen.Count > 0)
		{
			fallenIntro.text = aftermath.fallen.Count == 1
				? "Bound to the Vault — their name endures:"
				: "Bound to the Vault — their names endure:";
			foreach (var name in aftermath.fallen)
			{
				var label = new Label(name);
				label.AddToClassList("vault-entry");
				label.AddToClassList("fallen");
				fallenList.Add(label);
			}
		}
		else
		{
			fallenIntro.text = "None were claimed. The Vault stays silent.";
		}

		var survivorsIntro = root.Q<Label>("SurvivorsIntro");
		if (aftermath.survivors.Count > 0)
		{
			survivorsIntro.text = "Returned to the Sect:";
			foreach (var name in aftermath.survivors)
			{
				var label = new Label(name);
				label.AddToClassList("vault-entry");
				label.AddToClassList("survivor");
				survivorsList.Add(label);
			}
		}
		else
		{
			survivorsIntro.text = string.Empty;
		}
	}

	private void ReturnToSect()
	{
		Resume();
		GameManager.Instance.EnterHub();
		PartyManager.Instance.EndRun();
		SceneManager.LoadScene("Sect");
	}

	private void ReleaseCursorAndCamera()
	{
		UnityEngine.Cursor.lockState = CursorLockMode.None;
		UnityEngine.Cursor.visible = true;

		var cameraMovement = FindFirstObjectByType<Midevil.Camera.CameraMovement>();
		if (cameraMovement != null && cameraMovement.axisController != null)
			cameraMovement.axisController.enabled = false;
	}

	public void ShowDeathScreen()
	{
		ShowResults();

		resultsUi.rootVisualElement.Q<Button>("Continue").visible = true;
		resultsUi.rootVisualElement.Q<Button>("Flee").visible = false;
		resultsUi.rootVisualElement.Q<Button>("FightOn").visible = false;
	}

	private void PopulateResults()
	{
		var root = resultsUi.rootVisualElement;
		var totals = PartyManager.Instance.partyResults;
		var entries = PartyManager.Instance.characterResults;

		var totalColumn = root.Q<VisualElement>("TotalColumn");
		FillResultColumn(totalColumn, "Total", totals, alive: null);

		for (int i = 0; i < 3; i++)
		{
			var column = root.Q<VisualElement>($"CharacterColumn{i}");
			if (column == null)
				continue;

			if (i < entries.Count)
			{
				var entry = entries[i];
				column.RemoveFromClassList("hidden");
				FillResultColumn(column, entry.characterName, entry.result, entry.isAlive);
			}
			else
			{
				column.AddToClassList("hidden");
			}
		}
	}

	private void FillResultColumn(VisualElement column, string name, Midevil.Models.Result result, bool? alive)
	{
		column.Q<Label>("ColumnName").text = name;

		var status = column.Q<Label>("ColumnStatus");
		status.RemoveFromClassList("alive");
		status.RemoveFromClassList("dead");

		if (alive.HasValue)
		{
			status.text = alive.Value ? "Survived" : "Fallen";
			status.AddToClassList(alive.Value ? "alive" : "dead");
		}
		else
		{
			status.text = "Party";
		}

		column.Q<Label>("xpGained").text = Mathf.RoundToInt(result.xpGained).ToString();
		column.Q<Label>("kills").text = result.kills.ToString();
		column.Q<Label>("damageDealt").text = Mathf.RoundToInt(result.damageDealt).ToString();
		column.Q<Label>("damageTaken").text = Mathf.RoundToInt(result.damageTaken).ToString();
		column.Q<Label>("healed").text = Mathf.RoundToInt(result.healed).ToString();
		column.Q<Label>("hits").text = result.hits.ToString();
		column.Q<Label>("criticalHits").text = result.criticalHits.ToString();
	}

	public void SetEnemiesText(int enemiesRemaining, int totalEnemies)
	{
		if (gameUi == null || gameUi.rootVisualElement == null)
			return;

		var enemiesLabel = gameUi.rootVisualElement.Q<Label>("Enemies");
		enemiesLabel.text = $"Enemies: {enemiesRemaining}/{totalEnemies}";
	}

	public void SetWaveText(int current, int total)
	{
		if (gameUi == null || gameUi.rootVisualElement == null)
			return;

		var waveLabel = gameUi.rootVisualElement.Q<Label>("Wave");
		waveLabel.text = total < 0 ? $"Wave: {current}" : $"Wave: {current}/{total}";
	}

	public void SetTimerText(float seconds)
	{
		if (gameUi == null || gameUi.rootVisualElement == null)
			return;

		var timerLabel = gameUi.rootVisualElement.Q<Label>("Timer");
		if (timerLabel == null) return;

		seconds = Mathf.Max(0f, seconds);
		int m = Mathf.FloorToInt(seconds / 60f);
		int s = Mathf.FloorToInt(seconds % 60f);
		timerLabel.text = $"Time: {m}:{s:00}";
		timerLabel.style.display = DisplayStyle.Flex;
	}

	public void SetObjectiveText(string text)
	{
		if (gameUi == null || gameUi.rootVisualElement == null)
			return;

		var objectiveLabel = gameUi.rootVisualElement.Q<Label>("Objective");
		if (objectiveLabel == null) return;

		if (string.IsNullOrEmpty(text))
		{
			objectiveLabel.style.display = DisplayStyle.None;
			return;
		}

		objectiveLabel.text = text;
		objectiveLabel.style.display = DisplayStyle.Flex;
	}

	public void ShowVictoryScreen()
	{
		PopulateResults();
		resultsUi.rootVisualElement.visible = true;
		resultsUi.rootVisualElement.Q<Button>("Flee").visible = true;
		resultsUi.rootVisualElement.Q<Button>("FightOn").visible = false;
		resultsUi.rootVisualElement.Q<Button>("Continue").visible = false;
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
		{
			RefreshPartyStatsPanels();
			Time.timeScale = 0;
		}
		else
		{
			Time.timeScale = 1;
		}
	}

	private void RefreshPartyStatsPanels()
	{
		var members = PartyManager.Instance?.PartyMembers;
		if (members == null) return;

		foreach (var member in members)
		{
			if (member == null) continue;
			var container = statsUi.rootVisualElement.Q<VisualElement>(member.identity.id.ToString())?.parent;
			if (container == null) continue;
			CharacterGearPanel.PopulateStats(container, member);
		}
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

}
