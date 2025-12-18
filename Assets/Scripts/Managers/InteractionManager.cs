using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InteractionManager : Singleton<InteractionManager>
{
	// Private Variables
	[HideInInspector] private IInteractable selectedInteractable;

	// Override Methods
	protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (selectedInteractable != null)
			selectedInteractable = null;
	}

	// Private Methods
	private void Start()
	{
		InputManager.Instance.SelectionAction = Selection;
	}

	private void Selection()
	{
		if (selectedInteractable != null)
			selectedInteractable.Interact();
	}

	private void Update()
	{
		if (GameManager.Instance.IsGamePaused || GameManager.Instance.AtMainMenu)
			return;

		Vector2 mousePos = Mouse.current.position.ReadValue();
		Ray ray = Camera.main.ScreenPointToRay(mousePos);

		if (Physics.Raycast(ray, out RaycastHit hit, 1000, RefManager.Instance.targetableMask))
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