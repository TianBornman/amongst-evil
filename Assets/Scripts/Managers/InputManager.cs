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

	// Public Properties
	public Action MenuToggleAction { get; set; }
	public Action<InputAction.CallbackContext> AbilityAction { get; set; }
	public Action SelectionAction { get; set; }
	public Action EscapeAction { get; set; }
	public Action CameraToggleAction { get; set; }
	public Action SetPartyTargetAction { get; set; }

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

		inputActions.Enable();
		menuToggleInputAction.performed += OnMenuToggle;
		abilityInputAction.performed += OnAbility;
		selectionInputAction.performed += OnSelection;
		escapeInputAction.performed += OnEscape;
		cameraToggleInputAction.performed += OnCameraToggle;
		setPartyTargetInputAction.performed += OnSetPartyTarget;
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
}
