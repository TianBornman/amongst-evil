namespace Midevil.Effect
{
	public class HealOnKillEffect : Effect, IOnKill
	{
		public float healAmount = 2f;

		public void OnKill(Character owner, Character killer)
		{
			killer.Heal(healAmount);
		}
	}
}
