namespace Midevil.Effect
{
	public class BurnOnHitEffect : Effect, IOnHit
	{
		public string effectId;
		public float effectDps;
		public float effectDuration;

		public void OnHit(Character owner, Character target, float damage)
		{
			target.AddEffect(new BurningEffect(effectId, effectDps, effectDuration));
		}
	}
}