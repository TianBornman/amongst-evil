using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Character))]
public class PooledEnemy : MonoBehaviour
{
	[HideInInspector] public Character originalPrefab;

	private Character character;
	private Stats baseStatsSnapshot;
	private bool snapshotTaken;
	private Coroutine returnRoutine;

	private void Awake()
	{
		character = GetComponent<Character>();
	}

	public void CaptureSnapshot()
	{
		if (snapshotTaken) return;
		baseStatsSnapshot = CloneStats(character.baseStats);
		snapshotTaken = true;
	}

	public void RestoreSnapshot()
	{
		if (!snapshotTaken) return;
		CopyStats(baseStatsSnapshot, character.baseStats);
	}

	public void ScheduleReturn(float delay)
	{
		if (returnRoutine != null) StopCoroutine(returnRoutine);
		returnRoutine = StartCoroutine(ReturnAfter(delay));
	}

	private IEnumerator ReturnAfter(float delay)
	{
		yield return new WaitForSeconds(delay);
		EnemyPool.Return(this);
	}

	public void OnGetFromPool(Vector3 position, Quaternion rotation)
	{
		if (returnRoutine != null) { StopCoroutine(returnRoutine); returnRoutine = null; }

		var agent = character.agent;
		if (agent != null && agent.enabled)
		{
			agent.isStopped = true;
			agent.ResetPath();
			agent.enabled = false;
		}

		transform.SetPositionAndRotation(position, rotation);

		if (agent != null)
		{
			agent.enabled = true;
			if (NavMesh.SamplePosition(position, out var hit, 5f, NavMesh.AllAreas))
				agent.Warp(hit.position);
		}

		RestoreSnapshot();

		character.buffs.Clear();
		character.currentEffects.Clear();
		character.targets.Clear();
		character.target = null;

		character.stats.health = character.baseStats.maxHealth;

		if (character.animator != null)
		{
			character.animator.Rebind();
			character.animator.Update(0f);
		}

		gameObject.SetActive(true);

		character.OnPooledRespawn();
	}

	public void OnReturnToPool()
	{
		if (returnRoutine != null) { StopCoroutine(returnRoutine); returnRoutine = null; }

		var agent = character.agent;
		if (agent != null && agent.enabled)
		{
			agent.isStopped = true;
			agent.ResetPath();
			agent.enabled = false;
		}

		character.targets.Clear();
		character.target = null;

		gameObject.SetActive(false);
	}

	private static Stats CloneStats(Stats source)
	{
		var clone = new Stats();
		CopyStats(source, clone);
		return clone;
	}

	private static void CopyStats(Stats from, Stats to)
	{
		to.level = from.level;
		to.currentXp = from.currentXp;
		to.neededXp = from.neededXp;
		to.health = from.health;
		to.maxHealth = from.maxHealth;
		to.size = from.size;
		to.levelHeal = from.levelHeal;
		to.blockChance = from.blockChance;
		to.dodgeChance = from.dodgeChance;
		to.damage = from.damage;
		to.attackSpeed = from.attackSpeed;
		to.castSpeed = from.castSpeed;
		to.critChance = from.critChance;
		to.critDamage = from.critDamage;
		to.range = from.range;
		to.moveSpeed = from.moveSpeed;
		to.xpValue = from.xpValue;
		to.scalingFactor = from.scalingFactor;
		to.projectile = from.projectile;
	}
}
