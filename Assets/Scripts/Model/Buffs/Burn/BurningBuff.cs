using UnityEngine;

public class BurningBuff : Buff, IOnTick
{
	private float dps;
	private float tickInterval = 0.5f;
	private float tickTimer;

	public BurningBuff(string id, float dps, float duration)
	{
		stats = new Stats();

		this.id = id;
		this.dps = dps;
		this.duration = duration;
	}

	public void Tick(Character owner, float deltaTime)
	{
		tickTimer -= deltaTime;
		if (tickTimer <= 0f)
		{
			tickTimer = tickInterval;
			float damage = dps * tickInterval;
			owner.Damage(damage);
		}
	}

	public override bool RefreshOrStack(Buff existingBuff)
	{
		var burningBuff = existingBuff as BurningBuff;

		if (burningBuff == null)
			return false;

		duration = Mathf.Max(duration, burningBuff.duration);
		elapsed = 0;

		return true;
	}
}
