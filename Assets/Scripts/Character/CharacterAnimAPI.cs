using System;
using UnityEngine;

public class CharacterAnimAPI : MonoBehaviour
{
	// Public Variables
	public Action CheckValidTarget;
	public Action Attack;

	// Public Methods
	public void OnCheckValidTarget()
	{
		CheckValidTarget();
	}

	public void OnAttack()
	{
		Attack();
	}
}
