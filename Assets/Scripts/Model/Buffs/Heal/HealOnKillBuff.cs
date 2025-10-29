using System;

[Serializable]
public class HealOnKillBuff : Buff, IOnKill
{
	public string effectId;
	public float healAmount = 2;

	public void OnKill(Character owner, Character killer)
	{
		killer.Damage(-healAmount);
	}
}
