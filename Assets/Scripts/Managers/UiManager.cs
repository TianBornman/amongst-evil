using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
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

	// Private Variables
	private List<VisualElement> upgradeCards = new();

	// Public Methods
	public void BindPlayerStats(Player player)
	{
		gameUi.rootVisualElement.Q<ProgressBar>("PlayerHealth").dataSource = player.stats;
		gameUi.rootVisualElement.Q<Label>("Level").dataSource = player;
		gameUi.rootVisualElement.Q<ProgressBar>("XpBar").dataSource = player;

		statsUi.rootVisualElement.Q<VisualElement>("Stats").dataSource = player;
	}	
	
	public void BindEnemyStats(Stats stats)
	{
		gameUi.rootVisualElement.Q<ProgressBar>("EnemyHealth").dataSource = stats;
	}

	public void BindUpgradeCard(int index, UpgradeCard card, Action<UpgradeCard> onClick)
	{
		if (index < 0 || index >= upgradeCards.Count)
			return;

		upgradeCards[index].dataSource = card;
		upgradeCards[index].RegisterCallback<ClickEvent>(evt =>
		{
			onClick(card);
		});
	}

	public void ShowLevelUp()
	{
		levelUpUI.rootVisualElement.visible = true;
		Time.timeScale = 0;
	}

	public void HideLevelUp()
	{
		levelUpUI.rootVisualElement.visible = false;
		Time.timeScale = 1;
	}

	// Private Methods
	private void Start()
	{
		statsUi.rootVisualElement.visible = false;
		levelUpUI.rootVisualElement.visible = false;
		upgradeCards = levelUpUI.rootVisualElement.Q<VisualElement>("UpgradeCards").Children().ToList();
	}

	private void MenuToggle()
	{
		statsUi.rootVisualElement.visible = !statsUi.rootVisualElement.visible;

		if (statsUi.rootVisualElement.visible)
			Time.timeScale = 0;
		else
			Time.timeScale = 1;
	}
}
