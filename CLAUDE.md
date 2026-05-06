# Amongst Evil — Claude Project Config

## Memory

**Always activate Serena memories at the start of every session.**
Serena memory files live in `.serena/memories/`. Read them before making suggestions or edits.
Use `serena` MCP tools to read/write memories if available; otherwise read `.serena/memories/*.md` directly.

Memory files:
- `architecture.md` — patterns (Singleton, StateMachine, Strategy, SO split)
- `systems.md` — combat, stats, abilities, effects, items, spawning, leveling
- `managers.md` — all singleton managers and InputManager delegates
- `ui.md` — UIToolkit, UIDocuments, custom elements
- `scenes_and_flow.md` — Sect hub, Level dungeon, run lifecycle
- `lore.md` — world lore, Grimace Brotherhood, Grand Clock, key figures & locations

## Obsidian Vault

The project's design brain lives in `Obsidian/` at the repo root. It contains lore notes (Akalaer, Grand Clock, Brotherhood, characters, locations) and system notes (Combat, Blood Vault, Relics & Gear).

- **Read before deciding:** when a task touches lore, world-building, characters, or any system documented in the vault, read the relevant `Obsidian/*.md` files first.
- **Write after changing:** when lore is introduced/changed or a documented system is meaningfully altered, update or create the matching `.md` in `Obsidian/`. Use Obsidian-style `[[wikilinks]]` between notes when cross-referencing.
- Treat the vault as a peer to `.serena/memories/` and this `CLAUDE.md` for "keep memory current" guidance.

**Documentation is part of every task.** Whenever you change a system, add a feature, or alter lore, update the relevant docs in the same task — do not leave it for "later". The three places to keep in sync:
1. `Obsidian/*.md` — design intent, lore, system-level explanation
2. `.serena/memories/*.md` — code-level pointers (file paths, manager responsibilities, system summaries)
3. `CLAUDE.md` — quick-reference rules and directory map

If a change touches code, also update Serena memories and CLAUDE.md. If it touches design or lore, also update the Obsidian note.

## Project Overview

Amongst Evil is a Unity 6 (URP) action-RPG with party-based combat and roguelike progression.

- **Engine:** Unity 6, Universal Render Pipeline
- **Language:** C# (.NET)
- **UI system:** UI Toolkit (UIElements), data binding via `SetValueWithoutNotify` / `SetBinding`
- **Nav:** Unity NavMesh for character pathfinding
- **Camera:** Cinemachine state-machine cameras (explore vs. battle)
- **Input:** Unity New Input System (`InputActionAsset`)
- **Persistence:** JSON files in `Application.persistentDataPath` (`bloodvault.json`, `armoury.json`) + `PlayerPrefs` for settings

## Scenes

| Scene | Purpose |
|-------|---------|
| `Sect.unity` | Hub — recruit characters, manage armoury, access BloodVault |
| `Level.unity` | Dungeon run — exploration, combat, boss encounter, results |

## Key Directories

```
Assets/
  Scripts/
    Base Behaviours/   Singleton<T>, StateMachine
    Character/         Character base + states + party/recruit variants
    Ability/           Abstract Ability + concrete types (Fireball, HealPotion)
    Effect/            Abstract Effect + interfaces + concrete types (Burn, Heal)
    Attacks/           IAttackExecutor strategy pattern (melee / ranged)
    Item/              Item pickup, ItemStats, slots, rolling
    Party/             Party state machine + formation positions
    Managers/          All singleton managers (see below)
    Models/            Pure data: Stats, Identity, Buff, Result, ItemConfig, BrotherClass, ClassData
    Camera/            CameraMovement state machine + Cinemachine states
    Map/               SpawnPoint marker
    Encounter/         Encounter ScriptableObject (enemy group definitions)
    Upgrade Card/      UpgradeCard ScriptableObject (DEPRECATED — replaced by Boons)
    Boons/             In-run Boons — BoonCard SO, RunBoonManager, BoonOffer, enums
    Mission/           Mission data class, MissionType / MissionDifficulty enums, MissionGenerator
    Progression/       Sect Spiral of the Veil — SectProgressManager, SectRankData/SpiralProgression SOs, RankRequirement subclasses, StandingRewardTable
    Helpers/           ItemHelper, NameGenerator, ColorHelper, AudioHelper
    Visuals/           DamageNumber, HealthbarVisual
    Buildings/         Building base, BloodVault, Cart
    Projectile/        Projectile movement + collision
  Data/
    Abilities/         Fireball.asset, Health Potion.asset
    Effects/           Burn On Hit.asset, Heal On Kill.asset
    Encounters/        Zombie Feast.asset, Zombie Boss.asset
    Upgrade Cards/     DEPRECATED — replaced by Data/Boons/
    Boons/             BoonCard SOs (one per boon)
    Classes/           ClassData SOs (Knight, Ranger, Assassin)
  Prefabs/
    Characters/        Party Character, enemies (Slime, Zombie, Barbarian Boss), Recruitment Human
    Items/             Iron Armour, Rusty Sword, Wooden Bow, Beer Mug
    Buildings/         Tent, Blacksmith, BloodVault, Fire Place, Horse Carriage, Archer Tower
    UI/                GameUI, StatsUI, LevelUpUI, ItemPickUpUI, ResultsUI, SettingsUI, MissionBoardUI, etc.
    Projectiles/       Arrow.prefab
```

## Singleton Managers

| Manager | Responsibility |
|---------|---------------|
| `GameManager` | Game state: menu / hub / pause tracking |
| `PartyManager` | Party recruitment + run lifecycle |
| `SpawnManager` | Enemy wave spawning + boss logic |
| `InventoryManager` | Item armoury with JSON persistence |
| `InputManager` | Input action routing → `Action` delegates |
| `UiManager` | UIDocument management + binding |
| `AudioManager` | Music volume + PlayerPrefs persistence |
| `LevelUpManager` | **Deprecated stub.** Brother level-ups are passive in `Character.LevelUp` (stat scale + heal). |
| `RecruitManager` | NPC spawning in hub; rolls a `BrotherClass` for new identities |
| `BloodvaultManager` | Persistent dead-character storage |
| `DamageNumberManager` | Floating damage text pool |
| `RefManager` | Centralized asset references (items, effects, abilities, icons, classes) |
| `InteractionManager` | Hover / click detection (`IInteractable`) |
| `HubManager` | Hub scene management |
| `HubUiManager` | Hub UI state (incl. Spiral of the Veil panel) |
| `SectProgressManager` | Sect-wide Spiral of the Veil progression; rank, standing, requirements, ascension ceremony |
| `RunBoonManager` | In-run Boon pool — offers cards at mission beats, applies effects, tears them down at run end |

## Architecture Patterns

- **Singleton<T>** — all managers; `DontDestroyOnLoad` with scene-aware handling
- **StateMachine / IState** — Characters, Party, Camera all use the same generic `StateMachine` + `IState` (Enter/Exit/Update/CanExit)
- **Strategy (IAttackExecutor)** — `MeleeAttackExecutor` / `RangedAttackExecutor` selected by `AttackExecutorResolver`
- **ScriptableObject data** — `AbilityData`, `EffectData`, `Encounter`, `UpgradeCard`; runtime instances created via `CreateRuntime(owner)`
- **Component composition** — `CharacterAbilities`, `CharacterEquipment`, `CharacterEffects` each on the same GameObject
- **Observer / Action events** — `InputManager` exposes `public Action` properties; `Effect.OnCountChanged`

## Core Systems Quick Reference

### Combat
- Damage = `baseDamage` × crit multiplier if roll < `critChance`
- Block/dodge chance zeroes damage entirely
- On-hit effects fire from `CharacterEffects` after damage is dealt

### Stats & Buffs
- `Stats` holds base values; `Recalculate(baseStats, buffs[])` sums deltas
- Level scaling: `value * 1.10^level`
- Size modifier inversely scales attack/move/dodge speed
- `Character.baseScaledStats` is recomputed alongside `stats` each `RecalculateStats` and represents the character's stats *without* equipped gear (still includes level, size, and non-gear buffs). Drives the "Base / +Gear / Total" view on the recruit panel's Stats tab. Gear buffs are identified by `buff.id == item.id` (set in `CharacterEquipment.EquipItem`).
- Recruit & in-run StatsUI both use the tabbed `Character Gear.uxml` (Gear tab + Stats tab). Tab wiring + stats population live in `Midevil.UI.Elements.CharacterGearPanel` static helper. Custom `StatRowElement` renders one stat row (label / base / +gear delta / total).

### Abilities
- `Ability.IsReady` = `cooldownTimer <= 0 && remainingCharges > 0`
- Bound to UI slots via GUID (`AbilitySlot`)
- 2 ability slots per party member

### Effects / Statuses
- Duration < 0 = infinite; `IsExpired` when `elapsed >= duration`
- Each `EffectData` has: `group` (string), `stackPolicy` (`Refresh/Replace/Reject/Allow`), `duration`, `icon`, `itemPrefix`. Subclasses fill base fields via `PopulateBase(effect)` in `CreateRuntime()`.
- Same-group conflict resolution lives in `CharacterEffects.AddEffect`. Empty group = unrelated to anything else, always allowed alongside.
- Event interfaces: `IOnHit`, `IOnTakeHit`, `IOnKill`, `IOnDeath`, `IOnTick`. `OnApply/OnRemove` for setup/teardown.
- Built-ins: `StatModifierEffect` (flat Buff), `StatMultiplierEffect` (multiplies baseStats), `OutlineEffect`, `DropOnDeathEffect` (the *only* loot mechanism — Character has no drop logic itself), `CompositeEffect`, `BurnOnHitEffect`/`BurningEffect`, `HealOnKillEffect`. Compose tier effects (Cursed/Blighted/etc.) as a `CompositeEffectData` referencing a `StatMultiplierEffectData` + an `OutlineEffectData` + a `DropOnDeathEffectData`. **Do not** author a new `Effect` subclass for stat changes.

### Brother Classes
- `BrotherClass` enum (`Models/BrotherClass.cs`): `None / Knight / Ranger / Assassin`. Locked at recruitment.
- `ClassData` SO (assets in `Data/Classes/`) carries: `baseStats`, `themeColor`, `icon`, `starterWeapon` (`ItemReferenceIndex`).
- `RefManager.classes` lists them; `RefManager.GetClass(BrotherClass)` resolves one.
- `Identity.brotherClass` stored. `Identity.Randomize(class)` sets the starter weapon. `Character.SetupIdentity → ApplyClassProfile` overrides prefab `baseStats` with `classData.baseStats.Clone()`.
- `RecruitManager` rolls Knight/Ranger/Assassin uniformly when generating a new identity.

### In-Run Boons (`Assets/Scripts/Boons/`, data in `Data/Boons/`)
- Run-only buffs — applied during a mission, cleared on `EndRun`. **Never persist across runs.**
- `BoonCard` SO: `cardName`, `description`, `category` (Sigil/Hex/Rite), `requiredClass`, `rarity` (Common/Refined/Rare), `targeting` (Single/Creed), `EffectData effect`, `maxPicksPerRun`.
- `RunBoonManager : Singleton<>` owns the pool (`allBoons`), tracks per-run pick counts, applies effects via `character.effects.AddEffect`, tears them down on `EndRun`.
- Filter chain at each beat: alive-class → max-picks → rarity-cap → weighted draw (Common 100 / Refined 30 / Rare 15 by default).
- Rarity gating: Common from beat 1, Refined unlocks at `refinedUnlockBeat=3`, Rare unlocks at `rareUnlockBeat=5`.
- Recipient is **pre-rolled** server-side and shown on the card. Player picks the card-as-shown — no Brother-picker, no rerolls.
- Beats: **Purge** = start of every Breath. **Chaos** = every `MissionConfig.chaosBoonInterval` (60s default). **Relic Recovery** = every `MissionConfig.relicBoonKillInterval` kills (30 default).
- UI: reuses `LevelUpUI` UIDocument. `UiManager.ShowBoonPicker(offers, onPick) / HideBoonPicker()`. Rarity tint via USS classes `rarity-common / rarity-refined / rarity-rare`.

### Breaths (between Purge waves)
- `IMissionRunner.IsReadyForNextWave` — `SpawnManager` polls between waves and stalls `StartWave` while it's false.
- `PurgeMissionRunner` enters a Breath after each non-final wave (`MissionConfig.breathSeconds` = 10s). Objective text shows `Breath — Ns`. Boon offer fires at Breath start. `Time.timeScale = 0` (boon picker pause) freezes the breath timer naturally.

### Effect Application (chance + grouping)
- `EffectApplication` struct: `{ EffectData effect; float chance; float weight }`
- `EffectApplicationGroup` SO: `{ string label; bool pickOnlyOne; float skipChance; List<EffectApplication> entries }`
  - `pickOnlyOne=false` — each entry rolled independently against its `chance`
  - `pickOnlyOne=true` — weighted pick using `weight`; `skipChance` rolls the group off entirely
- Designer hooks effects to a character via `Character.spawnEffects: List<EffectApplicationGroup>` on the prefab. Each group is `Apply(this)`-ed in `Start` after `RecalculateStats`.
- Variant tiers are SOs in `Assets/Data/Effects/Variants/` referenced from a single shared `EffectApplicationGroup` SO; that SO is dragged onto every enemy prefab's `spawnEffects`. **No variant code, no enums, no tables.** Adding a tier = author a new `CompositeEffectData` and add it to the group.

### Missions
- `Mission` (plain data class in `Assets/Scripts/Mission/`): `title`, `MissionType` (Purge/RelicRecovery/Chaos), `MissionDifficulty` (I–X), `flavorText`. Maps to Obsidian `Combat.md §2.2 / §2.3` and Spiral threat ratings.
- `MissionGenerator.GenerateBatch(count, min, max)` — randomises type + difficulty, picks a title/flavor from per-type pools.
- Cart → `HubUiManager.ShowMissionBoardUI()`; selecting a card calls `HubManager.StartRun(mission)`, which sets `PartyManager.Instance.CurrentMission` before loading `Level.unity`.
- `SpawnManager.OnSceneLoaded` builds a `MissionConfig` and creates an `IMissionRunner` via `MissionRunnerFactory`. Runner controls win condition; SpawnManager runs the spawn loop. Three runners: `PurgeMissionRunner` (fixed waves), `ChaosMissionRunner` (timer), `RelicRecoveryMissionRunner` (touch the relic to win — Warden deferred).
- `MissionConfig.Build(mission)` derives `waveCount / baseEnemyCount / enemyCountScalingPerWave / healthMul / damageMul / spawnIntervalMul / chaosTimerSeconds` from Threat I–X.
- HUD: `UiManager.SetObjectiveText` / `SetTimerText` drive `#Objective` and `#Timer` labels in `GameUI.uxml`.
- Relic placement uses NavMesh sampling around the party (same as enemies) — tunable via `SpawnManager.relicMinDistance / relicMaxDistance`. `Relic` triggers on `PartyCharacter` collision.

### Spawning
- `Encounter` SO defines a list of enemy prefabs + optional scenery
- `SpawnManager` places enemies in a circle via rejection sampling
- Boss spawns after all normal waves are cleared

### Persistence
- `BloodvaultManager` ↔ `bloodvault.json` (character identities)
- `InventoryManager` ↔ `armoury.json` (items)
- Settings ↔ `PlayerPrefs` (music volume, etc.)

## Unity Editor Limitations — How to Handle References

Claude cannot interact with the Unity Editor. This means:
- Cannot drag assets/prefabs into Inspector fields
- Cannot create or modify scene hierarchies
- Cannot configure component references via the Inspector
- Cannot set up `[SerializeField]` references between objects in a scene or prefab

**The rule:** Never design solutions that require complex manual editor wiring. Keep reference setup as simple as possible, and always tell the user exactly what to assign where.

**What to do instead:**
- Write `[SerializeField]` fields clearly named so it's obvious what goes in them
- At the end of any task that introduces new Inspector references, include a **"Unity Setup"** section listing every field to assign and what to drag into it, e.g.:
  > **Unity Setup:**
  > - `SpawnManager` prefab → `spawnPoints` field: drag in all SpawnPoint objects from the scene
  > - `GameUI` UIDocument prefab → `gameUiDocument` field on `UiManager`
- Prefer runtime lookup over Inspector wiring when it keeps things simpler (e.g. `GetComponent<T>()`, `FindFirstObjectByType<T>()`, `Singleton.Instance`)
- Use `RefManager` for asset references (items, effects, abilities, icons) rather than scattering `[SerializeField]` across many scripts

## Coding Conventions

- No comments unless the WHY is non-obvious
- Managers are singletons — access via `Manager.Instance`
- New systems follow the ScriptableObject data / runtime instance split
- UI uses UIToolkit — no UGUI `Canvas` / `Text`
- NavMesh for all character movement; no Rigidbody locomotion
- States implement `IState`; register with the character's `StateMachine`
