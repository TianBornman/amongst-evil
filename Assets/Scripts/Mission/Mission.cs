namespace Midevil.Mission
{
	[System.Serializable]
	public class Mission
	{
		public string title;
		public MissionType type;
		public MissionDifficulty difficulty;
		public string flavorText;

		public Mission(string title, MissionType type, MissionDifficulty difficulty, string flavorText)
		{
			this.title = title;
			this.type = type;
			this.difficulty = difficulty;
			this.flavorText = flavorText;
		}
	}
}
