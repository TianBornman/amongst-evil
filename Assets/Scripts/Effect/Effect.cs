using System;
using UnityEngine;

namespace Midevil.Effect
{
	public class Effect
	{
		public Guid id;
		public string effectType;
		public Texture2D icon;
		public float duration = -1f;
		protected float elapsed;
		public int stackCount;

		public Action<int> OnCountChanged;

		public int StackCount
		{
			get => stackCount;
			set
			{
				if (stackCount == value) return;
				stackCount = value;
				OnCountChanged?.Invoke(stackCount);
			}
		}

		public virtual void OnApply(Character owner) { }
		public virtual void OnRemove(Character owner) { }

		public virtual bool RefreshOrStack(Effect existingEffect) { return false; }

		public bool IsExpired => duration > 0 && elapsed >= duration;

		public virtual void TickTimer(float deltaTime)
		{
			if (duration > 0) elapsed += deltaTime;
		}
	}
}