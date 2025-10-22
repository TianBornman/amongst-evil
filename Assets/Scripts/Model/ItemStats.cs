using System;
using UnityEngine;

[Serializable]
public class ItemStats
{
	public string name;
	[TextArea] public string description;
	public Buff buff;
	public float dropChance;
}