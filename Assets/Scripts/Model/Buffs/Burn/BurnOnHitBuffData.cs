using UnityEngine;

[CreateAssetMenu(menuName = "Buff/Burn On Hit")]
public class BurnOnHitBuffData : BuffData
{
	public string effectId = "burning";
	public float effectDps = 3f;
	public float effectDuration = 3f;

	public override Buff CreateRuntime()
	{
		return new BurnOnHitBuff
		{
			id = id,
			stats = stats,
			duration = duration,
			effectId = effectId,
			effectDps = effectDps,
			effectDuration = effectDuration
		};
	}
}
