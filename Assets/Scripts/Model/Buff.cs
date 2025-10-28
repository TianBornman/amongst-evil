using System;
using UnityEngine;

[Serializable]
public class Buff
{
	public Stats stats;

	// Effect Stats
	public string id;
	public float duration = -1f;
	protected float elapsed;

	public virtual void OnApply(Character owner) { }
	public virtual void OnRemove(Character owner) { }

	public virtual bool RefreshOrStack(Buff existingBuff) { return false; }

	public bool IsExpired => duration > 0 && elapsed >= duration;


	public virtual void TickTimer(float deltaTime)
	{
		if (duration > 0) elapsed += deltaTime;
	}
}