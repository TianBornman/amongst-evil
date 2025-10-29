using UnityEngine;

[CreateAssetMenu(menuName = "Buff/Heal On Kill")]
public class HealOnKillBuffData : BuffData
{
	public string effectId = "healonkill";
	public float healAmount;

	public override Buff CreateRuntime()
	{
		return new HealOnKillBuff
		{
			id = id,
			stats = stats,
			duration = duration,
			effectId = effectId,
			healAmount = healAmount
		};
	}
}
