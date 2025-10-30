using System;

namespace Midevil.Ability
{
	public abstract class Ability
	{
		protected Character owner;
		public AbilityData data;

		private float cooldownTimer;
		private bool isConsumable;
		private int remainingCharges;

		public event Action<int> OnChargesChanged;

		public int RemainingCharges
		{
			get => remainingCharges;
			set
			{
				if (remainingCharges == value) return;
				remainingCharges = value;
				OnChargesChanged?.Invoke(remainingCharges);
			}
		}

		protected Ability(Character owner, AbilityData data)
		{
			this.owner = owner;
			this.data = data;

			isConsumable = data.isConsumable;
			remainingCharges = data.maxCharges;
		}

		public bool IsReady => cooldownTimer <= 0f && RemainingCharges > 0;

		public virtual void Update(float deltaTime)
		{
			if (cooldownTimer > 0f)
				cooldownTimer -= deltaTime;
		}

		public void TryUse()
		{
			if (!IsReady) return;
			//if (owner.CurrentStats.mana < data.manaCost) return;

			//owner.CurrentStats.mana -= data.manaCost;

			if (isConsumable)
				RemainingCharges--;

			Execute();
			cooldownTimer = data.cooldown;

			//if (data.isConsumable && remainingCharges <= 0)
			//	owner.AbilityManager.RemoveAbility(this);
		}

		protected abstract void Execute();

		public void ResetCooldown() => cooldownTimer = 0f;
	}
}