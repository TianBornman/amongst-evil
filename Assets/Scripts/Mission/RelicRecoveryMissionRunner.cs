using UnityEngine;

namespace Midevil.Mission
{
	public class RelicRecoveryMissionRunner : IMissionRunner
	{
		public string ObjectiveText => relicTouched ? "Relic Recovered" : "Find the Relic";
		public bool IsEndless => true;

		private SpawnManager ctx;
		private MissionConfig cfg;
		private bool relicTouched;
		private bool ended;
		private bool victory;

		public void Begin(SpawnManager ctx, MissionConfig cfg)
		{
			this.ctx = ctx;
			this.cfg = cfg;

			ctx.PlaceRelic(OnRelicTouched);
		}

		public void Tick(float dt) { }
		public void OnEnemyDied(Character enemy) { }
		public void OnWaveCleared() { }

		public bool ShouldEndRun(out bool victory)
		{
			victory = this.victory;
			return ended;
		}

		private void OnRelicTouched()
		{
			if (relicTouched) return;
			relicTouched = true;
			ended = true;
			victory = true;
		}
	}
}
