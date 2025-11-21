using UnityEngine;

namespace Midevil.Helpers
{
	public static class NameGenerator
	{
		static readonly string[] prefixes = new string[]
		{
			"Tor", "Kel", "Var", "Mar", "Dro", "Fel", "Sar", "Gar", "Kor", "Lan",
			"Rin", "Sol", "Vor", "Tal", "Mav", "Vin", "Zal", "Ren", "Jar", "Bel"
		};

		static readonly string[] suffixes = new string[]
		{
			"en", "in", "ar", "us", "or", "an", "is", "ek", "as", "im",
			"os", "el", "un", "eth", "ok", "yr", "oth"
		};

		static readonly string[] surnamePrefixes = new string[]
		{
			"Stone", "Iron", "Storm", "Black", "Dawn", "Red", "Gold", "Wolf", "Oak", "Ash",
			"Hale", "Frost", "Bronze", "Night", "Hawk", "Raven", "Flint", "Wind", "Steel", "Crow"
		};

		static readonly string[] surnameSuffixes = new string[]
		{
			"born", "field", "hart", "son", "thorn", "wood", "ridge", "vale", "ford", "more",
			"brook", "hold", "well", "crest", "fall", "watch", "mark", "helm", "run", "dorn"
		};

		public static string GetRandomName()
		{
			string first = prefixes[UnityEngine.Random.Range(0, prefixes.Length)] +
						   suffixes[UnityEngine.Random.Range(0, suffixes.Length)];

			string last = surnamePrefixes[UnityEngine.Random.Range(0, surnamePrefixes.Length)] +
						  surnameSuffixes[UnityEngine.Random.Range(0, surnameSuffixes.Length)];

			return $"{first} {last}";
		}
	}
}
