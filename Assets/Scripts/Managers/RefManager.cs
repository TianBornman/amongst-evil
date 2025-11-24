using Midevil.Item;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RefManager : Singleton<RefManager>
{
	// Editor Variables
	[Header("Animations")]
	public AnimatorOverrideController unarmed;
	public AnimatorOverrideController sword1H;

	[Header("Reference Index")]
	public List<IconReference> icons;
	public List<ItemReference> items;

	// Public Methods
	public Texture2D GetIcon(IconReferenceIndex index)
	{
		return icons.Find(x => x.index == index).icon;
	}

	public Item GetItem(ItemReferenceIndex index)
	{
		return items.Find(x => x.index == index).item;
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
	HumanIcon
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
	RustySword
}