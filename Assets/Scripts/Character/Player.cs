using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class Player : Character
{
	// Editor References
	[Header("References")]
	public Transform cameraPosition;

	// Override Methods
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
}