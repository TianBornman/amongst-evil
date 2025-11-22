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

	// Public Methods
	public Texture2D GetIcon(IconReferenceIndex index)
	{
		return icons.Find(x => x.index == index).icon;
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