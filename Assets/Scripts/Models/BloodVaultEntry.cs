using System;
using System.Collections.Generic;

namespace Midevil.Models
{
	[Serializable]
	public class BloodVaultEntry
	{
		public Identity identity;
		public Result result;
		public string causeOfDeath;
		public string timeOfDeath;
	}

	public class BloodVaultData
	{
		public List<BloodVaultEntry> entries = new();
	}
}
