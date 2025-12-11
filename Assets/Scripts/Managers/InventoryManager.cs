using Midevil.Helpers;
using Midevil.Item;
using Midevil.Models;
using Midevil.UI.Elements;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryManager : Singleton<InventoryManager>
{
	private static string filePath =>
		Path.Combine(Application.persistentDataPath, "armoury.json");

	private static ArmouryData cachedData;

	// Editor Variables
	[Header("Settings")]
	public int maxItems;

	// Public Variables
	public List<ItemStats> runInventory = new();
	public List<ItemStats> armouryInventory = new();
	public ItemStats selectedItem = null;
	public ItemElement selectedItemElement = null;

	protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (GameManager.Instance.AtHub)
		{
			UpdateArmoury();
			return;
		}

		runInventory = new();
		selectedItem = null;
	}

	// Public Methods
	public bool AddItem(ItemStats itemStats)
	{
		if (GameManager.Instance.AtHub)
		{
			if (armouryInventory.Count >= maxItems)
				return false;

			armouryInventory.Add(itemStats);

			return true;
		}
		else
		{
			if (runInventory.Count >= maxItems)
				return false;

			runInventory.Add(itemStats);
			UiManager.Instance.UpdateInventory();

			return true;
		}
	}

	public void RemoveItem(ItemStats itemStats)
	{
		if (GameManager.Instance.AtHub)
		{
			armouryInventory.Remove(itemStats);
		}
		else
		{
			runInventory.Remove(itemStats);
			UiManager.Instance.UpdateInventory();
		}
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

	// Private Methods
	private void UpdateArmoury()
	{
		var data = Load();
		armouryInventory = new();

		foreach (var config in data.items)
		{
			var item = RefManager.Instance.GetItem(config.index);
			var itemStats = item.stats.Clone();
			itemStats.effectIndex = config.effectIndex;
			itemStats.shouldRoll = false;

			ItemHelper.Setup(itemStats);
			armouryInventory.Add(itemStats);
		}

		armouryInventory.AddRange(runInventory);

		Save();
	}

	public ArmouryData Load()
	{
		if (cachedData != null)
			return cachedData;

		if (!File.Exists(filePath))
		{
			cachedData = new ArmouryData();
			Save();
			return cachedData;
		}

		string json = File.ReadAllText(filePath);
		cachedData = JsonUtility.FromJson<ArmouryData>(json)
					  ?? new ArmouryData();

		return cachedData;
	}

	private void Save()
	{
		var itemConfigs = armouryInventory.Select(item => new ItemConfig() { index = item.index, effectIndex = item.effectIndex }).ToList();
		var data = new ArmouryData() { items = itemConfigs };

		string json = JsonUtility.ToJson(data, true);
		File.WriteAllText(filePath, json);
	}
}
