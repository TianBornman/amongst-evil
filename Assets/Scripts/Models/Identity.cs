using Midevil.Helpers;
using Midevil.Item;
using System;

namespace Midevil.Models
{
	[Serializable]
	public class Identity
	{
		public string id;
		public string characterName;
		public IconReferenceIndex profileIcon;

		public int level = -1;
		public float xp = 0;

		[NonSerialized] public ItemStats weapon;
		public ItemConfig weaponConfig = new();

		[NonSerialized] public ItemStats armour;
		public ItemConfig armourConfig = new();

		[NonSerialized] public Result currentResult = new();
		public Result lifeTimeResult = new();

		public void Randomize()
		{
			id = Guid.NewGuid().ToString();
			level = 1;
			characterName = NameGenerator.GetRandomName();
			profileIcon = IconReferenceIndex.HumanIcon;
		}

		public void Save(BloodVaultStatus status)
		{
			lifeTimeResult.Add(currentResult);
			currentResult = new();

			var bloodVaultEntry = new BloodVaultEntry
			{
				identity = this,
				status = status
			};

			BloodvaultManager.AddOrUpdate(bloodVaultEntry);
		}

		public void ClearGear()
		{
			weapon = null;
			armour = null;
		}

		public void Flee()
		{
			// TODO: Make sure each individual stat is saved properly
			//level = PlayerManager.Instance.player.stats.level;
			//xp = PlayerManager.Instance.player.currentXp;

			Save(BloodVaultStatus.Alive);
		}

		public void Die()
		{
			Save(BloodVaultStatus.Dead);
		}
	}
}
