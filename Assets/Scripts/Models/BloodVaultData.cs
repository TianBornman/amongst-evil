using System;
using System.Collections.Generic;

namespace Midevil.Models
{
	[Serializable]
	public class BloodVaultEntry
	{
		public Identity identity;
		public BloodVaultStatus status;
	}

	[Serializable]
	public class BloodVaultData
	{
		public List<BloodVaultEntry> entries = new();
		public SectProgressData sectProgress = new();
	}

	public enum BloodVaultStatus
	{
		Dead,
		Alive
	}
}
