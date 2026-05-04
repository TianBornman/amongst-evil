using System.Collections.Generic;
using UnityEngine;

namespace Midevil.Progression
{
	[CreateAssetMenu(fileName = "Sect Rank", menuName = "Progression/Sect Rank")]
	public class SectRankData : ScriptableObject
	{
		[Tooltip("1-based position on the Spiral. Rank 1 is the starting rank — its requirements are unused.")]
		public int rankNumber = 1;

		public string rankName = "Whispering Ashes";
		public string statusTitle = "Unrecognised / Aspirants";

		[TextArea] public string flavor;
		[TextArea] public string quote;

		public Sprite badge;

		[Tooltip("All requirements must be met to ascend to this rank. Rank 1's list is ignored.")]
		public List<RankRequirement> requirements = new();
	}
}
