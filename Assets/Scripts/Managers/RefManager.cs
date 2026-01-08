using Midevil.Ability;
using Midevil.Effect;
using Midevil.Item;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RefManager : Singleton<RefManager>
{
	// Editor Variables
	[Header("Objects")]
	public Encounter baseEncounter;

	[Header("Layer Masks")]
	public LayerMask targetableMask;

	[Header("Animations")]
	public AnimatorOverrideController unarmed;
	public AnimatorOverrideController sword1H;
	public AnimatorOverrideController bow;

	[Header("Reference Index")]
	public List<IconReference> icons;
	public List<ItemReference> items;
	public List<EffectReference> effects;
	public List<AbilityReference> abilities;

	// Public Methods
	public Texture2D GetIcon(IconReferenceIndex index)
	{
		return icons.Find(x => x.index == index).icon;
	}

	public Item GetItem(ItemReferenceIndex index)
	{
		return items.Find(x => x.index == index).item;
	}

	public EffectData GetEffect(EffectReferenceIndex index)
	{
		return effects.Find(x => x.index == index).effect;
	}

	public AbilityData GetAbility(AbilityReferenceIndex index)
	{
		return abilities.Find(x => x.index == index).ability;
	}
}

[Serializable]
public struct IconReference
{
	public IconReferenceIndex index;
	public Texture2D icon;
}

public enum IconReferenceIndex
{
	Human,
	Anonymous
}

[Serializable]
public struct ItemReference
{
	public ItemReferenceIndex index;
	public Item item;
}

public enum ItemReferenceIndex
{
	None,
	IronArmour,
	RustySword,
	WoodenBow
}

[Serializable]
public struct EffectReference
{
	public EffectReferenceIndex index;
	public EffectData effect;
}

public enum EffectReferenceIndex
{
	None,
	HealOnKill,
	BurnOnHit
}

[Serializable]
public struct AbilityReference
{
	public AbilityReferenceIndex index;
	public AbilityData ability;
}

public enum AbilityReferenceIndex
{
	None,
	HealthPotion,
	Fireball
}