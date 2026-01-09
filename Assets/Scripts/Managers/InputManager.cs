using System;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
	// Private Variables
	private InputSystem_Actions inputActions;
	private InputAction menuToggleInputAction;
	private InputAction abilityInputAction;
	private InputAction selectionInputAction;
	private InputAction escapeInputAction;
	private InputAction cameraToggleInputAction;
	private InputAction setPartyTargetInputAction;
	private InputAction mapViewInputAction;
	private InputAction selectBattleCharacterInputAction;

	// Public Properties
	public Action MenuToggleAction { get; set; }
	public Action<InputAction.CallbackContext> AbilityAction { get; set; }
	public Action SelectionAction { get; set; }
	public Action EscapeAction { get; set; }
	public Action CameraToggleAction { get; set; }
	public Action SetPartyTargetAction { get; set; }
	public Action MapViewInputActionStarted { get; set; }
	public Action MapViewInputActionCancelled { get; set; }
	public Action<InputAction.CallbackContext> SelectBattleCharacterAction { get; set; }

	protected override void OnEnable()
	{
		if (Instance != this)
			return;

		base.OnEnable();

		inputActions = new InputSystem_Actions();
		menuToggleInputAction = inputActions.Player.MenuToggle;
		abilityInputAction = inputActions.Player.Ability;
		selectionInputAction = inputActions.Player.Selection;
		escapeInputAction = inputActions.Player.Escape;
		cameraToggleInputAction = inputActions.Player.CameraToggle;
		setPartyTargetInputAction = inputActions.Player.SetPartyTarget;
        mapViewInputAction = inputActions.Player.MapView;
		selectBattleCharacterInputAction = inputActions.Player.SelectBattleCharacter;


        inputActions.Enable();
		menuToggleInputAction.performed += OnMenuToggle;
		abilityInputAction.performed += OnAbility;
		selectionInputAction.performed += OnSelection;
		escapeInputAction.performed += OnEscape;
		cameraToggleInputAction.performed += OnCameraToggle;
		setPartyTargetInputAction.performed += OnSetPartyTarget;
		mapViewInputAction.started += OnMapViewStarted;
		mapViewInputAction.canceled += OnMapViewCancelled;
		selectBattleCharacterInputAction.performed += OnSelectBattleCharacter;
    }

	protected override void OnDisable()
	{
		if (Instance != this)
			return;

		base.OnDisable();

		menuToggleInputAction.performed -= OnMenuToggle;
		abilityInputAction.performed -= OnAbility;
		selectionInputAction.performed -= OnSelection;
		escapeInputAction.performed -= OnEscape;
		cameraToggleInputAction.performed -= OnCameraToggle;
		setPartyTargetInputAction.performed -= OnSetPartyTarget;
        mapViewInputAction.started -= OnMapViewStarted;
        mapViewInputAction.canceled -= OnMapViewCancelled;
		selectBattleCharacterInputAction.canceled -= OnSelectBattleCharacter;
        inputActions.Disable();
	}

	private void OnMenuToggle(InputAction.CallbackContext context)
	{
		MenuToggleAction?.Invoke();
	}

	private void OnAbility(InputAction.CallbackContext context)
	{
		AbilityAction?.Invoke(context);
	}

	private void OnSelection(InputAction.CallbackContext context)
	{
		SelectionAction?.Invoke();
	}

	private void OnEscape(InputAction.CallbackContext context)
	{
		EscapeAction?.Invoke();
	}

	private void OnCameraToggle(InputAction.CallbackContext context)
	{
		CameraToggleAction?.Invoke();
	}

	private void OnSetPartyTarget(InputAction.CallbackContext context)
	{
		SetPartyTargetAction?.Invoke();
	}

	private void OnMapViewStarted(InputAction.CallbackContext context)
	{
		MapViewInputActionStarted?.Invoke();
	}

	private void OnMapViewCancelled(InputAction.CallbackContext context)
	{
		MapViewInputActionCancelled?.Invoke();
	}

	private void OnSelectBattleCharacter(InputAction.CallbackContext context)
	{
		SelectBattleCharacterAction?.Invoke(context);
	}
}
