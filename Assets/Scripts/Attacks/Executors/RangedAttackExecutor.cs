using UnityEngine;

public class RangedAttackExecutor : IAttackExecutor
{
	public void ExecuteAttack(AttackContext context)
	{
		var attacker = context.attacker;
		var target = context.target;

		attacker.transform.LookAt(target.transform);

		var projectile = GameObject.Instantiate(attacker.stats.projectile, attacker.equipment.weaponPos.position, Quaternion.identity);
		projectile.transform.LookAt(target.targetPos);

		projectile.Setup(attacker.stats, attacker);
	}
}
