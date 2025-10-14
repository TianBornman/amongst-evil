using UnityEngine;
using UnityEngine.UIElements;

public class UiManager : Singleton<UiManager>
{
	// Editor Variables
	[Header("References")]
	public UIDocument gameUi;

	// Public Methods
	public void BindPlayerStats(Stats stats)
	{
		gameUi.rootVisualElement.Q<ProgressBar>("PlayerHealth").dataSource = stats;
	}	
	
	public void BindEnemyStats(Stats stats)
	{
		gameUi.rootVisualElement.Q<ProgressBar>("EnemyHealth").dataSource = stats;
	}
}
