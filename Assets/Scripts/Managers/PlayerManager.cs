using Midevil.Item;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerManager : Singleton<PlayerManager>
{
	// Public Variables
	public Player player;
	public List<ItemStats> items = new();

	// Override Methods
	protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (GameManager.Instance.AtHub)
			return;

		player = FindFirstObjectByType<Player>();
	}

	// Public Methods
	public void AddItem(ItemStats item)
	{
		items.Add(item);

		if (item.type == ItemType.Relic)
			player.AddBuff(item.buff);

		UiManager.Instance.UpdateItems();
	}

	public void RemoveItem(ItemStats item)
	{
		items.Remove(item);
		UiManager.Instance.UpdateItems();
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
