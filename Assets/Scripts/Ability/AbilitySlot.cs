using System;

namespace Midevil.Ability
{
	[Serializable]
	public class AbilitySlot
	{
		public int slotIndex;
		public Ability assignedAbility;

		public bool HasAbility => assignedAbility != null;
	}
}