using UnityEngine;

namespace Midevil.Effect
{
	public class BurningEffect : Effect, IOnTick
	{
		public float dps;
		private float tickInterval = 0.5f;
		private float tickTimer;

		public void Tick(Character owner, float deltaTime)
		{
			tickTimer -= deltaTime;
			if (tickTimer <= 0f)
			{
				tickTimer = tickInterval;
				float damage = dps * tickInterval;
				owner.Damage(owner, damage);
			}
		}
	}
}
