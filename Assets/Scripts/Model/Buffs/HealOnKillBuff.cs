using System;

[Serializable]
public class HealOnKillBuff : Buff, IOnDeath
{
	public float healAmount = 2;

	public void OnDeath(Character owner, Character killer)
	{
		killer.Damage(-healAmount);
	}
}
