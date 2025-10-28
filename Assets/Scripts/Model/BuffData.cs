using UnityEngine;

public abstract class BuffData : ScriptableObject
{
	public string id;
	public Stats stats;
	public float duration = -1;

	public abstract Buff CreateRuntime();
}