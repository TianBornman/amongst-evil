using Midevil.Ability;
using Midevil.Item;
using Midevil.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PartyManager : Singleton<PartyManager>
{
	#region Input

	private InputSystem_Actions inputActions;
	private InputAction setPartyTargetAction;

	protected override void OnEnable()
	{
		if (Instance != this)
			return;

		base.OnEnable();

		inputActions = new InputSystem_Actions();
		setPartyTargetAction = inputActions.Player.SetPartyTarget;

		inputActions.Enable();
		setPartyTargetAction.performed += OnSetPartyTarget;
	}

	protected override void OnDisable()
	{
		if (Instance != this)
			return;

		base.OnDisable();

		setPartyTargetAction.performed -= OnSetPartyTarget;
		inputActions.Disable();
	}

	private void OnSetPartyTarget(InputAction.CallbackContext context)
	{
		if (GameManager.Instance.AtHub)
			return;

		SetPartyTarget();
	}

	#endregion

	// Editor Variables
	[Header("References")]
	public PartyCharacter partyCharacterPrefab;

	// Public Variables
	public int maxPartySize = 3;
	public List<Identity> partyIdentities = new();
	private Party playerParty;

	// Public Properties
	public bool InCombat => playerParty.InCombat;

	// Override Methods
	protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (GameManager.Instance.AtHub)
		{
			partyIdentities = new();
			return;
		}

		playerParty = FindFirstObjectByType<Party>();

		foreach (Identity identity in partyIdentities)
			playerParty.AddPrefabMember(partyCharacterPrefab, identity);
	}

	// Public Methods
	public void RecruitPartyMember(Identity identity)
	{
		if (partyIdentities.Count >= maxPartySize)
			return;

		partyIdentities.Add(identity);
	}

	public void RemovePartyMember(PartyCharacter character) => playerParty.RemoveMember(character);

	public void StartRun()
	{
		foreach (Identity identity in partyIdentities)
		{
			var bloodVaultEntry = new BloodVaultEntry
			{
				identity = identity,
				status = BloodVaultStatus.Alive
			};

			BloodvaultManager.AddOrUpdate(bloodVaultEntry);

			identity.ClearGear();
		}
	}

	public void AddEnemyInRange(Character character) => playerParty.AddEnemyInRange(character);
	public void RemoveEnemyInRange(Character character) => playerParty.RemoveEnemyInRange(character);

	public void EquipItem(string id, ItemStats item)
	{
		var character = playerParty.members.Where(member => member.identity.id == id).FirstOrDefault();

		if (character == null)
			return;

		character.EquipItem(item);
	}

	public void UnequipItem(string id, ItemStats item)
	{
		var character = playerParty.members.Where(member => member.identity.id == id).FirstOrDefault();

		if (character == null)
			return;

		character.UnequipItem(item);
	}

	public void BindAbility(Ability ability, PartyCharacter partyCharacter) => playerParty.BindAbility(ability, partyCharacter);
	public void ClearAbility(Ability ability) => playerParty.ClearAbility(ability);

	public Character GetTarget(PartyCharacter charater) => playerParty.GetTarget(charater);

	// Private Methods
	private void SetPartyTarget()
	{
		Vector2 mousePos = Mouse.current.position.ReadValue();
		Ray ray = Camera.main.ScreenPointToRay(mousePos);

		if (Physics.Raycast(ray, out RaycastHit hit, 1000, RefManager.Instance.targetableMask))
		{
			playerParty.SetPosition(hit.point);
		}
	}
}
