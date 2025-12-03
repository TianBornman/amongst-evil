using UnityEngine;

namespace Midevil.Helpers
{
	public static class ColorHelper
	{
		private static readonly Color[] PartyColors = new Color[]
	   {
			Color.blue, 
			Color.green, 
			Color.red 
	   };

		public static Color GetPartyColor(int index)
		{
			index = Mathf.Clamp(index, 0, PartyColors.Length - 1);
			return PartyColors[index];
		}
	}
}
