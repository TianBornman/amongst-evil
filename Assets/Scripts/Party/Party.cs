using Midevil.Ability;
using Midevil.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Party : StateMachine
{
	// Editor Variables
	[Header("Settings")]
	public Transform waypoint;
	public List<PartyCharacter> members = new();

	// Public Variables
	[HideInInspector] public AbilitySlot[] abilitySlots = new AbilitySlot[6];
	[HideInInspector] public CameraMovement cameraMovement;

	// Private Variables
	private List<PartyPosition> positions;

	// Public Methods
	public void AddPrefabMember(PartyCharacter memberPrefab, Identity identity)
	{
		var openPosition = positions.FirstOrDefault(p => !p.isOccupied);
		openPosition.isOccupied = true;

		PartyCharacter newMember = Instantiate(memberPrefab, openPosition.transform.position, Quaternion.identity, transform);
		newMember.identity = identity;
		newMember.idlePos = openPosition.transform;

		members.Add(newMember);
	}

	public void RemoveMember(PartyCharacter character)
	{
		members.Remove(character);

		if (members.Count <= 0 )
		{
			PartyManager.Instance.CalculateStats();
			UiManager.Instance.ShowDeathScreen();
		}
	}

	public void SetPosition(Vector3 position)
	{
		waypoint.position = position;

		foreach (PartyPosition partyPosition in positions)
			partyPosition.SetPosition(position);

		foreach (PartyCharacter member in members)
			member.CheckPositionChanged();
	}

	public void CheckBattle()
	{
		bool inCombat = members.Where(member => member.InBattle).Any();

		if (inCombat)
			SetState(new BattleState(this));
		else 
			SetState(new ExploreState(this));
	}

	public void BindAbility(Ability ability, PartyCharacter character)
	{
		for (int i = 0; i < abilitySlots.Length; i++)
		{
			if (abilitySlots[i].HasAbility)
				continue;

			abilitySlots[i].abilityId = ability.id;
			abilitySlots[i].character = character;

			UiManager.Instance.BindAbility(i, ability);
			break;
		}
	}

	public void ClearAbility(Ability ability)
	{
		var slot = abilitySlots.FirstOrDefault(slot => slot.abilityId == ability.id);
		slot.Clear();
	}

	public Vector3 GetGroupCenter()
	{
		var positions = members.Where(member => member.IsAlive)
									 .Select(member => member.transform.position)
									 .ToList();

		var enemyPositions = members.Where(member => member.IsAlive)
									.Select(member => member.target.transform.position)
									.ToList();

		positions.AddRange(enemyPositions);

		var bound = new Bounds(positions.FirstOrDefault(), Vector3.zero);

		foreach (var member in positions)
			bound.Encapsulate(member);

		return bound.center;
	}

	// Private Methods
	private void Awake()
	{
		positions = GetComponentsInChildren<PartyPosition>().ToList();
		cameraMovement = FindFirstObjectByType<CameraMovement>();
	}

	private void Start()
	{
		for (int i = 0; i < abilitySlots.Length; i++)
			abilitySlots[i].slotIndex = i;

		SetState(new ExploreState(this));
		UiManager.Instance.UpdateCharacterPanels();

		InputManager.Instance.AbilityAction = UseAbility;
	}

	private void UseAbility(InputAction.CallbackContext context)
	{
		int slot = (int)context.ReadValue<float>();
		abilitySlots[slot].TryUseAbility();
	}
}
