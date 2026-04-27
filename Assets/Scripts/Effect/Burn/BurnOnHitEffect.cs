namespace Midevil.Effect
{
	public class BurnOnHitEffect : Effect, IOnHit
	{
		public EffectData burningEffect;

		public void OnHit(Character owner, Character target, float damage)
		{
			if (burningEffect == null || target == null) return;
			target.effects.AddEffect(burningEffect.CreateRuntime());
		}
	}
}
