using Midevil.Effect;
using Midevil.Helpers;
using Midevil.Item;
using Midevil.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterEquipment : MonoBehaviour
{
	// Editor Variables
	[Header("References")]
	public Transform weaponPos;
	public SkinnedMeshRenderer helmetSkin;
	public SkinnedMeshRenderer armourSkin;
	public SkinnedMeshRenderer glovesSkin;
	public SkinnedMeshRenderer leggingsSKin;
	public SkinnedMeshRenderer bootsSkin;
	public Transform idlePos;

	// Protected Variables
	protected Character character;

	// Public Methods
	public void SetupIdentity()
	{
		if (character.identity.armourConfig.index > 0)
		{
			EquipItemConfig(character.identity.armourConfig);
		}

		if (character.identity.weaponConfig.index > 0)
		{
			EquipItemConfig(character.identity.weaponConfig);
		}
	}

	public virtual void EquipItem(ItemStats item)
	{
		item.buff.id = item.id;
		character.AddBuff(item.buff);

		foreach (var effectData in item.effects)
		{
			var effect = effectData.CreateRuntime();
			effect.id = item.id;
			character.effects.AddEffect(effect);
		}

		if (item.abilityIndex != AbilityReferenceIndex.None)
		{
			var abilityData = RefManager.Instance.GetAbility(item.abilityIndex);

			var ability = abilityData.CreateRuntime(character);
			ability.id = item.id;

			character.abilities.AddAbility(ability);
		}

		switch (item.type)
		{
			case ItemType.Helmet:
				break;
			case ItemType.Armour:
				if (character.identity.armour != null)
					UnequipItem(character.identity.armour);

				character.identity.armour = item;
				character.identity.armourConfig.Set(item);

				var skinnedRenderer = item.visual.GetComponentInChildren<SkinnedMeshRenderer>();
				armourSkin.sharedMesh = skinnedRenderer.sharedMesh;
				armourSkin.materials = skinnedRenderer.sharedMaterials;

				armourSkin.gameObject.SetActive(true);
				break;
			case ItemType.Weapon:
				if (character.identity.weapon != null)
					UnequipItem(character.identity.weapon);

				character.identity.weapon = item;
				character.identity.weaponConfig.Set(item);
				character.stats.projectile = item.projectile;

				Instantiate(item.visual, weaponPos);
				UpdateAnimations(item.animationType);
				break;
			case ItemType.Offhand:
				break;
			default:
				break;
		}

		character.RecalculateStats();
	}

	public virtual void UnequipItem(ItemStats item)
	{
		var buff = character.buffs.Where(buff => buff.id == item.id).FirstOrDefault();
		character.RemoveBuff(buff);

		List<Effect> effectsToRemove = new();

		foreach (var effect in character.currentEffects.Where(ef => ef.id == item.id))
			effectsToRemove.Add(effect);

		foreach (var effect in effectsToRemove)
			character.effects.RemoveEffect(effect);

		var ability = character.currentAbilities.FirstOrDefault(ability => ability.id == item.id);

		if (ability != null)
			character.abilities.RemoveAbility(ability);

		switch (item.type)
		{
			case ItemType.Helmet:
				break;
			case ItemType.Armour:
				character.identity.armour = null;
				character.identity.armourConfig.Clear();
				armourSkin.gameObject.SetActive(false);
				break;
			case ItemType.Weapon:
				character.identity.weapon = null;
				character.identity.weaponConfig.Clear();
				RemoveChildren(weaponPos);
				UpdateAnimations(ItemAnimationType.Unarmed);
				break;
			case ItemType.Offhand:
				break;
			default:
				break;
		}
	}

	// Private Methods
	public void Awake()
	{
		character = GetComponent<Character>();
	}

	private void EquipItemConfig(ItemConfig config)
	{
		var item = RefManager.Instance.GetItem(config.index);
		var itemStats = item.stats.Clone();
		itemStats.effectIndex = config.effectIndex;
		itemStats.shouldRoll = false;

		ItemHelper.Setup(itemStats);

		EquipItem(itemStats);
	}

	private void RemoveChildren(Transform transform)
	{
		foreach (Transform child in transform)
			Destroy(child.gameObject);
	}

	private void UpdateAnimations(ItemAnimationType type)
	{
		switch (type)
		{
			case ItemAnimationType.Unarmed:
				character.animator.runtimeAnimatorController = RefManager.Instance.unarmed;
				break;
			case ItemAnimationType.Sword1H:
				character.animator.runtimeAnimatorController = RefManager.Instance.sword1H;
				break;
			case ItemAnimationType.Sword2H:
				break;
			case ItemAnimationType.Bow:
				character.animator.runtimeAnimatorController = RefManager.Instance.bow;
				break;
			case ItemAnimationType.Shield:
				break;
			default:
				break;
		}
	}
}
