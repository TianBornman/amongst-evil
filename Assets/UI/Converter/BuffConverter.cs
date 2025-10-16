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
		var group = new ConverterGroup("Buff to String");

		group.AddConverter((ref Buff buff) => BuffToString(buff));

		ConverterGroups.RegisterConverterGroup(group);
	}

	private static string BuffToString(Buff buff)
	{
		var value = string.Empty;

		if (buff.stats.maxHealth != 0)
			value += $"Max Health: {buff.stats.maxHealth}\n";
		if (buff.stats.damage != 0)
			value += $"Damage: {buff.stats.damage}\n";
		if (buff.stats.critChance != 0)
			value += $"Critical Chance: {buff.stats.critChance * 100}%\n";
		if (buff.stats.critDamage != 0)
			value += $"Critical Damage: {buff.stats.critDamage * 100}%\n";
		if (buff.stats.range != 0)
			value += $"Range: {buff.stats.range}\n";
		if (buff.stats.moveSpeed != 0)
			value += $"Move Speed: {buff.stats.moveSpeed}\n";

		return value;
	}
}
