using System;
using UnityEngine;

public class CharacterAnimAPI : MonoBehaviour
{
	// Public Variables
	public Action Attack;
	public Action Ability;
	public Action Disappear;

	// Public Methods
	public void OnAttack() => Attack();

	public void OnAbility() => Ability();

	public void OnDisappear()
	{
		Disappear();
	}
}
