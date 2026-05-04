using System.Collections.Generic;
using UnityEngine;

public static class EnemyPool
{
	private static readonly Dictionary<Character, Stack<PooledEnemy>> pools = new();

	public static Character Get(Character prefab, Vector3 position, Quaternion rotation, Transform parent = null)
	{
		if (prefab == null) return null;

		if (pools.TryGetValue(prefab, out var stack))
		{
			while (stack.Count > 0)
			{
				var pooled = stack.Pop();
				if (pooled == null) continue;
				pooled.OnGetFromPool(position, rotation);
				return pooled.GetComponent<Character>();
			}
		}

		var instance = Object.Instantiate(prefab, position, rotation, parent);
		var pe = instance.GetComponent<PooledEnemy>();
		if (pe == null) pe = instance.gameObject.AddComponent<PooledEnemy>();
		pe.originalPrefab = prefab;
		pe.CaptureSnapshot();
		return instance;
	}

	public static void Return(PooledEnemy pooled)
	{
		if (pooled == null) return;
		var prefab = pooled.originalPrefab;
		if (prefab == null) { Object.Destroy(pooled.gameObject); return; }

		pooled.OnReturnToPool();

		if (!pools.TryGetValue(prefab, out var stack))
		{
			stack = new Stack<PooledEnemy>();
			pools[prefab] = stack;
		}
		stack.Push(pooled);
	}

	public static void Clear()
	{
		foreach (var stack in pools.Values)
			while (stack.Count > 0)
			{
				var pe = stack.Pop();
				if (pe != null) Object.Destroy(pe.gameObject);
			}
		pools.Clear();
	}
}
