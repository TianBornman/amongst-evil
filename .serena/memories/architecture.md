# Architecture & Patterns

## Core Patterns

**Singleton<T>** (`Assets/Scripts/Base Behaviours/Singleton.cs`)
- All managers inherit from this
- `DontDestroyOnLoad` with scene-aware teardown
- Access via `Manager.Instance`

**StateMachine / IState** (`Assets/Scripts/Base Behaviours/StateMachine.cs`, `Assets/Scripts/Interfaces/IState.cs`)
- Generic — used by Character, Party, and Camera systems
- Interface: `Enter()`, `Exit()`, `Update()`, `CanExit()`
- Transition via `stateMachine.SetState(new SomeState(...))`

**Strategy — IAttackExecutor** (`Assets/Scripts/Attacks/`)
- `IAttackExecutor`: `Execute(AttackContext)`
- Implementations: `MeleeAttackExecutor`, `RangedAttackExecutor`
- `AttackExecutorResolver` picks based on weapon `ItemAnimationType`

**ScriptableObject data / runtime split**
- `AbilityData.CreateRuntime(owner)` → `Ability` instance
- `EffectData.CreateRuntime()` → `Effect` instance
- `Encounter` SO defines enemy groups; `SpawnManager` instantiates from it

**Component Composition on Character**
- `CharacterAbilities` — ability management + casting
- `CharacterEquipment` — equip/unequip + stat buffs
- `CharacterEffects` — apply/tick/remove status effects

**Observer / Action events**
- `InputManager` exposes `public Action` delegates (subscribed by UI, abilities, etc.)
- `Effect.OnCountChanged` — stack count UI updates

## Character States (all implement IState)
- `MoveState` — NavMesh navigation to target or idle position
- `AttackState` — basic attack timing
- `AbilityState` — ability cast timing
- `IdleState` — waiting
- `DeadState` — death

## Party States
- `ExploreState` — free camera, movement
- `BattleState` — locked camera, combat

## Camera States (Cinemachine)
- `ExploreState` — follows party center, free rotation
- `BattleState` — tighter frame on combat
