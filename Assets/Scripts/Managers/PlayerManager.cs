using Midevil.Item;
using System.Collections.Generic;

public class PlayerManager : Singleton<PlayerManager>
{
	// Public Variables
	public Player player;

	// Private Variables
	private List<ItemStats> items = new();

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

	// Private Methods
	private void Start()
	{
		player = FindFirstObjectByType<Player>();
	}
}
