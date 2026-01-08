using Midevil.Ability;
using Midevil.Camera;
using Midevil.Helpers;
using Midevil.Models;
using Midevil.Party.States;
using NUnit.Framework.Constraints;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Midevil.Party
{
	public class Party : StateMachine
	{
		// Editor Variables
		[Header("Settings")]
		public Transform waypoint;
		public Transform partyCenter;
		public List<PartyCharacter> members = new();

		// Public Variables
		[HideInInspector] public List<AbilitySlot> abilitySlots = new();
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
			newMember.SetIndicatorColor(ColorHelper.GetPartyColor(members.Count));

			members.Add(newMember);
			abilitySlots.Add(new AbilitySlot() { slotIndex = abilitySlots.Count, character = newMember });
			abilitySlots.Add(new AbilitySlot() { slotIndex = abilitySlots.Count, character = newMember });
		}

		public void RemoveMember(PartyCharacter character)
		{
			members.Remove(character);

			if (members.Count <= 0)
			{
				PartyManager.Instance.CalculateStats();
				UiManager.Instance.ShowDeathScreen();
			}
		}

		public void SetPosition(Vector3 position)
		{
			waypoint.position = position;

			foreach (var partyPosition in positions)
				partyPosition.SetPosition(position);

			foreach (var member in members)
				member.CheckPositionChanged();
		}

		public void AddPartyXp(float amount, PartyCharacter character)
		{
			foreach (var member in members)
				member.AddXp(amount, true);
		}

		public void BindAbility(Ability.Ability ability, PartyCharacter character)
		{
			for (int i = 0; i < abilitySlots.Count; i++)
			{
				if (abilitySlots[i].HasAbility || abilitySlots[i].character != character)
					continue;

				abilitySlots[i].abilityId = ability.id;

				UiManager.Instance.BindAbility(i, ability);
				break;
			}
		}

		public void ClearAbility(Ability.Ability ability)
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
			SetState(new ExploreState(this));
			UiManager.Instance.UpdateCharacterPanels();

			InputManager.Instance.AbilityAction = UseAbility;
		}

		protected override void Update()
		{
			base.Update();

			SetPartyCenter();
		}

		private void UseAbility(InputAction.CallbackContext context)
		{
			int slot = (int)context.ReadValue<float>();
			abilitySlots[slot].TryUseAbility();
		}

		private void SetPartyCenter()
		{
			bool hasAny = false;
			Vector3 min = new(float.MaxValue, float.MaxValue, float.MaxValue);
			Vector3 max = new(float.MinValue, float.MinValue, float.MinValue);

			foreach (var member in members)
			{
				if (!member.IsAlive)
					continue;

				hasAny = true;
				Vector3 pos = member.transform.position;

				if (pos.x < min.x) min.x = pos.x;
				if (pos.y < min.y) min.y = pos.y;
				if (pos.z < min.z) min.z = pos.z;

				if (pos.x > max.x) max.x = pos.x;
				if (pos.y > max.y) max.y = pos.y;
				if (pos.z > max.z) max.z = pos.z;
			}

			if (hasAny)
				partyCenter.position = (min + max) * 0.5f;
		}
	}
}