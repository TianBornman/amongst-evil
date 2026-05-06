namespace Midevil.Mission
{
	public interface IMissionRunner
	{
		string ObjectiveText { get; }
		bool IsEndless { get; }

		// SpawnManager polls this between waves. Returning false stalls the wave loop
		// (e.g. Purge uses this for the Breath lull between waves).
		bool IsReadyForNextWave { get; }

		void Begin(SpawnManager ctx, MissionConfig cfg);
		void Tick(float dt);
		void OnEnemyDied(Character enemy);
		void OnWaveCleared();
		bool ShouldEndRun(out bool victory);
	}
}
