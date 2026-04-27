namespace Midevil.Mission
{
	public class PurgeMissionRunner : IMissionRunner
	{
		public string ObjectiveText => $"Purge — Wave {ctx.currentWave}/{cfg.waveCount}";
		public bool IsEndless => false;

		private SpawnManager ctx;
		private MissionConfig cfg;
		private bool ended;
		private bool victory;

		public void Begin(SpawnManager ctx, MissionConfig cfg)
		{
			this.ctx = ctx;
			this.cfg = cfg;
		}

		public void Tick(float dt) { }

		public void OnEnemyDied(Character enemy) { }

		public void OnWaveCleared()
		{
			if (ctx.currentWave >= cfg.waveCount)
			{
				ended = true;
				victory = true;
			}
		}

		public bool ShouldEndRun(out bool victory)
		{
			victory = this.victory;
			return ended;
		}
	}
}
