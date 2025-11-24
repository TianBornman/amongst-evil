using Midevil.Ability;
using Midevil.Effect;
using Midevil.Item;
using Midevil.UI.Elements;
using Midevil.UpgradeCard;
using System;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UiManager : Singleton<UiManager>
{
	#region Input

	private InputSystem_Actions inputActions;
	private InputAction menuToggleAction;

	protected override void OnEnable()
	{
		if (Instance != this)
			return;

		base.OnEnable();

		inputActions = new InputSystem_Actions();
		menuToggleAction = inputActions.Player.MenuToggle;

		inputActions.Enable();
		menuToggleAction.performed += OnMenuToggle;
	}

	protected override void OnDisable()
	{
		if (Instance != this)
			return;

		base.OnDisable();

		menuToggleAction.performed -= OnMenuToggle;
		inputActions.Disable();
	}

	private void OnMenuToggle(InputAction.CallbackContext context)
	{
		if (GameManager.Instance.AtHub)
			return;

		MenuToggle();
	}

	#endregion

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

		// Results
		resultsUi.rootVisualElement.visible = false;
		resultsUi.rootVisualElement.Q<VisualElement>("Results").dataSource = PlayerManager.Instance.player.Results;
		resultsUi.rootVisualElement.Q<Button>("Flee").clicked += Flee;
		resultsUi.rootVisualElement.Q<Button>("FightOn").clicked += SpawnWave;

		var continueButton = resultsUi.rootVisualElement.Q<Button>("Continue");
		continueButton.clicked += Die;
		continueButton.visible = false;
	}

	// Public Methods
	public void BindPlayerStats(Player player)
	{
		gameUi.rootVisualElement.Q<ProgressBar>("PlayerHealth").dataSource = player;
		gameUi.rootVisualElement.Q<Label>("Level").dataSource = player;
		gameUi.rootVisualElement.Q<ProgressBar>("XpBar").dataSource = player;

		statsUi.rootVisualElement.Q<VisualElement>("Stats").dataSource = player;
	}

	public void BindEnemyStats(Character character)
	{
		gameUi.rootVisualElement.Q<ProgressBar>("EnemyHealth").dataSource = character;
	}

	public void BindUpgradeCard(int index, UpgradeCard card, Action<UpgradeCard> onClick)
	{
		if (index < 0 || index >= upgradeCards.Count)
			return;

		upgradeCards[index].dataSource = card;
		upgradeCards[index].SetClickHandler(evt => onClick(card));
	}

	public void BindItemPickUp(Item item)
	{
		var itemElement = itemPickupUi.rootVisualElement.Q<VisualElement>("ItemCard");
		itemElement.dataSource = item;

		var pickUpButton = itemPickupUi.rootVisualElement.Q<ClickableElement>("PickUp");
		pickUpButton.SetClickHandler(evt =>
		{
			PlayerManager.Instance.AddItem(item.stats);
			Destroy(item.gameObject);
			HideItemPickUp();
		});

		var leaveButton = itemPickupUi.rootVisualElement.Q<ClickableElement>("Leave");
		leaveButton.SetClickHandler(evt => HideItemPickUp());
	}

	public void BindAbility(int index, Ability ability)
	{
		var abilityElement = gameUi.rootVisualElement.Q<VisualElement>("Abilities").Query<AbilityElement>().ToList();

		if (index < 0 || index >= abilityElement.Count)
			return;

		abilityElement[index].SetAbility(ability);
	}

	public void ClearAbility(int index)
	{
		var abilityElement = gameUi.rootVisualElement.Q<VisualElement>("Abilities").Query<AbilityElement>().ToList();

		if (index < 0 || index >= abilityElement.Count)
			return;

		abilityElement[index].ClearItem();
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

	public void ShowItemPickUp()
	{
		itemPickupUi.rootVisualElement.visible = true;
		Pause();
	}

	public void HideItemPickUp()
	{
		itemPickupUi.rootVisualElement.visible = false;
		Resume();
	}

	public void UpdateItems()
	{
		var itemElements = statsUi.rootVisualElement.Q<VisualElement>("Items").Query<ItemElement>().ToList();

		for (int i = 0; i < itemElements.Count; i++)
		{
			var itemElement = itemElements[i];

			if (i < PlayerManager.Instance.items.Count)
				itemElement.SetItem(PlayerManager.Instance.items[i]);
			else
				itemElement.ClearItem();
		}

		var player = PlayerManager.Instance.player;

		if (player.identity.weapon != null)
			EquipItem(player.identity.weapon.Value);
		else
			UnequipItem(ItemType.Weapon);

		if (player.identity.armour != null)
			EquipItem(player.identity.armour.Value);
		else
			UnequipItem(ItemType.Armour);
	}

	public void EquipItem(ItemStats item)
	{
		var equipSlot = statsUi.rootVisualElement.Q<VisualElement>("Equipped").Q<ItemElement>(item.type.ToString());
		equipSlot.SetItem(item);
	}

	public void UnequipItem(ItemType type)
	{
		var equipSlot = statsUi.rootVisualElement.Q<VisualElement>("Equipped").Q<ItemElement>(type.ToString());
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
	private void MenuToggle()
	{
		if (!canToggleMenu) return;

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
		PlayerManager.Instance.player.identity.Flee();
		SceneManager.LoadScene("Sect");
	}

	private void Die()
	{
		Resume();
		SceneManager.LoadScene("Sect");
	}

	private void SpawnWave()
	{
		resultsUi.rootVisualElement.visible = false;
		SpawnManager.Instance.SpawnWave();
	}
}
