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
	public float size;
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
	public float scalingFactor = 1.10f;

	public void Recalculate(Stats baseStats, List<Buff> buffs)
	{
		var newMaxHealth = Scale(baseStats.maxHealth + buffs.Sum(b => b.stats.maxHealth));

		health = Math.Min(health + (newMaxHealth - maxHealth), newMaxHealth);
		maxHealth = newMaxHealth;
		size = baseStats.size + (buffs.Sum(b => b.stats.size));
		levelHeal = baseStats.levelHeal + buffs.Sum(b => b.stats.levelHeal);
		blockChance = baseStats.blockChance + buffs.Sum(b => b.stats.blockChance);
		dodgeChance = baseStats.dodgeChance + buffs.Sum(b => b.stats.dodgeChance);
		damage = baseStats.damage + buffs.Sum(b => b.stats.damage);
		attackSpeed = baseStats.attackSpeed + buffs.Sum(b => b.stats.attackSpeed);
		castSpeed = baseStats.castSpeed + buffs.Sum(b => b.stats.castSpeed);
		critChance = baseStats.critChance + buffs.Sum(b => b.stats.critChance);
		critDamage = baseStats.critDamage + buffs.Sum(b => b.stats.critDamage);
		range = baseStats.range + buffs.Sum(b => b.stats.range);
		moveSpeed = baseStats.moveSpeed + buffs.Sum(b => b.stats.moveSpeed);
		xpValue = baseStats.xpValue + buffs.Sum(b => b.stats.xpValue);

		ApplyLevelModifiers();
		ApplySizeModifiers();
	}

	private float Scale(float value) => value * Mathf.Pow(scalingFactor, level);

	private void ApplyLevelModifiers()
	{
		damage = Scale(damage);

		xpValue = Scale(xpValue);
	}

	private void ApplySizeModifiers()
	{
		// Normalize size around 1.0
		float t = Mathf.InverseLerp(0.7f, 2.0f, size);

		float attackMultiplier = Mathf.Lerp(1.15f, 0.80f, t);
		attackSpeed *= attackMultiplier;

		float moveMultiplier = Mathf.Lerp(1.25f, 0.70f, t);
		moveSpeed *= moveMultiplier;

		if (size < 1f)
			dodgeChance += (1f - size) * 10f;
		else
			dodgeChance -= (size - 1f) * 8f;

		blockChance += (size - 1f) * 12f;
	}
}