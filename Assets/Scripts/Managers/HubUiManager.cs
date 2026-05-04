using Midevil.Item;
using Midevil.Mission;
using Midevil.Models;
using Midevil.Progression;
using Midevil.UI.Elements;
using System.Collections.Generic;
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
	public UIDocument settingsUiPrefab;
	public UIDocument missionBoardUiPrefab;
	public UIDocument spiralUiPrefab;

	[Header("Mission Board")]
	public int missionBoardCount = 3;
	public int missionBoardMinDifficulty = 1;
	public int missionBoardMaxDifficulty = 3;

	// Public Variables
	public bool BloodVaultOpen => bloodVaultUi.rootVisualElement.visible;
	public bool ArmouryOpen => recruitmentArmouryUi.visible || recruitmentGearUi.visible;
	public bool SettingsOpen => settingsUi.rootVisualElement.visible;
	public bool MissionBoardOpen => missionBoardUi.rootVisualElement.visible;
	public bool SpiralOpen => spiralUi != null && spiralUi.rootVisualElement.visible;

	// Private Variables
	private UIDocument mainMenuUi;
	private UIDocument recruitmentUi;
	private VisualElement recruitmentArmouryUi;
	private VisualElement recruitmentGearUi;
	private UIDocument bloodVaultUi;
	private UIDocument armouryUi;
	private UIDocument settingsUi;
	private UIDocument missionBoardUi;
	private UIDocument spiralUi;
	private List<Mission> currentMissionBatch;

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
		settingsUi = Instantiate(settingsUiPrefab).GetComponent<UIDocument>();
		missionBoardUi = Instantiate(missionBoardUiPrefab).GetComponent<UIDocument>();
		if (spiralUiPrefab != null)
			spiralUi = Instantiate(spiralUiPrefab).GetComponent<UIDocument>();

		// Config
		mainMenuUi.rootVisualElement.visible = true;
		recruitmentUi.rootVisualElement.visible = false;
		recruitmentArmouryUi.visible = false;
		recruitmentGearUi.visible = false;
		bloodVaultUi.rootVisualElement.visible = false;
		armouryUi.rootVisualElement.visible = false;
		settingsUi.rootVisualElement.visible = false;
		missionBoardUi.rootVisualElement.visible = false;
		if (spiralUi != null)
		{
			spiralUi.rootVisualElement.visible = false;
			SetupSpiralUI();
		}

		var cancelButton = missionBoardUi.rootVisualElement.Q<Button>("Cancel");
		if (cancelButton != null)
			cancelButton.clicked += HideMissionBoardUI;

		mainMenuUi.rootVisualElement.Q<Button>("Play").clicked += HubManager.Instance.StartGame;

		CharacterGearPanel.WireTabs(recruitmentGearUi);

		var musicSlider = settingsUi.rootVisualElement.Q<Slider>("Music");

        musicSlider.value = PlayerPrefs.GetFloat("MusicVolumeKey");

        musicSlider.RegisterValueChangedCallback(evt =>
        {
            AudioManager.Instance.MusicVolume = evt.newValue;
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

		if (HubManager.Instance.currentCharacter == null)
			return;

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

		if (character.identity.weapon != null)
			EquipItem(recruitmentGearUi, character.identity.weapon);
		else
			UnequipItem(recruitmentGearUi, ItemType.Weapon);

		if (character.identity.armour != null)
			EquipItem(recruitmentGearUi, character.identity.armour);
		else
			UnequipItem(recruitmentGearUi, ItemType.Armour);

		CharacterGearPanel.PopulateStats(recruitmentGearUi, character);

		recruitmentGearUi.visible = true;
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

	public void HideRecruitGearUI()
	{
		recruitmentGearUi.visible = false;
	}

	public void ShowMissionBoardUI()
	{
		currentMissionBatch = MissionGenerator.GenerateBatch(missionBoardCount, missionBoardMinDifficulty, missionBoardMaxDifficulty);

		var list = missionBoardUi.rootVisualElement.Q<VisualElement>("MissionList");
		var cards = list.Query<VisualElement>("Card").ToList();

		for (int i = 0; i < cards.Count; i++)
		{
			var card = cards[i];

			if (i < currentMissionBatch.Count)
			{
				var mission = currentMissionBatch[i];
				PopulateMissionCard(card, mission);
				card.style.display = DisplayStyle.Flex;
			}
			else
			{
				card.style.display = DisplayStyle.None;
			}
		}

		missionBoardUi.rootVisualElement.visible = true;
	}

	public void HideMissionBoardUI()
	{
		missionBoardUi.rootVisualElement.visible = false;
	}

	private void PopulateMissionCard(VisualElement card, Mission mission)
	{
		var titleLabel = card.Q<Label>("Title");
		var typeLabel = card.Q<Label>("Type");
		var threatLabel = card.Q<Label>("Threat");
		var flavorLabel = card.Q<Label>("Flavor");

		if (titleLabel != null) titleLabel.text = mission.title;
		if (typeLabel != null) typeLabel.text = FormatMissionType(mission.type);
		if (threatLabel != null) threatLabel.text = $"Threat {mission.difficulty}";
		if (flavorLabel != null) flavorLabel.text = mission.flavorText;

		card.userData = mission;

		var captured = mission;
		card.RegisterCallback<ClickEvent>(_ => OnMissionSelected(captured));
	}

	private void OnMissionSelected(Mission mission)
	{
		HideMissionBoardUI();
		HubManager.Instance.StartRun(mission);
	}

	private static string FormatMissionType(MissionType type) => type switch
	{
		MissionType.Purge => "Purge",
		MissionType.RelicRecovery => "Relic Recovery",
		MissionType.Chaos => "Chaos",
		_ => type.ToString()
	};

	public void ShowSettingsUI()
	{
        settingsUi.rootVisualElement.visible = true;
	}
	public void HideSettingsUI()
	{
        settingsUi.rootVisualElement.visible = false;
	}

	private void SetupSpiralUI()
	{
		var root = spiralUi.rootVisualElement;
		var close = root.Q<Button>("Close");
		if (close != null) close.clicked += HideSpiralUI;

		var ascend = root.Q<Button>("AscendButton");
		if (ascend != null) ascend.clicked += OnAscendClicked;

		var continueBtn = root.Q<Button>("AscensionContinue");
		if (continueBtn != null) continueBtn.clicked += () =>
		{
			root.Q<VisualElement>("AscensionOverlay").AddToClassList("hidden");
		};

		var sect = SectProgressManager.Instance;
		if (sect != null)
		{
			sect.OnAscended += OnAscended;
			sect.OnAscensionAvailable += _ => { if (SpiralOpen) RebuildSpiralUI(); };
			sect.OnStandingChanged += _ => { if (SpiralOpen) RebuildSpiralUI(); };
		}
	}

	public void ShowSpiralUI()
	{
		if (spiralUi == null) return;
		RebuildSpiralUI();
		spiralUi.rootVisualElement.visible = true;
	}

	public void HideSpiralUI()
	{
		if (spiralUi == null) return;
		spiralUi.rootVisualElement.visible = false;
	}

	private void OnAscendClicked()
	{
		var sect = SectProgressManager.Instance;
		if (sect == null) return;
		sect.PerformAscension();
	}

	private void OnAscended(SectRankData rank)
	{
		if (spiralUi == null || rank == null) return;
		var root = spiralUi.rootVisualElement;
		var overlay = root.Q<VisualElement>("AscensionOverlay");
		root.Q<Label>("AscensionRank").text = rank.rankName;
		root.Q<Label>("AscensionQuote").text = string.IsNullOrEmpty(rank.quote) ? rank.flavor : rank.quote;
		overlay.RemoveFromClassList("hidden");
		spiralUi.rootVisualElement.visible = true;
		RebuildSpiralUI();
	}

	private void RebuildSpiralUI()
	{
		var sect = SectProgressManager.Instance;
		if (sect == null || sect.spiral == null) return;

		var root = spiralUi.rootVisualElement;
		root.Q<Label>("Standing").text = $"Standing  {sect.Data.standing}";

		var banner = root.Q<VisualElement>("AscensionBanner");
		if (sect.AscensionPending && sect.NextRank != null)
		{
			banner.RemoveFromClassList("hidden");
			root.Q<Label>("AscensionText").text = $"You stand ready to rise to {sect.NextRank.rankName}.";
		}
		else
		{
			banner.AddToClassList("hidden");
		}

		var list = root.Q<ScrollView>("RankList");
		list.Clear();

		var ranks = sect.spiral.ranks;
		foreach (var rank in ranks)
		{
			if (rank == null) continue;
			list.Add(BuildRankRow(rank, sect));
		}
	}

	private VisualElement BuildRankRow(SectRankData rank, SectProgressManager sect)
	{
		var data = sect.Data;
		bool isCurrent = rank.rankNumber == data.currentRank;
		bool isLocked = rank.rankNumber > data.currentRank + 1;

		var row = new VisualElement();
		row.AddToClassList("spiral-rank");
		if (isCurrent) row.AddToClassList("spiral-rank--current");
		if (isLocked) row.AddToClassList("spiral-rank--locked");

		var head = new VisualElement();
		head.AddToClassList("spiral-rank__head");
		var name = new Label($"{rank.rankName}");
		name.AddToClassList("spiral-rank__name");
		var num = new Label($"Rank {rank.rankNumber}");
		num.AddToClassList("spiral-rank__num");
		head.Add(name);
		head.Add(num);
		row.Add(head);

		var status = new Label(rank.statusTitle);
		status.AddToClassList("spiral-rank__status");
		row.Add(status);

		if (!string.IsNullOrEmpty(rank.quote))
		{
			var quote = new Label(rank.quote);
			quote.AddToClassList("spiral-rank__quote");
			row.Add(quote);
		}

		if (rank.rankNumber > data.currentRank)
		{
			if (isLocked)
			{
				var unknown = new Label("Requirements hidden until the prior rank is taken.");
				unknown.AddToClassList("spiral-rank__quote");
				row.Add(unknown);
			}
			else
			{
				foreach (var req in rank.requirements)
				{
					if (req == null) continue;
					row.Add(BuildRequirementRow(req, data));
				}
			}
		}

		return row;
	}

	private VisualElement BuildRequirementRow(RankRequirement req, SectProgressData data)
	{
		var wrap = new VisualElement();
		var line = new VisualElement();
		line.AddToClassList("spiral-req");
		if (req.IsMet(data)) line.AddToClassList("spiral-req--met");

		var label = new Label(string.IsNullOrEmpty(req.label) ? req.name : req.label);
		label.AddToClassList("spiral-req__label");
		var prog = new Label(req.ProgressText(data));
		prog.AddToClassList("spiral-req__progress");
		line.Add(label);
		line.Add(prog);
		wrap.Add(line);

		var bar = new VisualElement();
		bar.AddToClassList("spiral-bar");
		var fill = new VisualElement();
		fill.AddToClassList("spiral-bar__fill");
		fill.style.width = Length.Percent(Mathf.Clamp01(req.Progress01(data)) * 100f);
		bar.Add(fill);
		wrap.Add(bar);

		return wrap;
	}
}
