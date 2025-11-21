using Midevil.Helpers;
using System;

namespace Midevil.Models
{
	[Serializable]
	public class RecruitData
	{
		public string name;

		public void Randomize()
		{
			name = NameGenerator.GetRandomName();
		}
	}
}
