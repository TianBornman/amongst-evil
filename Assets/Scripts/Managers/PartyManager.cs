using Midevil.Ability;
using Midevil.Item;
using Midevil.Mission;
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
	public List<CharacterResultEntry> characterResults = new();
	public MissionAftermath lastAftermath;
	public Mission CurrentMission { get; private set; }

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

	public void StartRun(Mission mission)
	{
		CurrentMission = mission;

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

		Midevil.Boons.RunBoonManager.Instance?.BeginRun();
	}

	public void FinalizeMission(bool success)
	{
		var sect = Midevil.Progression.SectProgressManager.Instance;
		var data = new MissionAftermath
		{
			success = success,
			mission = CurrentMission,
			standingBefore = sect != null ? sect.Data.standing : 0,
			rankBeforeName = sect?.CurrentRank?.rankName ?? string.Empty,
		};

		if (sect != null && CurrentMission != null)
			sect.RecordMissionCompleted(CurrentMission, success);

		data.standingAfter = sect != null ? sect.Data.standing : 0;
		data.standingDelta = data.standingAfter - data.standingBefore;
		data.rankAfterName = sect?.CurrentRank?.rankName ?? string.Empty;
		data.ascended = !string.IsNullOrEmpty(data.rankAfterName) && data.rankAfterName != data.rankBeforeName;
		data.ascensionPending = sect != null && sect.AscensionPending;
		data.nextRankName = sect?.NextRank?.rankName ?? string.Empty;

		foreach (var entry in characterResults)
		{
			if (entry.isAlive)
				data.survivors.Add(entry.characterName);
			else
				data.fallen.Add(entry.characterName);
		}

		lastAftermath = data;
	}

	public void EndRun()
	{
		Midevil.Boons.RunBoonManager.Instance?.EndRun();

		var aliveIds = new HashSet<string>();

		if (playerParty != null)
			foreach (var character in playerParty.members)
				aliveIds.Add(character.identity.id);

		foreach (var identity in partyIdentities)
		{
			if (aliveIds.Contains(identity.id))
				identity.RecordStatus(BloodVaultStatus.Alive);

			identity.CommitResult();
		}

		partyResults.Clear();
		characterResults.Clear();
	}

	public void AddPartyXp(float amount, PartyCharacter character) => playerParty.AddPartyXp(amount, character);

	public void CheckBattle() => playerParty.CheckBattle();

	public void CalculateStats()
	{
		partyResults.Clear();
		characterResults.Clear();

		var aliveIds = new HashSet<string>();
		if (playerParty != null)
			foreach (var member in playerParty.members)
				aliveIds.Add(member.identity.id);

		foreach (Identity identity in partyIdentities)
		{
			partyResults.Add(identity.currentResult);
			characterResults.Add(new CharacterResultEntry
			{
				characterName = identity.characterName,
				isAlive = aliveIds.Contains(identity.id),
				result = identity.currentResult,
			});
		}
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
