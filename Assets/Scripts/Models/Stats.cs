using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Stats
{
	public int level = 1;
	public float currentXp;
	public float neededXp;
	public float health;
	public float maxHealth;
	public float levelHeal;
	public float blockChance;
	public float dodgeChance;
	public float damage;
	public float attackSpeed;
	public float castSpeed;
	public float critChance;
	public float critDamage;
	public float range;
	public float moveSpeed;
	public float xpValue;

	public void Recalculate(Stats baseStats, List<Buff> buffs)
	{
		var newMaxHealth = Scale(baseStats.maxHealth, buffs.Sum(b => b.stats.maxHealth));

		health = Math.Min(health + (newMaxHealth - maxHealth), newMaxHealth);
		maxHealth = newMaxHealth;
		levelHeal = baseStats.levelHeal + buffs.Sum(b => b.stats.levelHeal);
		blockChance = baseStats.blockChance + buffs.Sum(b => b.stats.blockChance);
		dodgeChance = baseStats.dodgeChance + buffs.Sum(b => b.stats.dodgeChance);
		damage = Scale(baseStats.damage, buffs.Sum(b => b.stats.damage));
		attackSpeed = baseStats.attackSpeed + buffs.Sum(b => b.stats.attackSpeed);
		castSpeed = baseStats.castSpeed + buffs.Sum(b => b.stats.castSpeed);
		critChance = baseStats.critChance + buffs.Sum(b => b.stats.critChance);
		critDamage = baseStats.critDamage + buffs.Sum(b => b.stats.critDamage);
		range = baseStats.range + buffs.Sum(b => b.stats.range);
		moveSpeed = baseStats.moveSpeed + buffs.Sum(b => b.stats.moveSpeed);
		xpValue = Scale(baseStats.xpValue, buffs.Sum(b => b.stats.xpValue));
	}

	private float scalingFactor = 1.10f;
	private float Scale(float baseValue, float buffSum) => (baseValue + buffSum) * Mathf.Pow(scalingFactor, level);
}