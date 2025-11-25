using Midevil.Ability;
using Midevil.Effect;
using Midevil.Item;
using Midevil.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Character
{
	#region Input

	private InputSystem_Actions inputActions;
	private InputAction abilityAction;

	private void OnEnable()
	{
		inputActions = new InputSystem_Actions();
		abilityAction = inputActions.Player.Ability;

		inputActions.Enable();
		abilityAction.performed += OnAbility;
	}

	private void OnDisable()
	{
		abilityAction.performed -= OnAbility;
		inputActions.Disable();
	}

	private void OnAbility(InputAction.CallbackContext context)
	{
		int slot = (int)context.ReadValue<float>();
		UseAbility(slot);
	}

	#endregion

	// Editor References
	[Header("References")]
	public Transform cameraPosition;
	public List<AbilityReferenceIndex> startingAbilities;

	// Public Variables
	[HideInInspector] public AbilitySlot[] abilitySlots = new AbilitySlot[4];

	[HideInInspector] public float currentXp;
	[HideInInspector] public float neededXp;

	// Public Properties
	public Result Results => identity.currentResult;

	// Override Methods
	protected override void Awake()
	{
		base.Awake();

		identity = RecruitManager.Instance.playerIdentity;

		CharacterAnimAPI animAPI = GetComponentInChildren<CharacterAnimAPI>();
		animAPI.Disappear = () => UiManager.Instance.ShowDeathScreen();
	}

	protected override void Start()
	{
		base.Start();

		stats.level = identity.level;
		neededXp = GetNeededXp(stats.level);

		AddXp(identity.xp);

		UiManager.Instance.BindPlayerStats(this);

		foreach (var abilityIndex in startingAbilities)
		{
			var abilityData = RefManager.Instance.GetAbility(abilityIndex);
			AddAbility(abilityData.CreateRuntime(this));
		}
	}

	protected override void Update()
	{
		base.Update();

		if (State == CharacterState.Moving)
			Move();
	}

	protected override void SetState(CharacterState state)
	{
		if (State == CharacterState.Dead)
			return;

		base.SetState(state);

		switch (State)
		{
			case CharacterState.Idle:
				Idle();
				break;
			case CharacterState.Moving:
				// Moving();
				break;
			case CharacterState.Attacking:
				Attacking();
				break;
			case CharacterState.Dead:
				Die();
				break;
			default:
				break;
		}
	}

	public override void EquipItem(ItemStats item)
	{
		base.EquipItem(item);

		PlayerManager.Instance.RemoveItem(item);
	}

	public override void UnequipItem(ItemStats item)
	{
		base.UnequipItem(item);

		PlayerManager.Instance.AddItem(item);
	}

	public override void AddAbility(Ability ability)
	{
		base.AddAbility(ability);

		BindAbility(ability);
	}

	public override void RemoveAbility(Ability ability)
	{
		ClearAbility(ability);

		base.RemoveAbility(ability);
	}

	public override void AddEffect(Effect effect)
	{
		base.AddEffect(effect);

		UiManager.Instance.AddEffect(effect);
	}

	public override void RemoveEffect(Effect effect)
	{
		base.RemoveEffect(effect);

		UiManager.Instance.RemoveEffect(effect);
	}

	// State Methods
	private void Idle()
	{
		StartCoroutine(GetTarget());
	}

	private void Attacking()
	{
		if (target != null)
			target.SetTarget(this);
	}

	private void Die()
	{
		var entry = new BloodVaultEntry
		{
			identity = identity,
			status = BloodVaultStatus.Dead
		};

		BloodvaultManager.AddOrUpdate(entry);

		RecruitManager.Instance.playerIdentity = null;
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

	public void AssignAbilitySlot(int slot, Ability ability)
	{
		abilitySlots[slot].assignedAbility = ability;
	}

	public void UseAbility(int slot)
	{
		abilitySlots[slot].assignedAbility?.TryUse();
	}

	// Private Methods
	private void Move()
	{
		if (target == null)
		{
			SetState(CharacterState.Idle);
			return;
		}

		agent.SetDestination(target.transform.position);

		if (stats.range >= Vector3.Distance(transform.position, target.transform.position))
			SetState(CharacterState.Attacking);
	}

	private Character GetClosestTarget()
	{
		var targets = SpawnManager.Instance.spawnedCharacters;

		return targets.OrderBy(target => Vector3.Distance(transform.position, target.transform.position))
					  .Select(target => target.GetComponent<Character>())
					  .FirstOrDefault(target => target.IsAlive);
	}

	private void LevelUp()
	{
		stats.level++;
		neededXp = GetNeededXp(stats.level);
		Damage(-stats.maxHealth * stats.levelHeal);

		LevelUpManager.Instance.LevelUp();
	}

	private float GetNeededXp(int level)
	{
		return 10f * Mathf.Pow(level, 1.3f) + 5f * level;
	}

	private void BindAbility(Ability ability)
	{
		for (int i = 0; i < abilities.Count && i < abilitySlots.Length; i++)
		{
			if (abilitySlots[i].HasAbility)
				continue;

			abilitySlots[i].assignedAbility = abilities[i];
			UiManager.Instance.BindAbility(i, abilities[i]);
		}
	}

	private void ClearAbility(Ability ability)
	{
		var slot = abilitySlots.FirstOrDefault(slot => slot.assignedAbility == ability);
		slot.Clear();
	}

	private IEnumerator GetTarget()
	{
		while (target == null)
		{
			var getTarget = GetClosestTarget();
			target = getTarget;

			if (target != null)
				SetState(CharacterState.Moving);

			yield return new WaitForSeconds(1f);
		}
	}
}