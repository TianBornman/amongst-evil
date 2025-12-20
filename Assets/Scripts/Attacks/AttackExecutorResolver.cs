using Midevil.Item;

public static class AttackExecutorResolver
{
	private static readonly IAttackExecutor melee  = new MeleeAttackExecutor();
	private static readonly IAttackExecutor ranged = new RangedAttackExecutor();

	public static IAttackExecutor Resolve(ItemStats stats)
	{
		if (stats == null)
			return melee;

		return stats.attackType switch
		{
			AttackType.Melee => melee,
			AttackType.Ranged => ranged,
			_ => melee
		};
	}
}
