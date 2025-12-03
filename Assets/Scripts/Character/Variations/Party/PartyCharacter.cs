using Midevil.Models;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class PartyCharacter : Character
{
	// Editor References
	[Header("References")]
	public Transform cameraPosition;
	public List<AbilityReferenceIndex> startingAbilities;
	public Renderer indicatorRenderer;
	
	[HideInInspector] public Color color;

	// Public Properties
	public Result Results => identity.currentResult;

	// Override Methods
	protected override void Awake()
	{
		base.Awake();

		CharacterAnimAPI animAPI = GetComponentInChildren<CharacterAnimAPI>();
		animAPI.Disappear = () => PartyManager.Instance.RemovePartyMember(this);
	}

	protected override void Start()
	{
		base.Start();

		UiManager.Instance.BindPartyMemberStats(this);

		foreach (var abilityIndex in startingAbilities)
		{
			var abilityData = RefManager.Instance.GetAbility(abilityIndex);
			abilities.AddAbility(abilityData.CreateRuntime(this));
		}
	}

	public void SetIndicatorColor(Color color)
	{
		this.color = color;
		indicatorRenderer.material.color = color;
		//indicatorRenderer.material.SetColor("_EmissionColor", color * 1.1f);
		//indicatorRenderer.material.EnableKeyword("_EMISSION");
	}

	public override void AddXp(float amount, bool shareXp = false)
	{
		if (!shareXp)
			PartyManager.Instance.AddPartyXp(amount, this);
		else 
			base.AddXp(amount);
	}

	public override void ReevaluateTarget()
	{
		base.ReevaluateTarget();

		PartyManager.Instance.CheckBattle();
	}

	public override void Die()
	{
		base.Die();

		identity.Die();
	}

	//public override void AddEffect(Effect effect)
	//{
	//	base.AddEffect(effect);

	//	UiManager.Instance.AddEffect(effect);
	//}

	//public override void RemoveEffect(Effect effect)
	//{
	//	base.RemoveEffect(effect);

	//	UiManager.Instance.RemoveEffect(effect);
	//}

	public void CheckPositionChanged()
	{
		SetState(new MoveState(this));
	}
}