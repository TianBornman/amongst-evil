using Midevil.Effect;
using System.Linq;
using UnityEngine;

public class CharacterEffects : MonoBehaviour
{
	// Editor Variables
	[Header("References")]

	// Private Variables
	private Character character;

	// Public Methods
	public void AddEffect(Effect effect)
	{
		var sameEffect = character.currentEffects.FirstOrDefault(b => b.effectType == effect.effectType);
		if (sameEffect != null)
		{
			if (sameEffect.RefreshOrStack(effect))
				return;
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

	// Private Methods
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
