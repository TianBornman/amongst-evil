# Core Game Systems

## Missions
- `Assets/Scripts/Mission/` — `Mission` (data), `MissionType` (Purge/RelicRecovery/Chaos), `MissionDifficulty` (I–X), `MissionGenerator` (static, `GenerateBatch(count, min, max)`)
- Selected mission stored on `PartyManager.Instance.CurrentMission` before `Level` scene loads
- Cart no longer starts run directly — `Cart.Interact()` opens `HubUiManager.ShowMissionBoardUI()`; selecting a card calls `HubManager.StartRun(mission)`
- `SpawnManager.OnSceneLoaded` builds `MissionConfig.Build(mission)` and creates an `IMissionRunner` via `MissionRunnerFactory.Create(type)`. Runner owns spawn pacing flags and the win condition; SpawnManager owns the spawn loop.
- Runners: `PurgeMissionRunner` (fixed waves), `ChaosMissionRunner` (timed survival via `Tick`), `RelicRecoveryMissionRunner` (calls `SpawnManager.PlaceRelic` on Begin, touch-to-win — Warden deferred).
- Difficulty I–X scales `waveCount`, `baseEnemyCount`, `enemyCountScalingPerWave`, `healthMul`, `damageMul`, `spawnIntervalMul`, `chaosTimerSeconds` (formulas in `MissionConfig`).
- HUD: `UiManager.SetObjectiveText` and `SetTimerText` drive `#Objective` and `#Timer` in `GameUI.uxml` (both hidden by default; runner unhides as needed).
- Relic placement: NavMesh sampling around the party (same approach as enemy spawning) using `SpawnManager.relicMinDistance / relicMaxDistance / relicPlacementAttempts`. `Relic` script triggers `onTouched` on `OnTriggerEnter` with a `PartyCharacter`.
- Deferred: Relic Warden, Grand Clock Pressure modifier, Hourglass Ichor, rank-gated difficulty range, Breaths/extraction. See `Obsidian/Missions.md`.

## Combat
- Damage = baseDamage × crit multiplier if `Random.value < critChance`
- Block / dodge chance → damage reduced to 0
- On-hit effects fire from `CharacterEffects` post-damage
- Damage numbers: `DamageNumberManager` (singleton pool)

## Stats & Buffs (`Assets/Scripts/Models/Stats.cs`, `Buff.cs`)
- `Stats` holds all numeric properties
- `Stats.Recalculate(baseStats, List<Buff>)` sums all buff deltas
- `Stats.Clone()` — shallow per-field copy; used to compute "what would stats be without X buffs?" without mutating the live `stats`.
- Level scaling formula: `value * 1.10^level`
- Size modifier inversely scales `attackSpeed`, `moveSpeed`, `dodgeChance`
- Health is preserved (clamped to new max) on recalculate
- `Character.baseScaledStats` (recomputed every `RecalculateStats`) holds what `stats` would be if the character had no gear equipped — i.e. baseStats + level + size + non-gear buffs (spawn-effect tiers, future upgrade cards). Gear-applied buffs are filtered out via `IsGearBuff` (item buffs are tagged with `buff.id == item.id` in `CharacterEquipment.EquipItem`). `baseScaledStats` is the data source for the recruitment Stats tab.

## Ability System (`Assets/Scripts/Ability/`)
- `Ability.IsReady` = `cooldownTimer <= 0 && remainingCharges > 0`
- `AbilitySlot` binds via GUID — survives prefab reinstantiation
- 2 ability slots per party member
- Abilities can be attached to items (weapon grants fireball, etc.)
- Concrete abilities: `FireballAbility`, `HealPotionAbility`

## Effect / Status System (`Assets/Scripts/Effect/`)
- Duration < 0 = infinite. `IsExpired` when `elapsed >= duration`.
- Event interfaces (`EffectInterfaces.cs`): `IOnHit`, `IOnTakeHit`, `IOnKill`, `IOnDeath`, `IOnTick`. `Effect.OnApply/OnRemove` fire on add/remove.
- Each `EffectData` carries a `group` (string) and `stackPolicy` (`Refresh / Replace / Reject / Allow`). When a new effect with a non-empty group is added and one with the same group already exists, `CharacterEffects.AddEffect` resolves per the policy. Empty group = always allowed alongside others.
- `Effect.source` references the spawning `EffectData` (asset identity). `Effect.parent` references a containing `CompositeEffect` (or null).
- Subclasses populate base fields via `EffectData.PopulateBase(effect)` in `CreateRuntime()` — never copy fields by hand.
- Built-in effect types (each with a matching `EffectData`):
  - `StatModifierEffect` — generic flat buff/debuff via the `Buff` system.
  - `StatMultiplierEffect` — multiplies `baseStats` directly on apply, divides back on remove. Use for proportional changes that scale with base values (variant tiers).
  - `OutlineEffect` — sets outline color/width on apply, restores on remove.
  - `DropOnDeathEffect` (`IOnDeath`) — carries `items: List<Item>` + `chance`. On death rolls chance once; if pass, picks one item from the pool and instantiates. **Character has no drop logic itself** — all loot is delivered by this effect.
  - `CompositeEffect` — bundles multiple `EffectData` children; applying it applies all children, removing it removes all children. Use this for tier effects like "Cursed" = StatMultiplier + Outline + DropChance.
  - `BurnOnHitEffect` (`IOnHit`) — applies a configurable `EffectData` (typically `BurningEffectData`) to anything it hits.
  - `BurningEffect` (`IOnTick`) — DoT.
  - `HealOnKillEffect` (`IOnKill`) — heals killer on kill.

## Effect Application (`Assets/Scripts/Effect/EffectApplicationGroup.cs`, `Models/EffectApplication.cs`)
- `EffectApplication` struct: `{ EffectData effect; float chance; float weight }`.
- `EffectApplicationGroup` SO: `{ string label; bool pickOnlyOne; float skipChance; List<EffectApplication> entries }`.
  - `pickOnlyOne = false` (default) — every entry rolled independently against its `chance`.
  - `pickOnlyOne = true` — weighted pick across entries (using `weight`), `skipChance` lets the whole group roll nothing.
- `Character` has a `spawnEffects: List<EffectApplicationGroup>` field. On `Start` (after `RecalculateStats`), each group's `Apply(this)` is called.
- Variant tiers (Cursed/Blighted/Corrupted/Forsaken) are authored as `CompositeEffectData` SOs — there is no `EnemyVariant` enum or table in code. To make an enemy a "variant roller", drag a single `EffectApplicationGroup` SO (with `pickOnlyOne=true`) onto its `spawnEffects` list. Reuse the same SO across many prefabs.
- `SpawnManager` no longer rolls variants — it only spawns and scales by wave. Variant logic is entirely on the prefab's `spawnEffects` + the application group SO.

## Attack System (`Assets/Scripts/Attacks/`)
- `AttackContext` — data bag (attacker, target, damage, etc.)
- `MeleeAttackExecutor` — direct damage
- `RangedAttackExecutor` — spawns `Projectile.prefab` toward target
- Projectile handles own movement + collision (`Assets/Scripts/Projectile/Projectile.cs`)

## Item System (`Assets/Scripts/Item/`)
- `ItemStats` holds: damage, effects[], abilities[], itemType, animationType
- `ItemHelper.Setup()` rolls random effects onto item
- `CharacterEquipment.Equip()` applies stat buffs from item
- Animations change by `ItemAnimationType`: Unarmed / Sword1H / Bow
- Items dropped from enemies, picked up via `IInteractable`

## Spawning (`Assets/Scripts/Managers/SpawnManager.cs`, `Assets/Scripts/Encounter/Encounter.cs`)
- `Encounter` SO: list of character prefabs + optional scenery prefab + min/max count
- Spawn in circle around center via rejection sampling (prevents clustering)
- Normal run: 2–4 encounter waves
- Boss spawns after all enemies cleared
- Variant logic (Cursed/Blighted/Corrupted/Forsaken) is **not** in SpawnManager — see "Effect Application" above. SpawnManager only handles wave scheduling and per-wave HP scaling.

## Party System (`Assets/Scripts/Party/Party.cs`)
- Max 3 party members
- `PartyPosition` tracks formation slot + offset
- All members share XP (flag-controlled)
- Camera follows centroid of alive members
- Dead members return to hub (BloodVault)

## Leveling
- XP threshold: `10 * level^1.3 + 5 * level`
- XP is shared per kill: every alive Brother in the party receives the kill's XP via `Party.AddPartyXp` (each Brother tracks their own level + needed XP).
- Level-up = passive: `stats.level++`, `RecalculateStats()` (so the new level's `Scale` multiplier applies to damage / xpValue / maxHealth), `Heal(maxHealth * levelHeal)`. `identity.level` is also bumped so level persists.
- **No card pick on level-up.** `LevelUpManager` is a deprecated stub. The card UI (`LevelUpUI`) is repurposed by the boon system.
- Old `UpgradeCard` SOs in `Assets/Data/Upgrade Cards/` are dead data — replaced by `BoonCard` SOs.

## Brother Classes (`Assets/Scripts/Models/`)
- `BrotherClass` enum: `None`, `Knight`, `Ranger`, `Assassin`. Stored on `Identity.brotherClass`. Locked at recruit time.
- `ClassData` SO (`Assets/Data/Classes/`): per-class `baseStats`, `themeColor`, `icon`, `starterWeapon` (`ItemReferenceIndex`).
- `RefManager.classes` lists all `ClassData` SOs; `RefManager.GetClass(BrotherClass)` resolves one. Used by `Identity.Randomize(class)` to apply the starter weapon and by `Character.SetupIdentity → ApplyClassProfile` to override prefab `baseStats` with the class profile (cloned via `Stats.Clone`, so the SO is not mutated).
- `RecruitManager` rolls a uniform `Knight | Ranger | Assassin` for newly-spawned recruits in the hub. Existing BloodVault identities keep whatever class they had (default `None` for legacy saves).

## In-Run Boons (`Assets/Scripts/Boons/`)
- Run-only buffs picked at mission beats; cleared on `EndRun`. **Never persist across runs.**
- `BoonCard` SO: `cardName`, `description`, `category` (Sigil / Hex / Rite), `requiredClass`, `rarity` (Common / Refined / Rare), `targeting` (Single / Creed), `EffectData effect`, `maxPicksPerRun`. Lore framing: temporary curse channellings (Sigils = defense, Hexes = offense, Rites = utility).
- `RunBoonManager : Singleton<>` — owns the live pool, tracks per-run pick counts, applies effects, tears them down on `EndRun`. Reuses the existing `Effect` lifecycle (`character.effects.AddEffect/RemoveEffect`). Authored pool lives on `RunBoonManager.allBoons`.
- **Filtering**: at each beat, the available pool excludes (a) cards with `requiredClass != None` whose class isn't alive in the party, (b) cards already at `maxPicksPerRun`, (c) cards above the rarity cap (`refinedUnlockBeat=3`, `rareUnlockBeat=5`). Within the cap, weighted draw by rarity (Common 100 / Refined 30 / Rare 15).
- **Recipient pre-roll**: `Single` cards pre-pick a random alive Brother of the matching class (or any alive Brother if `requiredClass == None`); the recipient is shown on the card. `Creed` cards apply to every alive member. The player does not select the Brother — they pick or don't pick the card-as-shown. **No rerolls / banishes.**
- **Beats** by mission type (`MissionConfig`):
  - **Purge** — start of every Breath (lull between waves). `breathSeconds = 10f`. `PurgeMissionRunner` ticks the breath timer in `Tick(dt)`; `IsReadyForNextWave` returns false during the breath, stalling `SpawnManager`'s wave loop.
  - **Chaos** — every `chaosBoonInterval = 60f` seconds (driven by `ChaosMissionRunner.Tick`).
  - **Relic Recovery** — every `relicBoonKillInterval = 30` enemy kills (driven by `RelicRecoveryMissionRunner.OnEnemyDied`).
- **UI**: reuses `LevelUpUI` UIDocument. `UiManager.ShowBoonPicker(offers, onPick)` / `HideBoonPicker()`. Each card displays name, description, `Rarity Category\n→ Recipient (Class)` footer, and a rarity-coloured border (USS classes `rarity-common / rarity-refined / rarity-rare`). Card click → `RunBoonManager.OnPicked` → applies effect to recipient(s), records for run-end teardown.
- **Lifecycle**: `PartyManager.StartRun` calls `BeginRun()` (clears state). `PartyManager.EndRun` calls `EndRun()` — iterates tracked `(character, effect)` pairs and calls `RemoveEffect` on each.

## Targeting
- `CharacterDetectionRange` component triggers on enter/exit
- Closest-target priority within range
- Teams (`Team.cs` enum) define valid targets (player vs. enemy)
- Re-evaluates on death or range loss

## Sect Progression — Spiral of the Veil (`Assets/Scripts/Progression/`)
- `SectProgressManager` (singleton) tracks Sect-wide progression. Stored on `BloodVaultData.sectProgress` so it shares `bloodvault.json` (single save, no wipe). Lore: `Obsidian/Brotherhood Progression.md`.
- `SectProgressData`: `currentRank`, `standing`, `ascensionPending`, plus `killCounts`, `missionsCompleted`, `milestones` (generic counter map).
- Rank assets: `SectRankData` SOs (one per rank 1–10) referenced from a single `SpiralProgression` SO.
- A rank advances when *all* `RankRequirement` SOs in `SectRankData.requirements` are met. Concrete subclasses: `StandingRequirement`, `KillCountRequirement`, `MissionCompletedRequirement`, `MilestoneRequirement`, `CompositeRequirement` (AllOf/AnyOf/NofM). Add a new requirement type by subclassing `RankRequirement` — no enum or table changes.
- Standing rewards live on `StandingRewardTable` SO (`missionCompleteBase`, `perThreatBonus`, type multipliers, wipe penalty). `SectProgressManager.RecordMissionCompleted` consults it.
- Event hooks: `Character.Die` → `RecordKill(enemyId)` (player-team killer only). End-of-run mission recording is centralized in `PartyManager.FinalizeMission(success)` — called from `Party.RemoveMember` (wipe) and `SpawnManager.EndRun(victory:true)`. It snapshots Sect standing/rank, calls `RecordMissionCompleted`, captures the post-state into `PartyManager.lastAftermath` (a `MissionAftermath` model with standing delta, rank, ascension flags, fallen + survivor name lists).
- Ascension is a **ceremony** — when requirements pass, `OnAscensionAvailable` fires and `ascensionPending=true`. Player must click the **Tent** building in the Sect hub and press "Perform the Rite" → `PerformAscension()` mutates `currentRank`. Ceremony state persists across sessions.
- Authoring: `Tools → Progression → Create Spiral Progression Assets` scaffolds `Assets/Data/Progression/Spiral.asset`, ranks, requirement assets, and `Standing Reward Table.asset`.
- Enemy id: each enemy prefab needs `Character.enemyId` set (e.g. `zombie`, `slime`, `barbarian-boss`) for kill tracking.

## Persistence
- `BloodvaultManager` ↔ `bloodvault.json` — dead/stored character identities + `sectProgress`
- `InventoryManager` ↔ `armoury.json` — item armoury
- Settings ↔ `PlayerPrefs` — music volume, etc.
- Paths under `Application.persistentDataPath`
