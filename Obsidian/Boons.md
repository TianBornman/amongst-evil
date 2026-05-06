---
tags: [systems, boons, vermeil-creed]
status: active
---
# Boons of the Brotherhood

> [!quote] Tenth Oath of the Brotherhood
> "We wield the dark not to rule, but to restrain it."

A Brother who endures long enough to take a [[Combat#3.4 Breaths Between Floors|Breath]] may briefly *channel* the corruption around them — wielding the [[Akalaer|Curse]] without becoming it. The Brotherhood calls these temporary channellings **Boons**.

They fade with the mission's end. They were never the Brother's to keep.

---

## What a Boon Is

A Boon is a **run-only buff** picked from a 3-card offer at fixed mission beats. It is applied to a specific Brother (or to the whole [[Combat|Creed]]) for the duration of the mission and torn down when the mission ends — by extraction, victory, or wipe.

A Boon is **not gear**. It does not enter the [[Blood Vault]] record. It does not survive death. It does not transfer between Creeds.

---

## Categories

| Category | Theme | Examples |
|---|---|---|
| **Sigil** | Defensive runes etched in the moment | +block, +dodge, damage reduction |
| **Hex** | Offensive curses the Brother weaponises | burn-on-hit, +crit, +damage |
| **Rite** | Utility channellings | heal-on-kill, +attack speed for the Creed |

Sigils were originally developed by [[The Sanctum Vigil|Sanctum Vigil]] scholars during the [[The Sundering of the Crown|Sundering]] — invisible, deniable, lethal. The Brotherhood preserved the practice and extended it.

Hexes draw on the same blood-rites [[Akalaer]] fathered. Rites are the Brotherhood's own innovation — small ceremonies fast enough to perform between waves.

---

## Rarity

Boons come in three tiers. Rarer tiers are stronger and only available **deeper** into a mission — a Brother needs to have endured before they can channel power that costs more.

| Rarity | Unlocks at beat | Notes |
|---|---|---|
| **Common** | 1 | Always available |
| **Refined** | 3 | Stronger effects; sometimes class-restricted |
| **Rare** | 5 | Run-defining effects; usually class-restricted; tightly capped |

Within an offer, rarer cards are weighted *less* likely to appear (Common 100 / Refined 30 / Rare 15). Rare cards are events.

---

## Class Affinity

Each Boon has a **required class**. A Boon with `requiredClass = Knight` only appears in the offer if at least one Knight is alive in the Creed. **Universal** Boons (`requiredClass = None`) can appear regardless.

When a Boon is offered, the system **pre-rolls the recipient** — a random alive Brother of the required class (or any alive Brother for universal Boons). The recipient is shown on the card. The player picks the card *as it is shown*; they do not choose which Brother receives it.

This means class composition determines *which Boons you see*. Three Assassins will get a different set of offers than two Knights and a Ranger.

---

## How a Beat Fires

Beats are mission-type-specific:

| Mission | Beat | Cap |
|---|---|---|
| **[[Combat#Purge\|Purge]]** | Start of every Breath (lull between waves) | 1 boon per Breath |
| **[[Combat#Chaos\|Chaos]]** | Every 60s of mission timer | 1 per tick |
| **[[Combat#Relic Recovery\|Relic Recovery]]** | Every 30 enemy kills | open |

At a beat:

1. Build the available pool — exclude Boons whose required class isn't alive, exclude Boons already at their per-run cap, exclude Boons above the rarity cap.
2. Weighted-draw 3 cards.
3. Pre-roll a recipient for each card.
4. Show the picker. Game pauses.
5. Player clicks one card. The Boon's `EffectData` is applied to the recipient via the standard effect system (`character.effects.AddEffect`).
6. The other two cards are gone. **No rerolls.**

If the player ignores the picker, they cannot continue the mission — Boons are not optional offerings, they are choices the Brotherhood expects to be made.

---

## End of Mission

When a run ends — extraction, victory, or wipe — every Boon applied that mission is **removed**. The recipient's effect list is cleaned of all run-scoped effects.

A Brother who returns to camp returns the same as they left, plus their level-ups. Their gear is recovered (where the [[Combat|Creed]] permits it). Their Boons are not.

> [!quote] Fourteenth Oath
> "The hour falls for all — we only slow its hand."

The Brotherhood does not believe in keeping power borrowed from the Curse. To keep it would be to bend toward what they fight against.

---

## Lore Framing

The Brotherhood does not consider Boons to be *theirs*. They are bargains — the Curse responds to a Brother's blood and willingness, and grants a temporary edge. The Brotherhood accepts because the cost is paid by the Curse itself: each Boon is a small wound in the [[Grand Clock|Clock's]] grip on local reality, and the wound closes when the Brother leaves.

Boons earned by Veilforged-rank Creeds and above are sometimes more refined — older Brothers know the right gestures, the right invocations. The Boon system itself does not change with rank, but the rare-tier Boons are intended to feel like things only an experienced Creed can survive long enough to channel.

---

## In-Game Implementation

Code lives under `Assets/Scripts/Boons/`. Data SOs live under `Assets/Data/Boons/`.

**Authoring a Boon.**

1. Right-click in `Assets/Data/Boons/` → Create → Brotherhood → Boon Card.
2. Fill in: `cardName`, `description`, `category`, `requiredClass`, `rarity`, `targeting`, `effect` (drag in any `EffectData`), `maxPicksPerRun`.
3. Add the asset to `RunBoonManager.allBoons` on the scene's `RunBoonManager` GameObject.

Most stat-only Boons can use the existing `StatModifierEffectData`. Burn-on-hit Boons reuse `BurnOnHitEffectData`. Heal-on-kill Boons reuse `HealOnKillEffectData`. New behaviours = new `Effect` subclasses; the Boon system does not care what kind of effect it carries, only that it has an `EffectData` to instantiate.

**Lifecycle.** `PartyManager.StartRun` → `RunBoonManager.BeginRun()` clears state. `PartyManager.EndRun` → `RunBoonManager.EndRun()` iterates the tracked `(character, effect)` pairs and removes each effect.

**Beats.** Each `IMissionRunner` calls `RunBoonManager.Instance?.OfferBoons(beatIndex)` at its own trigger:
- `PurgeMissionRunner.OnWaveCleared` (after starting the Breath timer)
- `ChaosMissionRunner.Tick` (when the boon timer hits zero)
- `RelicRecoveryMissionRunner.OnEnemyDied` (when kills reach the milestone)

**UI.** Reuses `Assets/UI/LevelUpUI.uxml`. The picker panel pauses the game; clicking a card resumes.

---

*See also: [[Combat]] · [[Brotherhood Progression]] · [[Relics & Gear]] · [[Blood Vault]]*
