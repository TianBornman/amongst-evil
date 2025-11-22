using Midevil.Helpers;
using System;
using UnityEngine;

namespace Midevil.Models
{
	[Serializable]
	public class Identity
	{
		public string name;
		public Texture2D profileIcon;

		public void Randomize()
		{
			name = NameGenerator.GetRandomName();
		}
	}
}
