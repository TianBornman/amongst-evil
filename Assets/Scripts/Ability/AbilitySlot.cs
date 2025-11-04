using System;

namespace Midevil.Ability
{
	[Serializable]
	public class AbilitySlot
	{
		public int slotIndex;
		public Ability assignedAbility;

		public bool HasAbility => assignedAbility != null;

		public void Clear()
		{
			assignedAbility = null;
			UiManager.Instance.ClearAbility(slotIndex + 1);
		}
	}
}