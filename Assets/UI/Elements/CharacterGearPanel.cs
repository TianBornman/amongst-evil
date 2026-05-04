using UnityEngine.UIElements;

namespace Midevil.UI.Elements
{
	public static class CharacterGearPanel
	{
		public static void WireTabs(VisualElement root)
		{
			var gearTab = root.Q<VisualElement>("GearTab");
			var statsTab = root.Q<VisualElement>("StatsTab");
			var gearPanel = root.Q<VisualElement>("GearPanel");
			var statsPanel = root.Q<VisualElement>("StatsPanel");

			if (gearTab == null || statsTab == null || gearPanel == null || statsPanel == null)
				return;

			gearTab.RegisterCallback<ClickEvent>(_ => ShowGear(gearTab, statsTab, gearPanel, statsPanel));
			statsTab.RegisterCallback<ClickEvent>(_ => ShowStats(gearTab, statsTab, gearPanel, statsPanel));
		}

		public static void ResetToGearTab(VisualElement root)
		{
			var gearTab = root.Q<VisualElement>("GearTab");
			var statsTab = root.Q<VisualElement>("StatsTab");
			var gearPanel = root.Q<VisualElement>("GearPanel");
			var statsPanel = root.Q<VisualElement>("StatsPanel");

			if (gearTab == null || statsTab == null || gearPanel == null || statsPanel == null)
				return;

			ShowGear(gearTab, statsTab, gearPanel, statsPanel);
		}

		public static void PopulateStats(VisualElement root, Character character)
		{
			if (character == null) return;

			var stats = character.stats;
			var baseStats = character.baseScaledStats;
			if (stats == null || baseStats == null) return;

			Set(root, "MaxHealthRow", baseStats.maxHealth, stats.maxHealth);
			Set(root, "BlockChanceRow", baseStats.blockChance, stats.blockChance);
			Set(root, "DodgeChanceRow", baseStats.dodgeChance, stats.dodgeChance);

			Set(root, "DamageRow", baseStats.damage, stats.damage);
			Set(root, "AttackSpeedRow", baseStats.attackSpeed, stats.attackSpeed);
			Set(root, "CritChanceRow", baseStats.critChance, stats.critChance);
			Set(root, "CritDamageRow", baseStats.critDamage, stats.critDamage);
			Set(root, "RangeRow", baseStats.range, stats.range);

			Set(root, "MoveSpeedRow", baseStats.moveSpeed, stats.moveSpeed);
			Set(root, "CastSpeedRow", baseStats.castSpeed, stats.castSpeed);
			Set(root, "SizeRow", baseStats.size, stats.size);
		}

		private static void Set(VisualElement root, string name, float baseValue, float totalValue)
		{
			var row = root.Q<StatRowElement>(name);
			row?.SetValues(baseValue, totalValue);
		}

		private static void ShowGear(VisualElement gearTab, VisualElement statsTab, VisualElement gearPanel, VisualElement statsPanel)
		{
			gearTab.AddToClassList("tab-button--active");
			statsTab.RemoveFromClassList("tab-button--active");
			gearPanel.RemoveFromClassList("hidden");
			statsPanel.AddToClassList("hidden");
		}

		private static void ShowStats(VisualElement gearTab, VisualElement statsTab, VisualElement gearPanel, VisualElement statsPanel)
		{
			statsTab.AddToClassList("tab-button--active");
			gearTab.RemoveFromClassList("tab-button--active");
			statsPanel.RemoveFromClassList("hidden");
			gearPanel.AddToClassList("hidden");
		}
	}
}
