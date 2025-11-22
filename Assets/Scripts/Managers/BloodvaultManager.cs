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
			cachedData = new();
			Save();
			return cachedData;
		}

		string json = File.ReadAllText(filePath);
		cachedData = JsonUtility.FromJson<BloodVaultData>(json);

		if (cachedData == null)
			cachedData = new BloodVaultData();

		return cachedData;
	}

	public static void Add(BloodVaultEntry entry)
	{
		Load().entries.Add(entry);
		Save();
	}

	public static void Save()
	{
		string json = JsonUtility.ToJson(cachedData, true);
		File.WriteAllText(filePath, json);
	}
}
