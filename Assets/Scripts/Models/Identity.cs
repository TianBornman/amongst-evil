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
			profileIcon = IconReferenceIndex.Human;
		}

		public void RecordStatus(BloodVaultStatus status)
		{
			var bloodVaultEntry = new BloodVaultEntry
			{
				identity = this,
				status = status
			};

			BloodvaultManager.AddOrUpdate(bloodVaultEntry);
		}

		public void CommitResult()
		{
			lifeTimeResult.Add(currentResult);
			currentResult = new();
		}

		public void ClearGear()
		{
			weapon = null;
			armour = null;
		}

		public void Flee() => RecordStatus(BloodVaultStatus.Alive);

		public void Die() => RecordStatus(BloodVaultStatus.Dead);
	}
}
