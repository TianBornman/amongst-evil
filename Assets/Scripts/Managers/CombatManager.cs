using Midevil.Party;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CombatManager : Singleton<CombatManager>
{
	// Public Variables
	public List<Lane> lanes;
	public bool inBattle = false;

	// Private Variables
	private Character selectedCharacter;

	// Override Methods
	protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (GameManager.Instance.AtHub)
			return;

		lanes = FindObjectsByType<Lane>(UnityEngine.FindObjectsSortMode.None).ToList();
	}

	// Public Methods
	public void StartBattle(Encounter encounter)
	{
		PartyManager.Instance.SetBattleState();
		inBattle = true;
		selectedCharacter = PartyManager.Instance.GetCharacterIndex(0);

		foreach (var enemy in encounter.encounterCharacters)
		{
			var lane = GetStartingLane(enemy);
			enemy.EnterCombat(lane);
		}
	}

	public Lane GetStartingLane(Character character)
	{
		var lane = lanes.Find(lane => !lane.characters.Any(mem => mem.team == character.team));

		if (lane != null)
		{
			lane.characters.Add(character);
			return lane;
		}

		return lanes.FirstOrDefault();
	}

	// Private Methods
	private void Start()
	{
		InputManager.Instance.SelectBattleCharacterAction = SelectBattleCharacter;
		InputManager.Instance.VerticalCharacterMovementAction = VerticalCharacterMovement;
	}

	private void Update()
	{
		if (GameManager.Instance.AtHub || !inBattle)
			return;

		float x = InputManager.Instance.HorizontalCharacerMovementAxis;
		selectedCharacter.SetMovementIntent(selectedCharacter.combatPositionIntent.position + new Vector3(x, 0) * Time.deltaTime * selectedCharacter.stats.moveSpeed);
	}

	private void SelectBattleCharacter(InputAction.CallbackContext context)
	{
		if (GameManager.Instance.AtHub || !inBattle)
			return;

		int character = (int)context.ReadValue<float>();
		var newSelectedCharacter = PartyManager.Instance.GetCharacterIndex(character);

		if (newSelectedCharacter != null)
			selectedCharacter = newSelectedCharacter;
	}

	private void VerticalCharacterMovement(InputAction.CallbackContext context)
	{
		var originalLane = selectedCharacter.lane;

		int direction = (int)context.ReadValue<float>();

		var laneNumber = (int)originalLane.type;
		var targetLaneType = (LaneType)Mathf.Clamp(laneNumber - direction, 0, 2);

		Lane newLane = lanes.Find(l => l.type == targetLaneType);

		var lanePosition = new Vector3(selectedCharacter.combatPositionIntent.position.x,
			selectedCharacter.combatPositionIntent.position.y, newLane.transform.position.z);

		selectedCharacter.lane = newLane;
		selectedCharacter.SetMovementIntent(lanePosition);

		if (originalLane != newLane)
		{
			originalLane.characters.Remove(selectedCharacter);
			newLane.characters.Add(selectedCharacter);
		}
	}
}
