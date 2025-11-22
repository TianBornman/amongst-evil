using Midevil.Helpers;
using System;

namespace Midevil.Models
{
	[Serializable]
	public class Identity
	{
		public string name;
		public IconReferenceIndex profileIcon;

		public void Randomize()
		{
			name = NameGenerator.GetRandomName();
			profileIcon = IconReferenceIndex.HumanIcon;
		}
	}
}
