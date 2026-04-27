using System;

namespace Midevil.Effect
{
	public class StatModifierEffect : Effect
	{
		public Stats statDelta;
		private Buff appliedBuff;

		public override void OnApply(Character owner)
		{
			appliedBuff = new Buff
			{
				id = Guid.NewGuid(),
				stats = statDelta
			};
			owner.AddBuff(appliedBuff);
		}

		public override void OnRemove(Character owner)
		{
			if (appliedBuff != null)
				owner.RemoveBuff(appliedBuff);
		}
	}
}
