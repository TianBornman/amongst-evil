using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelUpManager : Singleton<LevelUpManager>
{
	// Editor Variables
	[Header("References")]
	public Player player;
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
		player.AddBuff(card.buff);
		UiManager.Instance.HideLevelUp();
	}

	// Private Methods
	private void Start()
	{
		if (player == null)
			player = FindFirstObjectByType<Player>();
	}
}
