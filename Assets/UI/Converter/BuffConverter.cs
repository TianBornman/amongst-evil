using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BuffConverter : MonoBehaviour
{
#if UNITY_EDITOR
	[InitializeOnLoadMethod]
#else
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
#endif
	public static void RegisterConverters()
	{

		// Create local Converters.
		var group = new ConverterGroup("Buff to String");

		// Converter groups can have multiple converters. This example converts a float to both a color and a string.
		group.AddConverter((ref Buff buff) => BuffToString(buff));

		// Register the converter group in InitializeOnLoadMethod to make it accessible from the UI Builder.
		ConverterGroups.RegisterConverterGroup(group);
	}

	private static string BuffToString(Buff buff)
	{
		var value = string.Empty;

		if (buff.stats.maxHealth != 0)
			value += $"Max Health: {buff.stats.maxHealth}\n";
		if (buff.stats.damage != 0)
			value += $"Damage: {buff.stats.damage}\n";
		if (buff.stats.range != 0)
			value += $"Range: {buff.stats.range}\n";
		if (buff.stats.moveSpeed != 0)
			value += $"Move Speed: {buff.stats.moveSpeed}\n";

		return value;
	}
}
