using System;
using UnityEngine;

public class CharacterAnimAPI : MonoBehaviour
{
	// Public Variables
	public Action Attack;

	// Public Methods
	public void OnAttack()
	{
		Attack();
	}
}
