using Midevil.Effect;

public class MeleeAttackExecutor : IAttackExecutor
{
	public void ExecuteAttack(AttackContext context)
	{
		var target = context.target;
		var attacker = context.attacker;

		if (target == null || !target.IsAlive)
			attacker.SetState(new CombatMoveState(attacker));

		foreach (var buff in attacker.currentEffects)
			if (buff is IOnHit onHit)
				onHit.OnHit(attacker, target, attacker.stats.damage);

		attacker.transform.LookAt(target.transform);
		target.Damage(attacker, attacker.stats.damage, attacker.stats.critChance, attacker.stats.critDamage);
	}
}
