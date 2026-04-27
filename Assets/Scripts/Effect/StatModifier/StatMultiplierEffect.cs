namespace Midevil.Effect
{
	public class StatMultiplierEffect : Effect
	{
		public float maxHealthMultiplier = 1f;
		public float damageMultiplier = 1f;
		public float moveSpeedMultiplier = 1f;
		public float attackSpeedMultiplier = 1f;
		public float sizeMultiplier = 1f;
		public float xpValueMultiplier = 1f;

		public override void OnApply(Character owner)
		{
			Apply(owner, +1);
		}

		public override void OnRemove(Character owner)
		{
			Apply(owner, -1);
		}

		private void Apply(Character owner, int direction)
		{
			float h = direction > 0 ? maxHealthMultiplier  : 1f / maxHealthMultiplier;
			float d = direction > 0 ? damageMultiplier     : 1f / damageMultiplier;
			float m = direction > 0 ? moveSpeedMultiplier  : 1f / moveSpeedMultiplier;
			float a = direction > 0 ? attackSpeedMultiplier: 1f / attackSpeedMultiplier;
			float s = direction > 0 ? sizeMultiplier       : 1f / sizeMultiplier;
			float x = direction > 0 ? xpValueMultiplier    : 1f / xpValueMultiplier;

			owner.baseStats.maxHealth   *= h;
			owner.baseStats.damage      *= d;
			owner.baseStats.moveSpeed   *= m;
			owner.baseStats.attackSpeed *= a;
			owner.baseStats.size        *= s;
			owner.baseStats.xpValue     *= x;

			owner.RecalculateStats();
		}
	}
}
