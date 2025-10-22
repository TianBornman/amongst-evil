using UnityEngine;

namespace Midevil.UpgradeCard
{
	[CreateAssetMenu]
	public class UpgradeCard : ScriptableObject
	{
		// Editor Variables
		public string cardName;
		public string cardDescription;
		public Buff buff;
	}
}
