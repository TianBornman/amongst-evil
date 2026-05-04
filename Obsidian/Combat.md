---
tags: [combat, brd, vermeil-creed]
version: 0.1
status: internal-dev-reference
---
# The Vermeil Creed — Combat System

> [!quote]
> "We stand where others flee. We kill what cannot die. We use what should never be touched."

*v0.1 · Internal Dev Reference*

---

## 0. World Context

Every mission in *The Vermeil Creed* is a small battle in a war that has lasted seven centuries.

When **[[Akalaer]]** — the Arch-Warden and keeper of the world's balance — was murdered by **[[Alexandr the Iron Tyrant]]**, the Curse spread across the kingdoms and the **[[Grand Clock]]** at **[[Kareth Vall]]** began to run faster. Its sands, once steady and eternal, now fall faster with each act of corruption, each death, each sin.

The **[[The Sanctum Vigil|Sanctum Vigil]]**, the order formed by Alexandr to manage the Curse's aftermath, uncovered the truth — that the King himself had caused it — and were declared traitors for knowing it. After a decade of civil war, the survivors gathered at the ruins of **[[High Thalenor]]** and reforged themselves as the Vermeil Creed: an order unbound by crown, morals, or conscience, sworn to slow the fall of the hourglass.

The Brotherhood does not seek victory. They seek delay — and understanding.

> [!quote] Fourteenth Oath of the Brotherhood
> "The hour falls for all — we only slow its hand."

Every mission run, every Ichor Core processed at camp, every Brother who falls in the field and leaves their Tomb behind — it is all in service of this. Not to win. To slow.

---

## 1. Overview

The combat loop of **The Vermeil Creed** is a top-down auto-battler in the vein of *Vampire Survivors*, adapted for squad-based play. Before each mission, the player assembles a **Creed** of 1–3 Brothers and deploys them into cursed territory. The mission unfolds in real time: the Brothers move together under WASD control while enemies spawn in escalating waves around them.

The loop is intentionally brutal and attrition-focused — reflecting the Brotherhood's ethos. Death is not failure; it is lore. Every Brother who falls is remembered. Every Brother who survives returns to camp changed.

> [!quote] Seventh Oath of the Brotherhood
> "The living serve. The dead teach. The blood remembers."

---

## 2. Pre-Mission: Creed Assembly

Before a mission begins, the player visits the Brotherhood camp and selects which Brothers to send. This selection screen is the strategic layer of the game — choosing the right Brothers for the right mission matters.

### 2.1 Party Size

| Brothers Selected | Behaviour |
|---|---|
| **1 Brother** | Solo run. Higher risk, higher relic drop chance. Harder missions penalise this heavily. |
| **2 Brothers** | Balanced. Brothers cover each other's blind spots. Recommended for most content. |
| **3 Brothers** | Full Creed. Best survivability. Some epic enemies only appear in 3-Brother runs. |

A Brother who has fallen cannot be selected. Their entry in the [[Blood Vault]] remains but is sealed — a record of who they were, not who they are.

### 2.1.1 Brother Inspection — Gear & Stats Sheet

Clicking a Brother in camp opens their personal sheet, a two-tab panel:

- **Gear** — equipment slots (helmet, weapon, gloves, leggings, armour, off-hand, amulet, boots) plus health and XP bars. Items can be equipped from the armoury and unequipped back to it.
- **Stats** — name, level, XP progress, and a per-stat breakdown grouped under *Vitals* / *Offense* / *Mobility*. Each row shows three numbers side-by-side: **Base** (what the Brother would have without any gear, but still including level, size, and any persistent non-gear buffs), **+Gear** (the contribution from currently equipped items, color-coded — green for positive, red for negative, dimmed when zero), and **Total** (live combat value). Equipping or unequipping in the Gear tab updates the Stats tab immediately, so the player can see exactly what each item is doing for that Brother.

The same panel is mirrored in-mission via the Stats overlay (default keybind: menu toggle), letting the player audit a Brother's combat numbers mid-run after applying upgrade cards or picking up loot.

### 2.2 Mission Briefing

Each mission has a stated objective drawn from one of three mission types, along with a visible threat rating and a recommended Creed size. Players can choose to ignore the recommendation at their own peril.

- **Threat Rating** — scales from I (*Whispering Ashes*-tier) to X (*Clockless*-tier); maps directly to the [[Brotherhood Progression#The Spiral of the Veil|Spiral of the Veil]] ranks
- **Recommended Creed Size** — shown as a guide, not a lock
- **Known Curse Type** — informs which enemy variants are likely to appear
- **Grand Clock Pressure** — a mission modifier representing how strongly the [[Grand Clock]]'s acceleration bleeds into local reality; high pressure means enemy spawns accelerate faster and [[#4.3 Hourglass Ichor (Special Spawn)|Hourglass Ichor]] appears more often

### 2.3 Mission Types

There are three mission types. Each changes the win condition, pacing, and how the player interacts with the wave system.

#### Purge
*Examples: Clear a Rift, Purify a Cursed Site, Burn the Nest*

The standard mission type. The party must survive a set number of waves to complete the objective. Once all waves are cleared the mission ends and surviving Brothers extract automatically.

- **Win condition:** survive all waves
- Wave count and density is fixed and visible in the mission briefing
- Extraction is available during Breaths but forfeits the mission completion reward
- Most accessible mission type — recommended for new Creeds

#### Relic Recovery
*Examples: Recover the Sealed Tome, Retrieve the Warden's Sigil*

An endless mission with no fixed wave count. Enemies spawn continuously while the Brothers search the map for a hidden relic. Once a Brother locates the relic, a **Relic Warden** — a powerful epic enemy — spawns to defend it. The party must defeat the Warden before they can extract with the relic.

- **Win condition:** find the relic, defeat the Relic Warden, extract
- Waves are endless until the relic is found — attrition is the primary threat
- The Relic Warden is always an epic enemy, stat-scaled to the mission's threat rating
- Extraction is available at any Breath before the relic is found, but the relic is forfeit

#### Chaos
*Examples: The Grand Toll, Last Stand at [[Kareth Vall]], Blood Tithe*

A pure timed survival mission. No objective beyond endurance. Enemies spawn continuously and escalate without pause. When the timer expires the mission ends, surviving Brothers extract automatically, and all loot gathered is kept.

- **Win condition:** survive until the timer expires
- **No Breaths** — waves are unrelenting for the full duration
- Epic enemy spawn rate is significantly higher than other mission types
- Loot is abundant but risky to collect
- Highest-risk mission type — recommended only for established Creeds

---

## 3. Core Combat Loop

Once deployed, the game runs continuously until the mission objective is completed, the party is wiped, or the player chooses to extract.

### 3.1 Movement

- **WASD** sets the party's *optimal position* — a formation anchor that Brothers pathfind toward. The player is moving a target point, not directly dragging the party.
- Each Brother independently auto-targets the nearest enemy within their attack range — Brothers do not share a target.
- **Positioning is the primary player skill** — funnelling enemies into kill zones, avoiding encirclement, and staying near loot.
- If a Brother gets stuck (collision, knockback, terrain), they catch up to the anchor independently — the rest of the party does not wait.

### 3.2 Combat — Auto-Attack

Brothers attack automatically based on their individual weapon loadouts and abilities. The player does not manually fire; instead, strategy comes from party composition, positioning, and ability timing.

- Each Brother has a primary weapon with its own range, attack speed, and damage profile
- Abilities (active and passive) fire on cooldown or trigger off conditions
- Brothers can **synergise** — a Brotherhood Scholar's hex marks a target, and a Warden's melee strike detonates it

### 3.3 Wave Structure

Enemies spawn in discrete waves around the party. The tempo of waves is governed by a mission timer and the **[[Grand Clock]] Pressure** modifier.

| Phase | Wave Behaviour | Approx. Timing |
|---|---|---|
| **Opening** | Light pressure — basic cursed spawns, sparse density | 0:00 – 2:00 |
| **Rising** | Increased spawn rate, enemy variety expands, flanking patterns begin | 2:00 – 6:00 |
| **Surge** | Heavy waves, overlapping spawns, elite enemies appear | 6:00 – 10:00 |
| **Grand Toll** | Maximum intensity — boss-level spawns, no breaks between waves | 10:00+ |

> [!note]
> Wave timings compress under high Grand Clock Pressure. A high-pressure mission may reach Surge phase in under three minutes.
>
> **Grand Toll** is named after the [[Grand Clock|Grand Clock's]] final chime — the sound the world will hear when the last grain of sand falls. Reaching Grand Toll phase in a mission is a miniature experience of what the Brotherhood is fighting to prevent.

### 3.4 Breaths Between Floors

Between wave surges, there are brief lulls — the Brotherhood calls them **Breaths**. During a Breath:

- No new enemies spawn for **8–15 seconds**
- Loot from the previous wave can be collected
- The player can choose to **extract** and end the mission with whatever is carried
- A brief ability cooldown refresh occurs for surviving Brothers

> [!quote]
> "Those who retreat may return with relics, blood, and coin. Those who stay and fall leave behind only their Tomb."

---

## 4. Enemy System

### 4.1 Standard Enemies

Standard cursed enemies spawn in groups and are the baseline combat pressure. They are drawn from the **Curse-born catalogue** — nightmare-spawned beasts, wraith-husks, plague-turned villagers — lore-appropriate horrors born from the Curse that spread when [[Akalaer]] was killed. Each enemy type is documented in the [[Crypt of Knowledge]]; the Brotherhood's field knowledge of their weaknesses, behaviours, and spawn conditions comes from centuries of Tombs written by fallen Brothers.

- Scale in number, not just stats, as waves progress
- Drop minor loot: blood shards, consumables, small relic fragments
- Die quickly to coordinated Brothers but are dangerous in mass

### 4.2 Enemy Variants

Every enemy type has four variants representing escalating levels of curse corruption. Variants are rarer and stronger than the base enemy, and are the **only source of gear drops** in the game. Drops are never guaranteed — they are chance-based, with odds improving at higher tiers.

| Tier          | Rarity    | Stat Scaling                  | Gear Drop                     | Visual Tell                                  |
| ------------- | --------- | ----------------------------- | ----------------------------- | -------------------------------------------- |
| **Cursed**    | Common    | 1.5× HP, +10% speed           | None                          | Faint dark aura                              |
| **Blighted**  | Uncommon  | 2× HP, +25% speed, +20% dmg   | ~25% chance                   | Visible mutation, sickly glow                |
| **Corrupted** | Rare      | 3.5× HP, +40% speed, +50% dmg | ~10% chance                   | Heavily warped, ichor trails                 |
| **Forsaken**  | Legendary | 5× HP, +60% speed, +100% dmg  | ~5% chance; best gear quality | Fully consumed, distorted form, unique audio |

> [!info] Design Intent
> Variants should feel like **events**. A Forsaken enemy appearing mid-wave demands a decision — push through it for the gear drop chance, or reposition and survive. Standard Cursed enemies never drop gear, keeping the risk-reward calculation clear.

### 4.3 Hourglass Ichor (Special Spawn)

The **Hourglass Ichor** is a unique enemy class — condensed temporal corruption left over from ritual sites, broken relics, or fractured rifts. It is a direct manifestation of the [[Grand Clock|Grand Clock's]] bleed into reality: the Clock's acceleration made physical, erupting from the environment where the Curse is most active.

It is not a standard wave enemy. It does not spawn in waves. It erupts.

- Appears when a rift is left unsealed too long, or when a Grand Clock Pressure event triggers
- **Accelerates time locally** — it speeds up all enemies within its aura, replicating the Clock's effect at a human scale
- Destroying it drops high-value loot — including **[[Relics & Gear#Ichor Cores|Ichor Cores]]**, the only material that can be used to slow Grand Clock Pressure at camp
- Ignoring it stacks **Ichor Corruption** — permanent wave tempo acceleration for the rest of the mission

> [!note] Lore
> The Brotherhood calls this enemy the Hourglass Ichor because that is precisely what it is: hourglass sand given malevolent form. Where the [[Grand Clock]] at [[Kareth Vall]] bleeds temporal energy into the world passively, the Ichor is an active crystallisation of that bleed — a wound in the local fabric of time that will worsen if not closed.

---

## 5. Death, Memory, and the Blood Vault

Death in *The Vermeil Creed* is permanent and meaningful. A Brother who falls in the field is gone — but not forgotten. Their legacy is encoded into the [[Blood Vault]], the game's living memorial.

### 5.1 When a Brother Dies

- The Brother is immediately removed from the active party
- A death notification appears: their name, their rank, the mission, the enemy that killed them
- The surviving Brothers take a brief **morale dip** (minor debuff for the next 30 seconds)
- If all Brothers die, the mission ends in a **Wipe** — all carried loot is lost

> [!quote] Sixth Oath of the Brotherhood
> "Only blood remembers what the tongue must forget."

### 5.2 The Blood Vault Record

Every Brother is recorded in the Blood Vault from the moment they take the **[[Blood Vault#The Rite of Names|Rite of Names]]** — a silent ritual where they write their name in blood into the Scrolls of Mortality, binding their essence permanently to the Brotherhood's record. Living and dead alike. It is not a graveyard. It is a living chronicle: who they are now, who they were before, and what they have done in service of the Brotherhood. When a Brother dies, their entry does not change category — it simply stops updating.

- Each entry tracks: original name, alias, current rank, missions survived, kill count, relics recovered, status (**Active / Fallen**), and if fallen — cause of death and mission
- Entries cannot be deleted or overwritten — every Brother who ever served is permanently on record
- Active Brothers show their current stats and history in real time; Fallen Brothers show a final snapshot
- Each entry links to the Brother's personal **Tomb** — a living record of their discoveries, items, and history that grows with every mission. If the Brother dies, the Tomb is sealed and preserved as-is.

The Blood Vault is visible in camp at all times. It should feel like a Brotherhood roster that quietly becomes a memorial — the same names, the same record, just some of them no longer changing.

### 5.3 Survival — Return to Camp

Brothers who survive a mission extract and return to camp, carrying whatever loot was collected during the Breaths. At camp:

- Loot is processed — relics stored, blood shards added to the Creed's reserves; see [[Relics & Gear]]
- Surviving Brothers gain experience and may progress in rank along the **[[Brotherhood Progression#The Spiral of the Veil|Spiral of the Veil]]**
- Brothers may be **injured (not dead)** — requiring recovery time before the next mission
- Mission discoveries are added to the **[[Crypt of Knowledge]]** archive — written up from the Brothers' field observations, eventually preserved as permanent record

> [!quote]
> "The living bring strength to the Creed, and the dead bring wisdom to the Crypt."

---

## 6. Loot & Relics

Loot is the primary reward for enduring the field. It is collected during Breaths and carried out only if at least one Brother survives.

| Loot Type | Source | Effect |
|---|---|---|
| **Blood Shards** | Standard enemies | Currency — used at camp to upgrade abilities and equipment |
| **Consumables** | Standard enemies, chests | One-use items: healing draughts, hex grenades, sigil scrolls |
| **Relic Fragments** | Standard enemies (rare) | Combine at camp to forge complete relics |
| **Relics** | Blighted / Corrupted / Forsaken variants (chance-based) | Equipped items with passive or active effects on Brothers |
| **Ichor Cores** | Hourglass Ichor | Rare crafting material; used to slow Grand Clock Pressure |

> [!warning]
> Loot dropped in the field despawns after **60 seconds** if not collected during a Breath. In high-pressure missions, loot management becomes a secondary skill.

---

## 7. Extraction

The player may choose to extract at the end of any Breath phase. Extraction is a deliberate choice — a Brother who extracts carries all currently held loot out of the mission.

- Extraction prompt appears at the start of every Breath
- Choosing to extract ends the mission immediately and returns surviving Brothers to camp
- Extracting before the objective is complete forfeits the mission's main reward but preserves loot gathered
- **Extraction is always the safe play.** Staying always risks more but offers more.

The tension between extraction and continuation is the core loop decision. It mirrors the Brotherhood's own philosophy: survival serves the Creed, but so does sacrifice.

---

## 8. Design Notes & Open Questions

The following are considerations for the current development phase. These should be revisited as implementation progresses.

### 8.1 Current Implementation Status

- [x] WASD party movement
- [x] Enemy wave spawning *(wave structure, timing tuning ongoing)*
- [x] Variant tier system — Normal / Cursed / Blighted / Corrupted / Forsaken (see §8.3)
- [x] Mission Board UI — type + difficulty selection on cart interact (see [[Missions]])
- [x] Mission-type behaviour — Purge (fixed waves), Chaos (timer), Relic Recovery (touch-to-win, Warden deferred)
- [x] Difficulty scaling — enemy count, HP×, Dmg×, spawn interval, Chaos timer all driven by Threat I–X
- [ ] Relic Warden epic spawn (Relic Recovery currently touch-to-win)
- [ ] Grand Clock Pressure as a stand-alone modifier on top of difficulty
- [ ] Unique per-tier behaviours beyond stat scaling (TBD)
- [ ] Blood Vault persistent record
- [ ] Loot drop system
- [ ] Extraction mechanic

### 8.3 Variant Tiers Are Effects

Variant tiers (§4.2) are **not** a separate system — they are just `EffectData` assets applied through the generic Effect system. There is no `EnemyVariant` enum, no variant table, no variant code paths in `SpawnManager`.

**Effect system architecture:**
- `EffectData` (SO) carries `group` (string tag), `stackPolicy` (Refresh / Replace / Reject / Allow), `duration`, `icon`. `CreateRuntime()` produces a runtime `Effect`.
- `Effect` subclasses receive events via `IOnHit / IOnTakeHit / IOnKill / IOnDeath / IOnTick` and lifecycle via `OnApply / OnRemove`.
- `CharacterEffects.AddEffect` resolves same-group conflicts via `stackPolicy`. Two fire DoTs sharing `group="fire"` will not stack ugly — second one refreshes (or replaces / is rejected) per its policy.
- Built-in effect types: `StatModifierEffect` (buff/debuff), `OutlineEffect` (visual tell), `CompositeEffect` (bundles children), `BurnOnHitEffect` + `BurningEffect`, `HealOnKillEffect`.

**Application:**
- `EffectApplication` = `{ EffectData effect; float chance; float weight }`.
- `EffectApplicationGroup` (SO) holds a list of applications and a `pickOnlyOne` toggle. With `pickOnlyOne=true`, only one application from the group can apply per roll (weighted) — that is how a single enemy gets *one* variant tier (or none). With `pickOnlyOne=false`, each entry rolls independently — that is how an enemy might receive multiple unrelated effects.
- `Character.spawnEffects: List<EffectApplicationGroup>` on the prefab. Each group is rolled on `Start`.

**Authored assets (already in repo):** `Assets/Data/Effects/Variants/`

```
Variants/
├── Cursed.asset              (Composite, group="variant-tier", stackPolicy=Replace)
├── Cursed Stats.asset        (StatMultiplier — HP×1.5, move×1.10, xp×1.2)
├── Cursed Outline.asset      (faint dark purple, width 3)
│                             (no Loot child — Cursed never drops gear)
├── Blighted.asset            (Composite)
├── Blighted Stats.asset      (HP×2.0, move×1.25, dmg×1.20, xp×1.6)
├── Blighted Outline.asset    (sickly green, width 4)
├── Blighted Loot.asset       (DropOnDeath — pool: [Rusty Sword], chance=0.25)
├── Corrupted.asset           (Composite)
├── Corrupted Stats.asset     (HP×3.5, move×1.40, dmg×1.50, xp×2.5)
├── Corrupted Outline.asset   (ichor purple, width 5)
├── Corrupted Loot.asset      (DropOnDeath — pool: [Iron Armour, Rusty Sword], chance=0.10)
├── Forsaken.asset            (Composite)
├── Forsaken Stats.asset      (HP×5.0, move×1.60, dmg×2.00, attack×1.10, size×1.10, xp×4)
├── Forsaken Outline.asset    (distorted red, width 6)
├── Forsaken Loot.asset       (DropOnDeath — pool: [Wooden Bow, Iron Armour], chance=0.05)
└── Variant Roll.asset        (ApplicationGroup — pickOnlyOne, skipChance=0.7, weights 70/20/8/2)
```

**Drop system:** `Character` has *no* drop logic. Loot is delivered by `DropOnDeathEffect` — a generic effect carrying a list of items + a single chance. On death the effect rolls its chance once; if it passes, one item from the pool is picked uniformly and instantiated. No second roll, no per-item filter. To make any character drop loot — under any condition (variant tier, boss kill, item bonus, etc.) — attach a `DropOnDeath` effect to it.

For variant tiers, the loot pool is *tier-aware* per the design intent: Forsaken drops higher-quality gear than Blighted, not just rarer rolls. To retune what a tier drops, edit its `<Tier> Loot.asset` (change the `items` list or the `chance`). To wire a different loot pool to a specific enemy type — either author a new `DropOnDeath` effect for that enemy and add it to its `spawnEffects`, or build an enemy-specific composite that bundles an enemy-specific loot effect.

**Tier reference values** (matches §4.2):

| Tier | Rel. Weight | HP × | Move × | Dmg × | Outline tell |
|---|---|---|---|---|---|
| Cursed | common | 1.5 | 1.1 | 1.0 | faint dark |
| Blighted | uncommon | 2.0 | 1.25 | 1.2 | sickly green glow |
| Corrupted | rare | 3.5 | 1.4 | 1.5 | ichor purple |
| Forsaken | legendary | 5.0 | 1.6 | 2.0 | distorted red/gold |

The "Normal" no-tier outcome is produced by raising `skipChance` on the application group — it's not authored as an effect.

> [!info] Why this design
> Variants share the same plumbing as every other effect (item-applied, ability-applied, on-hit, status). Adding a new tier — or any new combat behavior — is asset authoring, not code. Effects can also be dropped onto party members, bosses, or scripted spawns without changing anything.

### 8.2 Open Questions & Resolutions

- **Individual HP bars confirmed.** Each Brother has their own health pool, tracked and displayed separately. See §5.1 for death behaviour.
- **Morale debuff on death** is communicated via the character portraits in the bottom-left HUD. A debuff state will be indicated on the relevant Brother's face — exact visual TBD with art.
- **Party cohesion:** Brothers always move as one unit toward the formation anchor. A Brother who falls behind catches up independently. No mechanic to hold or reposition individual Brothers.
- **Gear drop rates confirmed** — exceedingly rare by design: Blighted ~25%, Corrupted ~10%, Forsaken ~5%. Relic definitions, item types, and quality bands to be detailed in a separate [[Relics & Gear]] doc.
- **Blood Vault gameplay effect confirmed.** Long-term goal: permanent camp buffs inherited from previous Brothers — legacy carries forward. Full legacy and progression system to be defined in a separate [[Brotherhood Progression]] doc.

---

## Appendix: Brotherhood Oaths Referenced

*Full oath list with context: [[Oaths of the Brotherhood]]*

The following oaths from the Brotherhood's founding creed are relevant to the combat and death systems:

> [!quote] Third Oath
> "We are the hand that slows the fall of the hourglass."

> [!quote] Fourth Oath
> "In blood, we learn. In death, we remember. In darkness, we preserve."

> [!quote] Sixth Oath
> "Only blood remembers what the tongue must forget."

> [!quote] Seventh Oath
> "The living serve. The dead teach. The blood remembers."

> [!quote] Twelfth Oath
> "Every fallen Brother marks a page in eternity."

> [!quote] Fifteenth Oath
> "For as long as one Brother stands, the world has not yet fallen."

---

*Vermeil Creed — Internal Dev Reference · Do Not Distribute*
