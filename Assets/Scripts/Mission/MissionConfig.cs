using UnityEngine;

namespace Midevil.Mission
{
	public class MissionConfig
	{
		public Mission mission;

		public int waveCount;
		public int baseEnemyCount;
		public int enemyCountScalingPerWave;
		public float healthMul;
		public float damageMul;
		public float spawnIntervalMul;
		public float chaosTimerSeconds;

		public static MissionConfig Build(Mission mission)
		{
			int d = (int)mission.difficulty;
			float t = (d - 1) / 9f;

			return new MissionConfig
			{
				mission = mission,
				waveCount = 4 + 2 * d,
				baseEnemyCount = 15 + 5 * d,
				enemyCountScalingPerWave = 8 + d,
				healthMul = 1f + 0.20f * (d - 1),
				damageMul = 1f + 0.15f * (d - 1),
				spawnIntervalMul = Mathf.Lerp(1.0f, 0.5f, t),
				chaosTimerSeconds = 120f + 30f * d
			};
		}
	}
}
