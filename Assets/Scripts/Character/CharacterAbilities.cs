using Midevil.Ability;
using System;
using System.Linq;
using UnityEngine;

public class CharacterAbilities : MonoBehaviour
{
	// Editor Variables
	[Header("References")]

	// Public Variables
	public Ability currentAbility;

	// Protected Variables
	protected Character character;

	// Public Methods
	public void TryUseAbility(Guid id)
	{
		if (currentAbility != null)
			return;

		var ability = character.currentAbilities.FirstOrDefault(ab => ab.id == id);

		if (ability == null || !ability.IsReady)
			return;

		currentAbility = ability;
		character.SetState(new AbilityState(character));
	}

	public virtual void AddAbility(Ability ability)
	{
		character.currentAbilities.Add(ability);
	}

	public virtual void RemoveAbility(Ability ability)
	{
		character.currentAbilities.Remove(ability);
	}

	// Private Methods
	public void Awake()
	{
		character = GetComponent<Character>();
		GetComponentInChildren<CharacterAnimAPI>().Ability = CastAbility;
	}

	private void Update()
	{
		foreach (var ability in character.currentAbilities)
			ability?.Update(Time.deltaTime);
	}

	private void CastAbility()
	{
		currentAbility?.TryUse();
	}
}
