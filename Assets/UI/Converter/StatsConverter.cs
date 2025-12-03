using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class StatsConverter : MonoBehaviour
{
#if UNITY_EDITOR
	[InitializeOnLoadMethod]
#else
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
#endif
	public static void RegisterConverters()
	{
		var group = new ConverterGroup("Current and Total HP");

		group.AddConverter((ref Stats value) => $"{value.health} / {value.maxHealth}");

		ConverterGroups.RegisterConverterGroup(group);

		group = new ConverterGroup("Current and Total XP");

		group.AddConverter((ref Stats value) => $"{value.currentXp} / {value.neededXp}");

		ConverterGroups.RegisterConverterGroup(group);
	}
}
