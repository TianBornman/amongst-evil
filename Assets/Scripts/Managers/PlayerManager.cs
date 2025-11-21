using Midevil.Item;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerManager : Singleton<PlayerManager>
{
	// Public Variables
	public Player player;

	// Private Variables
	private List<ItemStats> items = new();

	// Override Methods
	protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (scene.buildIndex == 0)
			return;

		player = FindFirstObjectByType<Player>();
	}

	// Public Methods
	public void AddItem(ItemStats item)
	{
		items.Add(item);

		if (item.type == ItemType.Relic)
			player.AddBuff(item.buff);

		UiManager.Instance.AddItem(item);
	}

	public void EquipItem(ItemStats item)
	{
		player.EquipItem(item);
	}

	public void UnequipItem(ItemStats item)
	{
		player.UnequipItem(item);
	}
}
