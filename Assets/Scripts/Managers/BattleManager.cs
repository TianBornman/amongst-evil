using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BattleManager : Singleton<BattleManager>
{
	// Public Variables
	public List<Lane> lanes;
	public bool inBattle = false;

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
	}

	public Lane GetStartingLane(Character character)
	{
		if (character is PartyCharacter)
		{
			var lane = lanes.Find(lane => !lane.characters.OfType<PartyCharacter>().Any());
			lane.characters.Add(character);

			return lane;
		}

		return lanes.FirstOrDefault();
	}

	// Private Methods
	// Private Methods
	private void Start()
	{
		InputManager.Instance.SelectBattleCharacterAction = SelectBattleCharacter;
	}

	private void SelectBattleCharacter(InputAction.CallbackContext context)
	{
		if (GameManager.Instance.AtHub || !inBattle)
			return;

		int character = (int)context.ReadValue<float>();
		Debug.Log($"Selecting Battle Character: {character}");
	}
}
