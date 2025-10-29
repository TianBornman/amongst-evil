using Midevil.Item;
using Midevil.UI.Elements;
using Midevil.UpgradeCard;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UiManager : Singleton<UiManager>
{
	#region Input

	private InputSystem_Actions inputActions;
	private InputAction menuToggleAction;

	private void OnEnable()
	{
		inputActions = new InputSystem_Actions();
		menuToggleAction = inputActions.Player.MenuToggle;

		inputActions.Enable();
		menuToggleAction.performed += OnMenuToggle;
	}

	private void OnDisable()
	{
		menuToggleAction.performed -= OnMenuToggle;
		inputActions.Disable();
	}

	private void OnMenuToggle(InputAction.CallbackContext context)
	{
		MenuToggle();
	}

	#endregion

	// Editor Variables
	[Header("References")]
	public UIDocument gameUi;
	public UIDocument statsUi;
	public UIDocument levelUpUI;
	public UIDocument itemPickupUI;

	// Private Variables
	private List<ClickableElement> upgradeCards = new();
	private bool canToggleMenu = true;

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
		var itemElement = itemPickupUI.rootVisualElement.Q<VisualElement>("ItemCard");
		itemElement.dataSource = item;

		var pickUpButton = itemPickupUI.rootVisualElement.Q<ClickableElement>("PickUp");
		pickUpButton.SetClickHandler(evt => 
		{
			PlayerManager.Instance.AddItem(item.stats);
			Destroy(item.gameObject);
			HideItemPickUp();
		});

		var leaveButton = itemPickupUI.rootVisualElement.Q<ClickableElement>("Leave");
		leaveButton.SetClickHandler(evt => HideItemPickUp());
	}

	public void ShowLevelUp()
	{
		levelUpUI.rootVisualElement.visible = true;
		Pause();
	}

	public void HideLevelUp()
	{
		levelUpUI.rootVisualElement.visible = false;
		Resume();
	}

	public void ShowItemPickUp()
	{
		itemPickupUI.rootVisualElement.visible = true;
		Pause();
	}

	public void HideItemPickUp()
	{
		itemPickupUI.rootVisualElement.visible = false;
		Resume();
	}

	public void AddItem(ItemStats item)
	{
		if (item.type == ItemType.Relic)
		{
			var newItemElement = new ItemElement();
			newItemElement.SetItem(item);

			statsUi.rootVisualElement.Q<VisualElement>("Relics").Add(newItemElement);
			return;
		}

		var itemElement = statsUi.rootVisualElement.Q<VisualElement>("Items").Q<ItemElement>();
		itemElement.SetItem(item);
	}

	public void EquipItem(ItemStats item)
	{
		var equipSlot = statsUi.rootVisualElement.Q<VisualElement>("Equipped").Q<ItemElement>(item.type.ToString());
		equipSlot?.SetItem(item);
	}

	public void UnequipItem(ItemType type)
	{
		var equipSlot = statsUi.rootVisualElement.Q<VisualElement>("Equipped").Q<ItemElement>(type.ToString());
		equipSlot?.ClearItem();
	}

	public void ShowDeathScreen()
	{
		SceneManager.LoadScene("Sect");
	}

	// Private Methods
	private void Start()
	{
		statsUi.rootVisualElement.visible = false;
		levelUpUI.rootVisualElement.visible = false;
		upgradeCards = levelUpUI.rootVisualElement.Q<VisualElement>("UpgradeCards").Query<ClickableElement>().ToList();
		itemPickupUI.rootVisualElement.visible = false;
	}

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
		Time.timeScale = 0;
		canToggleMenu = false;
	}

	private void Resume()
	{
		Time.timeScale = 1;
		canToggleMenu = true;
	}
}
