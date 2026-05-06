using Midevil.Boons;
using UnityEngine;

namespace Midevil.Mission
{
	public class ChaosMissionRunner : IMissionRunner
	{
		public string ObjectiveText => $"Chaos — Endure {FormatTime(timeRemaining)}";
		public bool IsEndless => true;
		public bool IsReadyForNextWave => true;

		private SpawnManager ctx;
		private MissionConfig cfg;
		private float timeRemaining;
		private float boonTimer;
		private int beatIndex;
		private bool ended;
		private bool victory;

		public void Begin(SpawnManager ctx, MissionConfig cfg)
		{
			this.ctx = ctx;
			this.cfg = cfg;
			timeRemaining = cfg.chaosTimerSeconds;
			boonTimer = cfg.chaosBoonInterval;
		}

		public void Tick(float dt)
		{
			if (ended) return;

			timeRemaining -= dt;
			boonTimer -= dt;

			UiManager.Instance.SetTimerText(timeRemaining);

			if (boonTimer <= 0f)
			{
				boonTimer = cfg.chaosBoonInterval;
				beatIndex++;
				RunBoonManager.Instance?.OfferBoons(beatIndex);
			}

			if (timeRemaining <= 0f)
			{
				timeRemaining = 0f;
				ended = true;
				victory = true;
			}
		}

		public void OnEnemyDied(Character enemy) { }
		public void OnWaveCleared() { }

		public bool ShouldEndRun(out bool victory)
		{
			victory = this.victory;
			return ended;
		}

		private static string FormatTime(float seconds)
		{
			seconds = Mathf.Max(0f, seconds);
			int m = Mathf.FloorToInt(seconds / 60f);
			int s = Mathf.FloorToInt(seconds % 60f);
			return $"{m}:{s:00}";
		}
	}
}
