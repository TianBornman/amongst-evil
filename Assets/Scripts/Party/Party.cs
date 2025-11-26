using Midevil.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Party : MonoBehaviour
{
	// Editor Variables
	[Header("Settings")]
	public Transform waypoint;
	public List<PartyCharacter> members = new();

	// Private Variables
	private List<PartyPosition> positions;

	// Public Methods
	public bool AddPrefabMember(PartyCharacter memberPrefab, Identity identity)
	{
		var openPosition = positions.FirstOrDefault(p => !p.isOccupied);
		openPosition.isOccupied = true;

		PartyCharacter newMember = Instantiate(memberPrefab, transform);
		newMember.identity = identity;
		newMember.partyPosition = openPosition.transform;

		members.Add(newMember);
		return true;
	}

	public bool AddMember(PartyCharacter newMember)
	{
		members.Add(newMember);
		return true;
	}

	public void SetPosition(Vector3 position)
	{
		waypoint.position = position;

		foreach (PartyPosition partyPosition in positions)
			partyPosition.SetPosition(position);

		foreach (PartyCharacter member in members)
			member.CheckPositionChanged();
	}

	// Private Methods
	private void Awake()
	{
		positions = GetComponentsInChildren<PartyPosition>().ToList();
	}

	private void Start()
	{
		UiManager.Instance.UpdateInventory();
	}
}
