using Midevil.Boons;
using UnityEngine;

namespace Midevil.Mission
{
	public class PurgeMissionRunner : IMissionRunner
	{
		public string ObjectiveText => InBreath
			? $"Breath — {Mathf.CeilToInt(breathRemaining)}s"
			: $"Purge — Wave {ctx.currentWave}/{cfg.waveCount}";

		public bool IsEndless => false;
		public bool IsReadyForNextWave => !InBreath;

		private SpawnManager ctx;
		private MissionConfig cfg;
		private bool ended;
		private bool victory;
		private float breathRemaining;
		private int beatIndex;

		private bool InBreath => breathRemaining > 0f;

		public void Begin(SpawnManager ctx, MissionConfig cfg)
		{
			this.ctx = ctx;
			this.cfg = cfg;
		}

		public void Tick(float dt)
		{
			if (!InBreath) return;

			breathRemaining -= dt;
			UiManager.Instance.SetObjectiveText(ObjectiveText);

			if (breathRemaining <= 0f)
			{
				breathRemaining = 0f;
				UiManager.Instance.SetObjectiveText(ObjectiveText);
				RunBoonManager.Instance?.NotifyBreathEnded();
			}
		}

		public void OnEnemyDied(Character enemy) { }

		public void OnWaveCleared()
		{
			if (ctx.currentWave >= cfg.waveCount)
			{
				ended = true;
				victory = true;
				return;
			}

			breathRemaining = Mathf.Max(0.1f, cfg.breathSeconds);
			beatIndex++;

			RunBoonManager.Instance?.OfferBoons(beatIndex);
		}

		public bool ShouldEndRun(out bool victory)
		{
			victory = this.victory;
			return ended;
		}
	}
}
