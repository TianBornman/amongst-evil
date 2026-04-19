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
- Duration < 0 = infinite
- `IsExpired` when `elapsed >= duration`
- Stack vs. refresh controlled in `CharacterEffects`
- Event interfaces on `EffectInterfaces.cs`: `IOnHit`, `IOnTakeHit`, `IOnKill`, `IOnDeath`, `IOnTick`
- Concrete effects: `BurnOnHitEffect` (applies burning), `BurningEffect` (DoT tick), `HealOnKillEffect`

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
