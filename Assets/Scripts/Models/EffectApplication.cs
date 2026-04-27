using Midevil.Effect;
using System;
using UnityEngine;

[Serializable]
public struct EffectApplication
{
	public EffectData effect;
	[Range(0f, 1f)] public float chance;
	[Tooltip("Used only when the parent group has 'pickOnlyOne' enabled — relative weight for the weighted pick.")]
	public float weight;

	public bool RollChance() => effect != null && UnityEngine.Random.value < chance;
}
