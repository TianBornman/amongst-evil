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
			AddAbility(abilityData.CreateRuntime(this));
		}
	}

	protected override void SetState(CharacterState state)
	{
		if (State == CharacterState.Dead)
			return;

		base.SetState(state);

		switch (State)
		{
			case CharacterState.Moving:
				Moving();
				break;
			case CharacterState.Attacking:
				//Attacking();
				break;
			case CharacterState.Dead:
				Die();
				break;
			default:
				break;
		}
	}

	public override void AddAbility(Ability ability)
	{
		base.AddAbility(ability);

		PartyManager.Instance.BindAbility(ability, this);
	}

	public override void RemoveAbility(Ability ability)
	{
		base.RemoveAbility(ability);

		PartyManager.Instance.ClearAbility(ability);
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
		SetState(CharacterState.Moving);
	}

	public override void EquipItem(ItemStats item)
	{
		base.EquipItem(item);

		InventoryManager.Instance.RemoveItem(item);
		UiManager.Instance.UpdateCharacterPanels();
	}

	public override void UnequipItem(ItemStats item)
	{
		base.UnequipItem(item);

		InventoryManager.Instance.AddItem(item);
		UiManager.Instance.UpdateCharacterPanels();
	}

	// State Methods
	private void Moving()
	{
		//if (target == null && PartyManager.Instance.InCombat)
		//{
		//	var target = PartyManager.Instance.GetTarget(this);
		//	SetTarget(target);
		//}
	}

	private void Die()
	{
		identity.Die();
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