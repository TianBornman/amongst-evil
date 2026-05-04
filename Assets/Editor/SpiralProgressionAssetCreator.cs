#if UNITY_EDITOR
using Midevil.Mission;
using Midevil.Progression;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class SpiralProgressionAssetCreator
{
	private const string Root = "Assets/Data/Progression";
	private const string RanksDir = Root + "/Ranks";
	private const string ReqsDir = Root + "/Requirements";

	[MenuItem("Tools/Progression/Create Spiral Progression Assets")]
	public static void CreateAll()
	{
		EnsureFolder(Root);
		EnsureFolder(RanksDir);
		EnsureFolder(ReqsDir);

		var rewardTable = LoadOrCreate<StandingRewardTable>($"{Root}/Standing Reward Table.asset");
		EditorUtility.SetDirty(rewardTable);

		var spiral = LoadOrCreate<SpiralProgression>($"{Root}/Spiral.asset");
		spiral.ranks = new List<SectRankData>();

		spiral.ranks.Add(BuildRank1());
		spiral.ranks.Add(BuildRank2());
		spiral.ranks.Add(BuildRank3());
		spiral.ranks.Add(BuildRank4());
		spiral.ranks.Add(BuildRank5());
		spiral.ranks.Add(BuildRank6());
		spiral.ranks.Add(BuildRank7());
		spiral.ranks.Add(BuildRank8());
		spiral.ranks.Add(BuildRank9());
		spiral.ranks.Add(BuildRank10());

		EditorUtility.SetDirty(spiral);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		Selection.activeObject = spiral;
		EditorGUIUtility.PingObject(spiral);
		Debug.Log($"Spiral progression authored at {Root}/Spiral.asset. Drag this onto SectProgressManager.spiral, and the reward table onto SectProgressManager.rewardTable.");
	}

	private static SectRankData BuildRank1()
	{
		var rank = LoadOrCreate<SectRankData>($"{RanksDir}/Rank 01 - Whispering Ashes.asset");
		rank.rankNumber = 1;
		rank.rankName = "Whispering Ashes";
		rank.statusTitle = "Unrecognised / Aspirants";
		rank.flavor = "Your Creed exists only as rumour. Survival and small deeds are all that mark your beginning.";
		rank.quote = "Smoke in the wind, whispers that may never be heard.";
		rank.requirements = new List<RankRequirement>();
		EditorUtility.SetDirty(rank);
		return rank;
	}

	private static SectRankData BuildRank2()
	{
		var rank = LoadOrCreate<SectRankData>($"{RanksDir}/Rank 02 - Shrouded Vow.asset");
		rank.rankNumber = 2;
		rank.rankName = "Shrouded Vow";
		rank.statusTitle = "Initiates / Recognised";
		rank.flavor = "The Brotherhood has noted you in hidden records. The Rite of Names is performed.";
		rank.quote = "Our names are known, but our faces remain unseen.";
		rank.requirements = new List<RankRequirement>
		{
			Standing("Rank 02 - Standing", 50, "Standing", "Earn the Brotherhood's notice.")
		};
		EditorUtility.SetDirty(rank);
		return rank;
	}

	private static SectRankData BuildRank3()
	{
		var rank = LoadOrCreate<SectRankData>($"{RanksDir}/Rank 03 - Iron Phalanx.asset");
		rank.rankNumber = 3;
		rank.rankName = "Iron Phalanx";
		rank.statusTitle = "Bound to the Purpose";
		rank.flavor = "Your Creed acts as a single unit. Reputation quietly spreads.";
		rank.quote = "Bound by steel and oath, we move as one purpose.";
		rank.requirements = new List<RankRequirement>
		{
			Standing("Rank 03 - Standing", 150, "Standing", "Prove your resolve in the field."),
			Kills("Rank 03 - Slay Zombies", "zombie", 25, "Slay Zombies", "Cull the risen dead.")
		};
		EditorUtility.SetDirty(rank);
		return rank;
	}

	private static SectRankData BuildRank4()
	{
		var rank = LoadOrCreate<SectRankData>($"{RanksDir}/Rank 04 - Veilforged.asset");
		rank.rankNumber = 4;
		rank.rankName = "Veilforged";
		rank.statusTitle = "Trusted Arm of the Brotherhood";
		rank.flavor = "Your Creed gains access to forbidden artefacts and high-risk rituals.";
		rank.quote = "Forged in shadow, we shape the unseen war.";
		rank.requirements = new List<RankRequirement>
		{
			Standing("Rank 04 - Standing", 400, "Standing", "Mounting deeds, mounting trust."),
			Missions("Rank 04 - Threat III Clears", null, MissionDifficulty.III, 3, "Clear Threat III Missions", "Survive at the upper edge of your tier.")
		};
		EditorUtility.SetDirty(rank);
		return rank;
	}

	private static SectRankData BuildRank5()
	{
		var rank = LoadOrCreate<SectRankData>($"{RanksDir}/Rank 05 - Eclipsed Sigil.asset");
		rank.rankNumber = 5;
		rank.rankName = "Eclipsed Sigil";
		rank.statusTitle = "Revered / Feared";
		rank.flavor = "Your Creed is a name whispered in awe. Relics begin to be catalogued.";
		rank.quote = "Our sigil casts shadows long enough to hide the world.";
		rank.requirements = new List<RankRequirement>
		{
			Standing("Rank 05 - Standing", 900, "Standing", ""),
			Missions("Rank 05 - Relic Recoveries", MissionType.RelicRecovery, MissionDifficulty.III, 3, "Recover Relics", "Three relics, intact.")
		};
		EditorUtility.SetDirty(rank);
		return rank;
	}

	private static SectRankData BuildRank6()
	{
		var rank = LoadOrCreate<SectRankData>($"{RanksDir}/Rank 06 - Nightbound Covenant.asset");
		rank.rankNumber = 6;
		rank.rankName = "Nightbound Covenant";
		rank.statusTitle = "Elite Operatives";
		rank.flavor = "You oversee strategic operations across multiple regions.";
		rank.quote = "We walk where the night binds the world.";
		rank.requirements = new List<RankRequirement>
		{
			Standing("Rank 06 - Standing", 1800, "Standing", ""),
			Missions("Rank 06 - Threat V Clears", null, MissionDifficulty.V, 5, "Clear Threat V Missions", "")
		};
		EditorUtility.SetDirty(rank);
		return rank;
	}

	private static SectRankData BuildRank7()
	{
		var rank = LoadOrCreate<SectRankData>($"{RanksDir}/Rank 07 - Shadowed Archive.asset");
		rank.rankNumber = 7;
		rank.rankName = "Shadowed Archive";
		rank.statusTitle = "Keepers of Forbidden Knowledge";
		rank.flavor = "Your Creed manages access to the Crypt of Knowledge.";
		rank.quote = "We hold the truths that the world dares not remember.";
		rank.requirements = new List<RankRequirement>
		{
			Standing("Rank 07 - Standing", 3500, "Standing", ""),
			Missions("Rank 07 - Threat VI Clears", null, MissionDifficulty.VI, 5, "Clear Threat VI Missions", "")
		};
		EditorUtility.SetDirty(rank);
		return rank;
	}

	private static SectRankData BuildRank8()
	{
		var rank = LoadOrCreate<SectRankData>($"{RanksDir}/Rank 08 - Obsidian Hand.asset");
		rank.rankNumber = 8;
		rank.rankName = "Obsidian Hand";
		rank.statusTitle = "Commanders of the Hidden Wars";
		rank.flavor = "The Council of Shadows takes note of your actions.";
		rank.quote = "Our grip unseen, yet the world bends beneath it.";
		rank.requirements = new List<RankRequirement>
		{
			Standing("Rank 08 - Standing", 6500, "Standing", ""),
			Missions("Rank 08 - Threat VII Clears", null, MissionDifficulty.VII, 5, "Clear Threat VII Missions", "")
		};
		EditorUtility.SetDirty(rank);
		return rank;
	}

	private static SectRankData BuildRank9()
	{
		var rank = LoadOrCreate<SectRankData>($"{RanksDir}/Rank 09 - Prime Obsidian.asset");
		rank.rankNumber = 9;
		rank.rankName = "Prime Obsidian";
		rank.statusTitle = "The Heart of the Shadows";
		rank.flavor = "Decisions made at this rank can shift the fate of nations.";
		rank.quote = "The clock does not strike without our hand guiding its hour.";
		rank.requirements = new List<RankRequirement>
		{
			Standing("Rank 09 - Standing", 12000, "Standing", ""),
			Missions("Rank 09 - Threat VIII Clears", null, MissionDifficulty.VIII, 5, "Clear Threat VIII Missions", "")
		};
		EditorUtility.SetDirty(rank);
		return rank;
	}

	private static SectRankData BuildRank10()
	{
		var rank = LoadOrCreate<SectRankData>($"{RanksDir}/Rank 10 - Clockless.asset");
		rank.rankNumber = 10;
		rank.rankName = "Clockless";
		rank.statusTitle = "Beyond the Veil / Mythic";
		rank.flavor = "Legends say the Clockless exist outside time itself.";
		rank.quote = "Time bends around those who walk beyond its measure.";
		rank.requirements = new List<RankRequirement>
		{
			Standing("Rank 10 - Standing", 25000, "Standing", ""),
			Milestone("Rank 10 - Witness the Grand Clock", "grand-clock-witnessed", 1, "Witness the Grand Clock", "An unmarked deed.")
		};
		EditorUtility.SetDirty(rank);
		return rank;
	}

	private static StandingRequirement Standing(string fileName, int amount, string label, string desc)
	{
		var req = LoadOrCreate<StandingRequirement>($"{ReqsDir}/{fileName}.asset");
		req.amount = amount;
		req.label = label;
		req.description = desc;
		EditorUtility.SetDirty(req);
		return req;
	}

	private static KillCountRequirement Kills(string fileName, string enemyId, int count, string label, string desc)
	{
		var req = LoadOrCreate<KillCountRequirement>($"{ReqsDir}/{fileName}.asset");
		req.enemyId = enemyId;
		req.count = count;
		req.label = label;
		req.description = desc;
		EditorUtility.SetDirty(req);
		return req;
	}

	private static MissionCompletedRequirement Missions(string fileName, MissionType? type, MissionDifficulty? minDiff, int count, string label, string desc)
	{
		var req = LoadOrCreate<MissionCompletedRequirement>($"{ReqsDir}/{fileName}.asset");
		req.filterByType = type.HasValue;
		req.type = type ?? MissionType.Purge;
		req.filterByMinDifficulty = minDiff.HasValue;
		req.minDifficulty = minDiff ?? MissionDifficulty.I;
		req.count = count;
		req.label = label;
		req.description = desc;
		EditorUtility.SetDirty(req);
		return req;
	}

	private static MilestoneRequirement Milestone(string fileName, string milestoneId, int count, string label, string desc)
	{
		var req = LoadOrCreate<MilestoneRequirement>($"{ReqsDir}/{fileName}.asset");
		req.milestoneId = milestoneId;
		req.count = count;
		req.label = label;
		req.description = desc;
		EditorUtility.SetDirty(req);
		return req;
	}

	private static T LoadOrCreate<T>(string path) where T : ScriptableObject
	{
		var existing = AssetDatabase.LoadAssetAtPath<T>(path);
		if (existing != null) return existing;
		var obj = ScriptableObject.CreateInstance<T>();
		AssetDatabase.CreateAsset(obj, path);
		return obj;
	}

	private static void EnsureFolder(string path)
	{
		if (AssetDatabase.IsValidFolder(path)) return;
		var parent = Path.GetDirectoryName(path).Replace('\\', '/');
		var leaf = Path.GetFileName(path);
		if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
		AssetDatabase.CreateFolder(parent, leaf);
	}
}
#endif
