using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class Player : Character
{
	// Editor References
	[Header("References")]
	public Transform cameraPosition;

	// Public Variables
	[HideInInspector] public int level;
	[HideInInspector] public float currentXp;
	[HideInInspector] public float neededXp;

	// Override Methods
	protected override void Start()
	{
		base.Start();

		level = 1;
		neededXp = GetNeededXp(level);

		UiManager.Instance.BindPlayerStats(this);
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
				Attack();
				break;
			case CharacterState.Dead:
				break;
			default:
				break;
		}
	}

	// State Methods
	private void Idle()
	{
		var getTarget = GetClosestTarget();
		target = getTarget;

		if (target != null)
			SetState(CharacterState.Moving);
	}

	private void Attack()
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
	private void Update()
	{
		if (State == CharacterState.Moving)
			Move();
	}

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
		var targets = Physics.OverlapSphere(transform.position, 30, LayerMask.GetMask("Enemy"));

		return targets.Select(target => target.GetComponent<Character>()).FirstOrDefault(target => target.IsAlive);
	}

	private void LevelUp()
	{
		level++;
		neededXp = GetNeededXp(level);
		Damage(-stats.maxHealth * stats.levelHeal);

		LevelUpManager.Instance.LevelUp();
	}

	private float GetNeededXp(int level)
	{
		return 10f * Mathf.Pow(level, 1.3f) + 5f * level;
	}
}