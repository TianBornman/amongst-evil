using Midevil.Ability;
using Midevil.Item;
using Midevil.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Guid = System.Guid;

public class PartyCharacter : Character
{
	// Editor References
	[Header("References")]
	public Transform cameraPosition;
	public List<AbilityReferenceIndex> startingAbilities;

	// Public Variables
	[HideInInspector] public float currentXp;
	[HideInInspector] public float neededXp;

	// Public Properties
	public Result Results => identity.currentResult;

	// Override Methods
	protected override void Awake()
	{
		base.Awake();

		CharacterAnimAPI animAPI = GetComponentInChildren<CharacterAnimAPI>();
		animAPI.Disappear = () => PartyManager.Instance.RemovePartyMember(this);
	}

	protected override void Start()
	{
		base.Start();

		UiManager.Instance.BindPartyMemberStats(this);

		//	stats.level = identity.level;
		//	neededXp = GetNeededXp(stats.level);

		//	AddXp(identity.xp);

		//	UiManager.Instance.BindPlayerStats(this);

		foreach (var abilityIndex in startingAbilities)
		{
			var abilityData = RefManager.Instance.GetAbility(abilityIndex);
			abilities.AddAbility(abilityData.CreateRuntime(this));
		}
	}

	public override void Die()
	{
		base.Die();

		identity.Die();
	}

	//public override void AddEffect(Effect effect)
	//{
	//	base.AddEffect(effect);

	//	UiManager.Instance.AddEffect(effect);
	//}

	//public override void RemoveEffect(Effect effect)
	//{
	//	base.RemoveEffect(effect);

	//	UiManager.Instance.RemoveEffect(effect);
	//}

	public void CheckPositionChanged()
	{
		SetState(new MoveState(this));
	}

	// Public Methods
	public void AddXp(float amount)
	{
		currentXp += amount;

		Results.xpGained += currentXp;

		while (currentXp >= neededXp)
		{
			currentXp -= neededXp;
			LevelUp();
		}
	}

	// Private Methods
	private void LevelUp()
	{
		stats.level++;
		neededXp = GetNeededXp(stats.level);
		Heal(stats.maxHealth * stats.levelHeal);

		LevelUpManager.Instance.LevelUp();
	}

	private float GetNeededXp(int level)
	{
		return 10f * Mathf.Pow(level, 1.3f) + 5f * level;
	}
}