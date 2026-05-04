using Midevil.Models;
using System.IO;
using UnityEngine;

public static class BloodvaultManager
{
	private static string filePath =>
		Path.Combine(Application.persistentDataPath, "bloodvault.json");

	private static BloodVaultData cachedData;

	public static BloodVaultData Load()
	{
		if (cachedData != null)
			return cachedData;

		if (!File.Exists(filePath))
		{
			cachedData = new BloodVaultData();
			Save();
			return cachedData;
		}

		string json = File.ReadAllText(filePath);
		cachedData = JsonUtility.FromJson<BloodVaultData>(json)
					  ?? new BloodVaultData();

		return cachedData;
	}

	public static void AddOrUpdate(BloodVaultEntry entry)
	{
		var data = Load();

		var existing = data.entries
			.Find(e => e.identity.id == entry.identity.id);

		if (existing != null)
		{
			existing.identity = entry.identity;
			existing.status = entry.status;
		}
		else
		{
			data.entries.Add(entry);
		}

		Save();
	}

	public static void Save()
	{
		if (cachedData == null) return;
		string json = JsonUtility.ToJson(cachedData, true);
		File.WriteAllText(filePath, json);
	}
}
