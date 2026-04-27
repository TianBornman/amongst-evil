using Midevil.Effect;
using System.Linq;
using UnityEngine;

public class CharacterEffects : MonoBehaviour
{
	private Character character;

	public void AddEffect(Effect effect)
	{
		if (!string.IsNullOrEmpty(effect.group))
		{
			var existing = character.currentEffects.FirstOrDefault(e => e.group == effect.group);
			if (existing != null)
			{
				switch (effect.stackPolicy)
				{
					case EffectStackPolicy.Reject:
						return;
					case EffectStackPolicy.Refresh:
						existing.Refresh(effect);
						return;
					case EffectStackPolicy.Replace:
						RemoveEffect(existing);
						break;
					case EffectStackPolicy.Allow:
						break;
				}
			}
		}

		effect.OnApply(character);
		character.currentEffects.Add(effect);
		character.RecalculateStats();
	}

	public void RemoveEffect(Effect effect)
	{
		effect.OnRemove(character);
		character.currentEffects.Remove(effect);
		character.RecalculateStats();
	}

	public void Awake()
	{
		character = GetComponent<Character>();
	}

	private void Update()
	{
		for (int i = character.currentEffects.Count - 1; i >= 0; i--)
		{
			var effect = character.currentEffects[i];
			effect.TickTimer(Time.deltaTime);

			if (effect is IOnTick tick)
				tick.Tick(character, Time.deltaTime);

			if (effect.IsExpired)
				RemoveEffect(effect);
		}
	}
}
