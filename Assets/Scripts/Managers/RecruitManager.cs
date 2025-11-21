using Midevil.Models;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RecruitManager : Singleton<RecruitManager>
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
		selectionAction.performed += OnSelection;
	}

	protected override void OnDisable()
	{
		if (Instance != this)
			return;

		base.OnDisable();

		selectionAction.performed -= OnSelection;
		inputActions.Disable();
	}

	private void OnSelection(InputAction.CallbackContext context)
	{
		SelectCharacter();
	}

	#endregion

	// Editor Variables
	[Header("References")]
	public List<RecruitCharacter> recruitPrefabs;

	// Private Methods
	private void Start()
	{
		var recruitmentCount = Random.Range(2, 5);

		for (int i = 0; i < recruitmentCount; i++)
		{
			var recruitData = new RecruitData();
			recruitData.Randomize();

			var recruitPrefab = recruitPrefabs[Random.Range(0, recruitPrefabs.Count)];
			var spawnPosition = transform.position + (Vector3)(Random.insideUnitCircle * 5f);
			var recruitInstance = Instantiate(recruitPrefab, spawnPosition, Quaternion.identity);

			recruitInstance.recruitData = recruitData;
		}
	}

	private void SelectCharacter()
	{
		Vector2 mousePos = Mouse.current.position.ReadValue();
		var ray = Camera.main.ScreenPointToRay(mousePos);

		if (Physics.Raycast(ray, out RaycastHit hit))
		{
			if (hit.collider.TryGetComponent<RecruitCharacter>(out var recruit))
				HubUiManager.Instance.UpdateRecruitmentUI(recruit.recruitData);
		}
	}
}