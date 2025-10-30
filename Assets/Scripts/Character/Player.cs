using Midevil.Item;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Character
{
	#region Input

	private InputSystem_Actions inputActions;
	private InputAction blockAction;

	private void OnEnable()
	{
		inputActions = new InputSystem_Actions();
		blockAction = inputActions.Player.Block;

		inputActions.Enable();
		blockAction.performed += OnBlock;
	}

	private void OnDisable()
	{
		blockAction.performed -= OnBlock;
		inputActions.Disable();
	}

	private void OnBlock(InputAction.CallbackContext context)
	{
		if (State == CharacterState.Blocking)
			SetState(CharacterState.Attacking);
		else if (State == CharacterState.Attacking)
			SetState(CharacterState.Blocking);
	}

	#endregion

	// Editor References
	[Header("References")]
	public Transform cameraPosition;

	// Public Variables
	[HideInInspector] public float currentXp;
	[HideInInspector] public float neededXp;

	// Override Methods
	protected override void Awake()
	{
		base.Awake();

		CharacterAnimAPI animAPI = GetComponentInChildren<CharacterAnimAPI>();
		// Navigate to main menu
		animAPI.Disappear = () => UiManager.Instance.ShowDeathScreen();
	}

	protected override void Start()
	{
		base.Start();

		stats.level = 1;
		neededXp = GetNeededXp(stats.level);

		UiManager.Instance.BindPlayerStats(this);
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
				break;
			default:
				break;
		}
	}

	public override void EquipItem(ItemStats item)
	{
		base.EquipItem(item);

		UiManager.Instance.EquipItem(item);
	}

	public override void UnequipItem(ItemStats item)
	{
		base.UnequipItem(item);

		UiManager.Instance.UnequipItem(item.type);
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

	// Public Methods
	public void AddXp(float amount)
	{
		currentXp += amount;

		while (currentXp >= neededXp)
		{
			currentXp -= neededXp;
			LevelUp();
		}
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