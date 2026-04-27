namespace Midevil.Mission
{
	public interface IMissionRunner
	{
		string ObjectiveText { get; }
		bool IsEndless { get; }

		void Begin(SpawnManager ctx, MissionConfig cfg);
		void Tick(float dt);
		void OnEnemyDied(Character enemy);
		void OnWaveCleared();
		bool ShouldEndRun(out bool victory);
	}
}
