using Midevil.Ability;
using UnityEngine;

public class PartyCharacterAbilities : CharacterAbilities
{
	// Override Methods
	public override void AddAbility(Ability ability)
	{
		base.AddAbility(ability);

		var partyCharacter = character as PartyCharacter;
		PartyManager.Instance.BindAbility(ability, partyCharacter);
	}

	public override void RemoveAbility(Ability ability)
	{
		base.RemoveAbility(ability);

		PartyManager.Instance.ClearAbility(ability);
	}
}
