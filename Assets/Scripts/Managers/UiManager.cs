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

	// Private Methods
	private void MenuToggle()
	{
		statsUi.rootVisualElement.visible = !statsUi.rootVisualElement.visible;

		if (statsUi.rootVisualElement.visible)
			Time.timeScale = 0;
		else
			Time.timeScale = 1;
	}
}
