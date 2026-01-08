using UnityEngine;

public class BattleManager : Singleton<BattleManager>
{
	// Public Variables
	public bool inBattle = false;

	// Public Methods
	public void StartBattle(Encounter encounter)
	{
		Debug.Log($"Starting battle: {encounter.encounterName}");
		inBattle = true;
	}
}
