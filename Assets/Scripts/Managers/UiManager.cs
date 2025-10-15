using UnityEngine;
using UnityEngine.UIElements;

public class UiManager : Singleton<UiManager>
{
	// Editor Variables
	[Header("References")]
	public UIDocument gameUi;
	public UIDocument statsUi;

	// Public Methods
	public void BindPlayerStats(Player player)
	{
		gameUi.rootVisualElement.Q<ProgressBar>("PlayerHealth").dataSource = player.stats;
		gameUi.rootVisualElement.Q<Label>("Level").dataSource = player;
		gameUi.rootVisualElement.Q<ProgressBar>("XpBar").dataSource = player;

		statsUi.rootVisualElement.Q<VisualElement>("Stats").dataSource = player;
	}	
	
	public void BindEnemyStats(Stats stats)
	{
		gameUi.rootVisualElement.Q<ProgressBar>("EnemyHealth").dataSource = stats;
	}
}
