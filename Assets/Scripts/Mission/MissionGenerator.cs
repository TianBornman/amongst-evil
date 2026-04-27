using System.Collections.Generic;
using UnityEngine;

namespace Midevil.Mission
{
	public static class MissionGenerator
	{
		private static readonly string[] PurgeTitles =
		{
			"Clear the Rift",
			"Purify the Cursed Site",
			"Burn the Nest",
			"Silence the Choir",
			"Cleanse the Hollow"
		};

		private static readonly string[] RelicTitles =
		{
			"Recover the Sealed Tome",
			"Retrieve the Warden's Sigil",
			"Unearth the Bloodbound Reliquary",
			"Reclaim the Vermilion Mark"
		};

		private static readonly string[] ChaosTitles =
		{
			"The Grand Toll",
			"Last Stand at the Veil",
			"Blood Tithe",
			"Hour of the Hourglass"
		};

		private static readonly string[] PurgeFlavor =
		{
			"A rift has bloomed where the Curse pools deepest. Survive its waves until the wound closes.",
			"The cursed site festers. The Brotherhood demands it burned to silence."
		};

		private static readonly string[] RelicFlavor =
		{
			"A relic stirs in the dark. Find it, defeat its Warden, and carry it home.",
			"The Brotherhood has marked this ground. Something sacred waits beneath."
		};

		private static readonly string[] ChaosFlavor =
		{
			"No objective. No mercy. Endure until the bell falls quiet.",
			"The Grand Clock bleeds heavy here. Stand if you can."
		};

		public static List<Mission> GenerateBatch(int count, int minDifficulty = 1, int maxDifficulty = 3)
		{
			var batch = new List<Mission>(count);

			for (int i = 0; i < count; i++)
				batch.Add(Generate(minDifficulty, maxDifficulty));

			return batch;
		}

		public static Mission Generate(int minDifficulty = 1, int maxDifficulty = 3)
		{
			var type = (MissionType)Random.Range(0, System.Enum.GetValues(typeof(MissionType)).Length);
			var difficulty = (MissionDifficulty)Random.Range(minDifficulty, maxDifficulty + 1);

			string title = type switch
			{
				MissionType.Purge => Pick(PurgeTitles),
				MissionType.RelicRecovery => Pick(RelicTitles),
				MissionType.Chaos => Pick(ChaosTitles),
				_ => "Unknown Mission"
			};

			string flavor = type switch
			{
				MissionType.Purge => Pick(PurgeFlavor),
				MissionType.RelicRecovery => Pick(RelicFlavor),
				MissionType.Chaos => Pick(ChaosFlavor),
				_ => string.Empty
			};

			return new Mission(title, type, difficulty, flavor);
		}

		private static string Pick(string[] pool) => pool[Random.Range(0, pool.Length)];
	}
}
