using Midevil.UpgradeCard;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelUpManager : Singleton<LevelUpManager>
{
	// TODO: Move into an XP system where the entire party gets XP and levels up together
	// Levels just give minor stat improvements and maybe some version of an ability
	// Popups only between floors

	// Editor Variables
	[Header("References")]
	public List<UpgradeCard> upgrades;

	// Public Methods
	public void LevelUp()
	{
		var selectedUpgrades = upgrades
								.OrderBy(x => Random.value)
								.Take(Mathf.Min(3, upgrades.Count))
								.ToList();

		for (int i = 0; i < selectedUpgrades.Count; i++)
			UiManager.Instance.BindUpgradeCard(i, selectedUpgrades[i], ApplyUpgrade);

		UiManager.Instance.ShowLevelUp();
	}

	public void ApplyUpgrade(UpgradeCard card)
	{
		//PlayerManager.Instance.player.AddBuff(card.buff);
		UiManager.Instance.HideLevelUp();
	}
}
