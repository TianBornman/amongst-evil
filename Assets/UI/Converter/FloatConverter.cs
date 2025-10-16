using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class FloatConverter : MonoBehaviour
{
#if UNITY_EDITOR
	[InitializeOnLoadMethod]
#else
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
#endif
	public static void RegisterConverters()
	{
		var group = new ConverterGroup("Float to Percentage");

		group.AddConverter((ref float value) => $"{value * 100}%");

		ConverterGroups.RegisterConverterGroup(group);

		group = new ConverterGroup("Float to Attack Speed");

		group.AddConverter((ref float value) => $"{value} /s");

		ConverterGroups.RegisterConverterGroup(group);
	}
}
