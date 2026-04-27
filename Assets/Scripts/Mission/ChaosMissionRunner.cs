using UnityEngine;

namespace Midevil.Mission
{
	public class ChaosMissionRunner : IMissionRunner
	{
		public string ObjectiveText => $"Chaos — Endure {FormatTime(timeRemaining)}";
		public bool IsEndless => true;

		private SpawnManager ctx;
		private MissionConfig cfg;
		private float timeRemaining;
		private bool ended;
		private bool victory;

		public void Begin(SpawnManager ctx, MissionConfig cfg)
		{
			this.ctx = ctx;
			this.cfg = cfg;
			timeRemaining = cfg.chaosTimerSeconds;
		}

		public void Tick(float dt)
		{
			if (ended) return;

			timeRemaining -= dt;

			UiManager.Instance.SetTimerText(timeRemaining);

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
