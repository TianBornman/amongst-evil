using System;

namespace Midevil.Ability
{
	[Serializable]
	public class AbilitySlot
	{
		public int slotIndex;
		public Guid abilityId;
		public PartyCharacter character;

		public bool HasAbility => abilityId != Guid.Empty;

		public void TryUseAbility()
		{
			if (character != null)
				character.abilities.TryUseAbility(abilityId);
		}

		public void Clear()
		{
			abilityId = Guid.Empty;
			character = null;
			UiManager.Instance.ClearAbility(slotIndex);
		}
	}
}