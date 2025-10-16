using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class Stats
{
	public float health;
	public float maxHealth;
	public float levelHeal;
	public float damage;
	public float attackSpeed;
	public float critChance;
	public float critDamage;
	public float range;
	public float moveSpeed;

	public void Recalculate(Stats baseStats, List<Buff> buffs)
	{
		var newMaxHealth = baseStats.maxHealth + buffs.Sum(b => b.stats.maxHealth);

		health = Math.Min(health + (newMaxHealth - maxHealth), newMaxHealth);
		maxHealth = newMaxHealth;
		levelHeal = baseStats.levelHeal + buffs.Sum(b => b.stats.levelHeal);
		damage = baseStats.damage + buffs.Sum(b => b.stats.damage);
		attackSpeed = baseStats.attackSpeed + buffs.Sum(b => b.stats.attackSpeed);
		critChance = baseStats.critChance + buffs.Sum(b => b.stats.critChance);
		critDamage = baseStats.critDamage + buffs.Sum(b => b.stats.critDamage);
		range = baseStats.range + buffs.Sum(b => b.stats.range);
		moveSpeed = baseStats.moveSpeed + buffs.Sum(b => b.stats.moveSpeed);
	}
}