using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InteractionManager : Singleton<InteractionManager>
{
	#region Input

	private InputSystem_Actions inputActions;
	private InputAction selectionAction;

	protected override void OnEnable()
	{
		if (Instance != this)
			return;

		base.OnEnable();

		inputActions = new InputSystem_Actions();
		selectionAction = inputActions.Player.Selection;

		inputActions.Enable();
		selectionAction.performed += OnInteractAction;
	}

	protected override void OnDisable()
	{
		if (Instance != this)
			return;

		base.OnDisable();

		selectionAction.performed -= OnInteractAction;
		inputActions.Disable();
	}

	private void OnInteractAction(InputAction.CallbackContext context)
	{
		if (selectedInteractable != null)
			selectedInteractable.Interact();
	}

	#endregion

	// Private Variables
	[HideInInspector] private IInteractable selectedInteractable;

	// Override Methods
	protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (selectedInteractable != null)
			selectedInteractable = null;
	}

	// Private Methods
	private void Update()
	{
		Vector2 mousePos = Mouse.current.position.ReadValue();
		Ray ray = Camera.main.ScreenPointToRay(mousePos);

		if (Physics.Raycast(ray, out RaycastHit hit, 1000))
		{
			hit.transform.TryGetComponent<IInteractable>(out var selectable);

			if (selectable != selectedInteractable)
			{
				if (selectedInteractable != null)
					selectedInteractable.OnHoverExit();

				selectedInteractable = selectable;

				if (selectedInteractable != null)
					selectedInteractable.OnHoverEnter();
			}
		}
		else if (selectedInteractable != null)
		{
			selectedInteractable = null;
		}
	}
}