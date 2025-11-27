using Midevil.Ability;
using Midevil.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Party : StateMachine<PartyState>
{
	#region Input

	private InputSystem_Actions inputActions;
	private InputAction abilityAction;

	private void OnEnable()
	{
		inputActions = new InputSystem_Actions();
		abilityAction = inputActions.Player.Ability;

		inputActions.Enable();
		abilityAction.performed += OnAbility;
	}

	private void OnDisable()
	{
		abilityAction.performed -= OnAbility;
		inputActions.Disable();
	}

	private void OnAbility(InputAction.CallbackContext context)
	{
		int slot = (int)context.ReadValue<float>();
		UseAbility(slot);
	}

	#endregion

	// Editor Variables
	[Header("Settings")]
	public Transform waypoint;
	public List<PartyCharacter> members = new();

	// Public Variables
	[HideInInspector] public AbilitySlot[] abilitySlots = new AbilitySlot[6];

	// Private Variables
	private List<PartyPosition> positions;
	private CameraMovement cameraMovement;

	// Public Properties
	public bool InCombat => State == PartyState.Combat;

	// Override Methods
	protected override void SetState(PartyState state)
	{
		base.SetState(state);

		switch (state)
		{
			case PartyState.Idle:
				cameraMovement.SetMapView();
				break;
			case PartyState.Combat:
				SetPosition(GetGroupCenter());
				cameraMovement.SetBattleView();
				break;
			default:
				break;
		}
	}

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

	public void AddEnemyInRange(Character character)
	{
		//if (combatEnemies.Count == 0 || !combatEnemies.Contains(character))
		//	combatEnemies.Add(character);

		//SetState(PartyState.Combat);
	}

	public void RemoveEnemyInRange(Character character)
	{
		//combatEnemies.Remove(character);	

		//if (combatEnemies.Count == 0)
		//	SetState(PartyState.Idle);
	}

	public Character GetTarget(PartyCharacter member)
	{
		//var target = combatEnemies.OrderBy(enemy => Vector3.Distance(member.transform.position, enemy.transform.position)).FirstOrDefault();
		//return target;
		return new();
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

		SetState(PartyState.Idle);
		UiManager.Instance.UpdateCharacterPanels();
	}

	private Vector3 GetGroupCenter()
	{
		//if (combatEnemies == null || combatEnemies.Count == 0)
		//	return Vector3.zero;

		//Bounds b = new Bounds(combatEnemies[0].transform.position, Vector3.zero);

		//for (int i = 1; i < combatEnemies.Count; i++)
		//	b.Encapsulate(combatEnemies[i].transform.position);

		//return b.center;
		return new();
	}

	private void UseAbility(int slot)
	{
		abilitySlots[slot].TryUseAbility();
	}
}
