# Core Game Systems

## Combat
- Damage = baseDamage × crit multiplier if `Random.value < critChance`
- Block / dodge chance → damage reduced to 0
- On-hit effects fire from `CharacterEffects` post-damage
- Damage numbers: `DamageNumberManager` (singleton pool)

## Stats & Buffs (`Assets/Scripts/Models/Stats.cs`, `Buff.cs`)
- `Stats` holds all numeric properties
- `Stats.Recalculate(baseStats, List<Buff>)` sums all buff deltas
- Level scaling formula: `value * 1.10^level`
- Size modifier inversely scales `attackSpeed`, `moveSpeed`, `dodgeChance`
- Health is preserved (clamped to new max) on recalculate

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
- Level-up: heals % of max health + opens `LevelUpManager` card selection
- `LevelUpManager` picks 3 random `UpgradeCard` SOs, pauses game, applies chosen buff

## Targeting
- `CharacterDetectionRange` component triggers on enter/exit
- Closest-target priority within range
- Teams (`Team.cs` enum) define valid targets (player vs. enemy)
- Re-evaluates on death or range loss

## Persistence
- `BloodvaultManager` ↔ `bloodvault.json` — dead/stored character identities
- `InventoryManager` ↔ `armoury.json` — item armoury
- Settings ↔ `PlayerPrefs` — music volume, etc.
- Paths under `Application.persistentDataPath`
