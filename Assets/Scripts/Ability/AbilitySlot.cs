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
			character.TryUseAbility(abilityId);
		}

		public void Clear()
		{
			UiManager.Instance.ClearAbility(slotIndex + 1);
		}
	}
}