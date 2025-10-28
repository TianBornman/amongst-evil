public class BurnOnHitBuff : Buff, IOnHit
{
	public string effectId;
	public float effectDps;
	public float effectDuration;

	public void OnHit(Character owner, Character target, float damage)
	{
		target.AddBuff(new BurningBuff(effectId, effectDps, effectDuration));
	}
}
