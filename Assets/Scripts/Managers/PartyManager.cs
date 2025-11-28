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
	// Editor Variables
	[Header("References")]
	public PartyCharacter partyCharacterPrefab;

	// Public Variables
	public int maxPartySize = 3;
	public List<Identity> partyIdentities = new();
	public Result partyResults = new();

	// Private Variables
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

	public void EndRun()
	{
		partyResults.Clear();

		foreach (var character in playerParty.members)
			character.identity.Save(BloodVaultStatus.Alive);
	}

	public void CalculateStats()
	{
		partyResults.Clear();

		foreach (Identity identity in partyIdentities)
			partyResults.Add(identity.currentResult);
	}

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

	public void SetPartyTarget(Vector3 position) => playerParty.SetPosition(position);

	// Private Methods
	private void Start()
	{
		InputManager.Instance.SetPartyTargetAction = SetPartyTarget;
	}

	private void SetPartyTarget()
	{
		if (GameManager.Instance.AtHub)
			return;

		Vector2 mousePos = Mouse.current.position.ReadValue();
		Ray ray = Camera.main.ScreenPointToRay(mousePos);

		if (Physics.Raycast(ray, out RaycastHit hit, 1000, RefManager.Instance.targetableMask))
		{
			playerParty.SetPosition(hit.point);
		}
	}
}
