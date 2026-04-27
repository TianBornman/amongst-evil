using System;
using UnityEngine;

namespace Midevil.Effect
{
	public class Effect
	{
		public Guid id;
		public EffectData source;
		public string group;
		public EffectStackPolicy stackPolicy = EffectStackPolicy.Refresh;
		public Texture2D icon;
		public float duration = -1f;
		public Effect parent;
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

		public bool IsExpired => duration > 0 && elapsed >= duration;

		public virtual void OnApply(Character owner) { }
		public virtual void OnRemove(Character owner) { }

		public virtual void Refresh(Effect incoming)
		{
			duration = Mathf.Max(duration, incoming.duration);
			elapsed = 0f;
		}

		public virtual void TickTimer(float deltaTime)
		{
			if (duration > 0) elapsed += deltaTime;
		}
	}
}
