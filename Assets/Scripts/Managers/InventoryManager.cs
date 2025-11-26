using Midevil.Item;
using Midevil.UI.Elements;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryManager : Singleton<InventoryManager>
{
	// Editor Variables
	[Header("Settings")]
	public int maxItems;

	// Public Variables
	public List<ItemStats> runInventory = new();
	public ItemStats selectedItem = null;
	public ItemElement selectedItemElement = null;

	protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (GameManager.Instance.AtHub)
			return;

		runInventory = new();
		selectedItem = null;
	}

	// Public Methods
	public bool AddItem(ItemStats itemStats)
	{
		if (runInventory.Count >= maxItems)
			return false;

		runInventory.Add(itemStats);
		UiManager.Instance.UpdateInventory();

		return true;
	}

	public void RemoveItem(ItemStats itemStats)
	{
		runInventory.Remove(itemStats);

		UiManager.Instance.UpdateInventory();
	}

	public void SetSelectedItem(ItemStats item, ItemElement element)
	{
		selectedItem = item;
		selectedItemElement = element;
		selectedItemElement.AddToClassList("selected");
	}

	public void ClearSelectedItem()
	{
		selectedItem = null;
		selectedItemElement.RemoveFromClassList("selected");
		selectedItemElement = null;
	}
}
