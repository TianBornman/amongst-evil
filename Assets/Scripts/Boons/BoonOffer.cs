namespace Midevil.Boons
{
	public class BoonOffer
	{
		public BoonCard card;
		// null when card.targeting == Creed (the whole party receives it).
		public PartyCharacter recipient;
	}
}
