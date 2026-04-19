using Midevil.Ability;
using Midevil.Item;
using Midevil.Models;
using Midevil.Party;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
	public Transform Center => playerParty.partyCenter;
	public IReadOnlyList<PartyCharacter> PartyMembers => playerParty?.members;

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
		if (partyIdentities.Count >= maxPartySize || partyIdentities.Contains(identity))
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

	public void AddPartyXp(float amount, PartyCharacter character) => playerParty.AddPartyXp(amount, character);

	public void CheckBattle() => playerParty.CheckBattle();

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

		character.equipment.EquipItem(item);
	}

	public void UnequipItem(string id, ItemStats item)
	{
		var character = playerParty.members.Where(member => member.identity.id == id).FirstOrDefault();

		if (character == null)
			return;

		character.equipment.UnequipItem(item);
	}

	public void BindAbility(Ability ability, PartyCharacter partyCharacter) => playerParty.BindAbility(ability, partyCharacter);
	public void ClearAbility(Ability ability) => playerParty.ClearAbility(ability);

	// Private Methods
	private void Start()
	{
	}

	private void Update()
	{
		if (GameManager.Instance.AtHub || GameManager.Instance.IsGamePaused || playerParty == null)
			return;

		playerParty.Move(InputManager.Instance.MoveInput);
	}
}
