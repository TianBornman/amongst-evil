---
tags: [systems, classes, vermeil-creed]
status: active
---
# Brother Classes

A Brother enters the [[Blood Vault]] as one of three trades — what they were *before* they took the [[Blood Vault#The Rite of Names|Rite of Names]]. The Brotherhood does not retrain. A knight stays a knight. A ranger stays a ranger. The class is locked at recruitment for the Brother's lifetime.

> [!info] Aliases vs Classes
> Classes are *what a Brother arrives as*. **Aliases** (The Warden, The Hollow, Ashblade, Seraph Null — see [[Blood Vault#The Rite of Names|Rite of Names]]) are *what a Brother becomes* after surviving long enough to be renamed by their Creed's Keeper. Classes are the floor; aliases are the ceiling. The two systems are independent — a Knight may one day earn the alias *The Warden* (or any other), but they will always have been a Knight.

---

## The Three Starter Classes

### Knight
*The protector. The line that holds.*

Knights came to the Brotherhood from the broken orders of the old crown — soldiers who had sworn to crowns that broke their oaths first. They wear what plate the Brotherhood can scavenge. They block. They stay standing.

- **Role:** Tank — front of the formation, soaks pressure
- **Weapon:** Sword (one-handed)
- **Profile:** Highest HP, highest block chance, slowest movement
- **Boon affinity:** Sigils — defensive runes, taunts, damage reduction

### Ranger
*The horizon. The killing line at distance.*

Rangers came from the woods, the ruins, the long roads — hunters and outriders who had learned to live in places kingdoms forgot. They carry bows that the Brotherhood's smiths reinforce with hex-steel where they can. They kill before being seen.

- **Role:** Ranged damage — keeps a clean line of sight, picks off threats
- **Weapon:** Bow
- **Profile:** Mid HP, longest attack range, mid speed
- **Boon affinity:** Hexes (range-leaning) and Rites (utility) — multi-shot, pierce, slow-on-hit

### Assassin
*The blade between heartbeats.*

Assassins came from the shadow trades — knife-fighters, spies, killers the old crown employed and then disowned. The Brotherhood took them in because the Brotherhood does not trust clean hands. They are fast. They die quickly. They take more with them than they leave.

- **Role:** Glass cannon — burst damage, fast repositioning, low survivability
- **Weapon:** Daggers (sword-rig animation for now)
- **Profile:** Lowest HP, highest crit chance, fastest movement, highest dodge
- **Boon affinity:** Hexes (crit-leaning) — backstab, heal-on-kill, life-steal

---

## Why Three (For Now)

The current set is Knight / Ranger / Assassin to cover **front line / back line / flanker** — the minimum that produces meaningful composition decisions. Additional classes (e.g. a Pyromancer / mage archetype focused on AoE) can be added later as `ClassData` SOs without code changes; the Boon system already filters by class via the `requiredClass` field on every card.

---

## Recruitment

Recruits appear in the Sect hub spawned by [[#In-Game Implementation|`RecruitManager`]]. Each recruit's class is rolled uniformly across the three at spawn — the player sees a mix in any given hub session.

Class is shown on the recruit's panel (icon, name, theme colour). Clicking through to the Stats tab shows the per-class profile applied. Players who want a balanced [[Combat|Creed]] can refuse to recruit until a missing class appears; the Brotherhood does not penalise patience.

---

## Class & Boons

Classes shape the Boon pool a Creed sees in a mission. A Boon with `requiredClass = Knight` only appears in offers if at least one Knight is alive in the party. Universal Boons (`requiredClass = None`) appear for any Creed. See [[Boons]] for the full system.

This means choosing your Creed shapes which power-ups you can build into during a run. Three Knights is a different run from three Assassins, even on the same mission.

---

## In-Game Implementation

Code: `Assets/Scripts/Models/BrotherClass.cs` (enum), `Assets/Scripts/Models/ClassData.cs` (SO).
Data: `Assets/Data/Classes/` (one `ClassData` SO per class).

**`BrotherClass` enum:**

| Value | Class |
|---|---|
| 0 | None *(legacy / no override)* |
| 1 | Knight |
| 2 | Ranger |
| 3 | Assassin |

**`ClassData` SO carries:**

- `classType` (the enum)
- `className`, `description`, `icon`, `themeColor`
- `baseStats` — the Stats profile applied at character spawn
- `starterWeapon` — `ItemReferenceIndex` auto-equipped at recruit time

**Plumbing:**

- `Identity.brotherClass` stores the assigned class (serialised with [[Blood Vault]] persistence).
- `Identity.Randomize(class)` sets the starter weapon's `weaponConfig.index` so the Brother spawns equipped.
- `Character.SetupIdentity → ApplyClassProfile` looks up the class via `RefManager.GetClass(...)` and replaces the prefab's `baseStats` with `classData.baseStats.Clone()`. Existing variant tier effects (Cursed/Blighted/Corrupted/Forsaken) and Boons stack on top of the class profile via the standard effect system.
- `RecruitManager.RollBrotherClass()` picks Knight / Ranger / Assassin uniformly for new identities. Existing Vault entries keep whatever class they were originally rolled with.

---

*See also: [[Combat]] · [[Boons]] · [[Blood Vault]] · [[Brotherhood Progression]]*
