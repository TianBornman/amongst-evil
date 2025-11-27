using System;

namespace Midevil.Ability
{
	public abstract class Ability
	{
		public Guid id = Guid.Empty;
		protected Character owner;
		public AbilityData data;

		public float cooldownTimer;
		public bool isConsumable;
		public int remainingCharges;

		protected Ability(Character owner, AbilityData data)
		{
			this.owner = owner;
			this.data = data;

			id = Guid.NewGuid();
			isConsumable = data.isConsumable;
			remainingCharges = data.maxCharges;
		}

		public bool IsReady => cooldownTimer <= 0f && remainingCharges > 0;

		public virtual void Update(float deltaTime)
		{
			if (cooldownTimer > 0f)
			{
				cooldownTimer -= deltaTime;
			}
		}

		public void TryUse()
		{
			if (!IsReady) return;
			//if (owner.CurrentStats.mana < data.manaCost) return;

			//owner.CurrentStats.mana -= data.manaCost;

			if (isConsumable)
				remainingCharges--;

			Execute();
			cooldownTimer = data.cooldown;

			//if (data.isConsumable && remainingCharges <= 0)
			//	owner.AbilityManager.RemoveAbility(this);
		}

		protected abstract void Execute();

		public void ResetCooldown() => cooldownTimer = 0f;
	}
}