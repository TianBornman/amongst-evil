using Midevil.Boons;
using UnityEngine;

namespace Midevil.Mission
{
	public class RelicRecoveryMissionRunner : IMissionRunner
	{
		public string ObjectiveText => relicTouched ? "Relic Recovered" : "Find the Relic";
		public bool IsEndless => true;
		public bool IsReadyForNextWave => true;

		private SpawnManager ctx;
		private MissionConfig cfg;
		private bool relicTouched;
		private bool ended;
		private bool victory;
		private int killsSinceLastBoon;
		private int beatIndex;

		public void Begin(SpawnManager ctx, MissionConfig cfg)
		{
			this.ctx = ctx;
			this.cfg = cfg;

			ctx.PlaceRelic(OnRelicTouched);
		}

		public void Tick(float dt) { }

		public void OnEnemyDied(Character enemy)
		{
			if (cfg.relicBoonKillInterval <= 0) return;

			killsSinceLastBoon++;
			if (killsSinceLastBoon >= cfg.relicBoonKillInterval)
			{
				killsSinceLastBoon = 0;
				beatIndex++;
				RunBoonManager.Instance?.OfferBoons(beatIndex);
			}
		}

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
