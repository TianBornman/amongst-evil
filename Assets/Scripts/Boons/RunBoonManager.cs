using Midevil.Effect;
using Midevil.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Midevil.Boons
{
	public class RunBoonManager : Singleton<RunBoonManager>
	{
		[Header("Pool")]
		[Tooltip("All BoonCard SOs that can appear in this game.")]
		public List<BoonCard> allBoons = new();

		[Header("Offer Settings")]
		public int cardsPerOffer = 3;

		[Header("Rarity Gating")]
		[Tooltip("Beat index at which Refined-rarity cards become eligible to appear.")]
		public int refinedUnlockBeat = 3;
		[Tooltip("Beat index at which Rare-rarity cards become eligible.")]
		public int rareUnlockBeat = 5;

		[Header("Rarity Weights")]
		[Tooltip("Relative draw weight when a card of that rarity is in the eligible pool.")]
		public int commonWeight = 100;
		public int refinedWeight = 30;
		public int rareWeight = 15;

		// Tracking
		private readonly Dictionary<BoonCard, int> picksPerRun = new();
		private readonly List<TrackedEffect> appliedEffects = new();

		// Lifecycle — called by PartyManager
		public void BeginRun()
		{
			ClearAppliedEffects();
			picksPerRun.Clear();
		}

		public void EndRun()
		{
			ClearAppliedEffects();
			picksPerRun.Clear();
		}

		// Hook for future polish (ability cooldown refresh on Breath end, etc.)
		public void NotifyBreathEnded() { }

		// Right-click the RunBoonManager component in the scene Inspector to fire
		// a manual offer for testing. Only works in Play mode while a mission is running.
		[ContextMenu("Debug — Offer Boons Now")]
		public void DebugOfferBoonsNow()
		{
			if (!Application.isPlaying)
			{
				Debug.LogWarning("[RunBoonManager] Debug offer ignored — must be in Play mode.");
				return;
			}
			if (PartyManager.Instance == null || PartyManager.Instance.PartyMembers == null
				|| PartyManager.Instance.PartyMembers.Count == 0)
			{
				Debug.LogWarning("[RunBoonManager] Debug offer ignored — no party present.");
				return;
			}
			if (allBoons == null || allBoons.Count == 0)
			{
				Debug.LogWarning("[RunBoonManager] Debug offer ignored — `allBoons` is empty. Author a BoonCard SO and add it to the manager.");
				return;
			}

			OfferBoons(rareUnlockBeat);
		}

		// Mission runners call this at their beat triggers (Breath / Chaos timer / Relic kill milestone).
		public void OfferBoons(int beatIndex)
		{
			var party = PartyManager.Instance;
			if (party == null) return;

			var members = party.PartyMembers;
			if (members == null || members.Count == 0) return;

			var aliveMembers = members.Where(m => m != null && m.IsAlive).ToList();
			if (aliveMembers.Count == 0) return;

			var aliveClasses = new HashSet<BrotherClass>();
			foreach (var m in aliveMembers)
				if (m.identity != null) aliveClasses.Add(m.identity.brotherClass);

			var maxRarity = ResolveRarityCap(beatIndex);

			var pool = allBoons
				.Where(c => c != null && c.effect != null)
				.Where(c => c.rarity <= maxRarity)
				.Where(c => c.requiredClass == BrotherClass.None || aliveClasses.Contains(c.requiredClass))
				.Where(c => GetPickCount(c) < c.maxPicksPerRun)
				.ToList();

			if (pool.Count == 0) return;

			var draw = DrawWeighted(pool, cardsPerOffer);

			var offers = new List<BoonOffer>();
			foreach (var card in draw)
			{
				PartyCharacter recipient = null;

				if (card.targeting == BoonTargeting.Single)
				{
					var candidates = aliveMembers
						.Where(m => card.requiredClass == BrotherClass.None
									|| (m.identity != null && m.identity.brotherClass == card.requiredClass))
						.ToList();
					if (candidates.Count == 0) continue;
					recipient = candidates[Random.Range(0, candidates.Count)];
				}

				offers.Add(new BoonOffer { card = card, recipient = recipient });
			}

			if (offers.Count == 0) return;

			UiManager.Instance.ShowBoonPicker(offers, OnPicked);
		}

		public void OnPicked(BoonOffer offer)
		{
			if (offer == null || offer.card == null)
			{
				UiManager.Instance.HideBoonPicker();
				return;
			}

			picksPerRun[offer.card] = GetPickCount(offer.card) + 1;

			if (offer.card.targeting == BoonTargeting.Creed)
			{
				foreach (var member in PartyManager.Instance.PartyMembers)
				{
					if (member == null || !member.IsAlive) continue;
					ApplyTo(offer.card, member);
				}
			}
			else if (offer.recipient != null)
			{
				ApplyTo(offer.card, offer.recipient);
			}

			UiManager.Instance.HideBoonPicker();
		}

		// Private

		private void ApplyTo(BoonCard card, PartyCharacter target)
		{
			if (card.effect == null || target == null) return;

			var effect = card.effect.CreateRuntime();
			target.effects.AddEffect(effect);
			appliedEffects.Add(new TrackedEffect { character = target, effect = effect });
		}

		private void ClearAppliedEffects()
		{
			foreach (var tracked in appliedEffects)
			{
				if (tracked.character == null || tracked.effect == null) continue;
				tracked.character.effects.RemoveEffect(tracked.effect);
			}
			appliedEffects.Clear();
		}

		private int GetPickCount(BoonCard card)
		{
			return picksPerRun.TryGetValue(card, out int n) ? n : 0;
		}

		private BoonRarity ResolveRarityCap(int beatIndex)
		{
			if (beatIndex >= rareUnlockBeat) return BoonRarity.Rare;
			if (beatIndex >= refinedUnlockBeat) return BoonRarity.Refined;
			return BoonRarity.Common;
		}

		private List<BoonCard> DrawWeighted(List<BoonCard> pool, int n)
		{
			var picks = new List<BoonCard>();
			var bag = new List<BoonCard>(pool);

			for (int i = 0; i < n && bag.Count > 0; i++)
			{
				int totalWeight = bag.Sum(c => GetRarityWeight(c.rarity));
				if (totalWeight <= 0) break;

				int roll = Random.Range(0, totalWeight);
				int cumulative = 0;
				BoonCard chosen = bag[bag.Count - 1];
				foreach (var card in bag)
				{
					cumulative += GetRarityWeight(card.rarity);
					if (roll < cumulative)
					{
						chosen = card;
						break;
					}
				}

				picks.Add(chosen);
				bag.Remove(chosen);
			}

			return picks;
		}

		private int GetRarityWeight(BoonRarity rarity)
		{
			switch (rarity)
			{
				case BoonRarity.Common: return commonWeight;
				case BoonRarity.Refined: return refinedWeight;
				case BoonRarity.Rare: return rareWeight;
				default: return 0;
			}
		}

		private class TrackedEffect
		{
			public PartyCharacter character;
			public Midevil.Effect.Effect effect;
		}
	}
}
