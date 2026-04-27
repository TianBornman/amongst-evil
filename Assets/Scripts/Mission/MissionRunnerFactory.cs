namespace Midevil.Mission
{
	public static class MissionRunnerFactory
	{
		public static IMissionRunner Create(MissionType type) => type switch
		{
			MissionType.Purge => new PurgeMissionRunner(),
			MissionType.Chaos => new ChaosMissionRunner(),
			MissionType.RelicRecovery => new RelicRecoveryMissionRunner(),
			_ => new PurgeMissionRunner()
		};
	}
}
