using System;

namespace Midevil.Ability
{
	public abstract class Ability
	{
		public Guid id;
		protected Character owner;
		public AbilityData data;

		private float cooldownTimer;
		public bool isConsumable;
		private int remainingCharges;

		public event Action<int> OnChargesChanged;
		public event Action<float> OnCooldownChanged;

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

		public float CooldownTimer
		{
			get => cooldownTimer;
			set
			{
				if (cooldownTimer == value) return;
				cooldownTimer = value;
				OnCooldownChanged?.Invoke(cooldownTimer);
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
			if (CooldownTimer > 0f)
			{
				CooldownTimer -= deltaTime;
			}
		}

		public void TryUse()
		{
			if (!IsReady) return;
			//if (owner.CurrentStats.mana < data.manaCost) return;

			//owner.CurrentStats.mana -= data.manaCost;

			if (isConsumable)
				RemainingCharges--;

			Execute();
			CooldownTimer = data.cooldown;

			//if (data.isConsumable && remainingCharges <= 0)
			//	owner.AbilityManager.RemoveAbility(this);
		}

		protected abstract void Execute();

		public void ResetCooldown() => CooldownTimer = 0f;
	}
}