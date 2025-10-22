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
	}

	// Private Methods
	private void Start()
	{
		player = FindFirstObjectByType<Player>();
	}
}
