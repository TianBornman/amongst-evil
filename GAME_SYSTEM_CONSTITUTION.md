# GAME_SYSTEM_CONSTITUTION

## Scope
This document defines the current game system contract for the codebase under `Assets/Scripts` and related runtime assets. It is descriptive and normative for future changes.

Entities named in the prompt such as `Creed` and `Tomb` do not exist in the current codebase. The lane concept exists as `Lane` (`Assets/Scripts/Lane/Lane.cs`), not `CombatLane`.

## 1) Architecture Map

### Engine and Runtime Stack
- Engine: Unity `6000.3.1f1` (`ProjectSettings/ProjectVersion.txt`)
- Scenes in build: `Assets/Scenes/Sect.unity` (hub/menu), `Assets/Scenes/Level.unity` (run/combat) (`ProjectSettings/EditorBuildSettings.asset`)
- Core packages in use: Input System, Cinemachine, URP, UI Toolkit (`Packages/manifest.json`)

### Project Structure (Runtime)
- `Assets/Scripts/Managers`: global orchestration singletons (game mode, party, combat, spawning, UI, input, persistence adapters).
- `Assets/Scripts/Character`: character core entity, states, teaming, equipment/abilities/effects composition.
- `Assets/Scripts/Party`: party aggregate and party-level states.
- `Assets/Scripts/Lane`: combat lane model.
- `Assets/Scripts/Encounter`, `Assets/Scripts/Map`: encounter definitions and spawn locations.
- `Assets/Scripts/Ability`, `Assets/Scripts/Effect`, `Assets/Scripts/Item`, `Assets/Scripts/Attacks`: combat payload systems.
- `Assets/Scripts/Buildings`: hub interactables.
- `Assets/Scripts/Models`: persistent/runtime data models.
- `Assets/Scripts/Camera`: camera state machine.
- `Assets/Scripts/Visuals`, `Assets/Scripts/Projectile`, `Assets/Scripts/VFX`: visual feedback and projectile behavior.
- `Assets/UI/Elements`: UI Toolkit custom elements.

### Core Modules and Responsibilities
- `GameManager`: global mode flags (`AtMainMenu`, `AtHub`, `IsGamePaused`) and `Time.timeScale` pause control.
- `Singleton<T>`: persistent service lifetime (`DontDestroyOnLoad`) and scene-load callback dispatch.
- `InputManager`: input-action fan-out via delegates; no domain logic.
- `PartyManager` + `Party`: party identity roster, party instantiation, movement target setting, party-level state transitions.
- `CombatManager`: battle entry, lane assignment, selected-character control, battle movement input routing.
- `SpawnManager`: encounter/boss wave spawning and enemy depletion progression.
- `Character` + `CharacterEquipment` + `CharacterAbilities` + `CharacterEffects`: entity core, stat pipeline, combat behavior, equip/ability/effect runtime.
- `UiManager` / `HubUiManager`: runtime and hub UI composition and visibility gates.
- `InventoryManager`: run vs armoury inventory separation plus armoury persistence.
- `BloodvaultManager`: character lifecycle persistence across runs.
- `RefManager`: central typed reference lookup for icons/items/effects/abilities/animation overrides.

### Design Pattern
- Primary: OOP + composition with `MonoBehaviour` components.
- Data-driven segments: `ScriptableObject` assets (`EncounterData`, `AbilityData`, `EffectData`, `UpgradeCard`).
- State modeling: explicit finite-state interfaces (`IState`) with concrete state objects for character, party, and camera.
- Architecture style: manager-centric service layer using global singletons.

### State Management Model
- Global mode state: booleans in `GameManager`.
- Scene lifecycle state: singleton `OnSceneLoaded` hooks rebind references and rebuild scene-local collections.
- Entity state: local state machines (`StateMachine`) on `Character`, `Party`, `CameraMovement`.
- Combat mode state: `CombatManager.inBattle` gate plus party/camera state transitions.
- UI state: visibility flags on instantiated `UIDocument`s and panel subtrees.

## 2) Gameplay Loop

### Core Loop: Input -> Update -> Render
1. Input arrives through `InputManager` actions (`Assets/InputSystem_Actions.inputactions`).
2. Delegates route commands to systems:
- interaction select -> `InteractionManager`
- party target set -> `PartyManager`
- ability slot use -> `Party`
- battle character selection/lane movement -> `CombatManager`
- menu toggle/map hold/escape -> UI/camera/hub managers
3. Update phase runs distributed state updates:
- state machine updates (`Character`, `Party`, `CameraMovement`)
- `NavMeshAgent` movement (`MoveState`)
- combat intent smoothing (`CombatMoveState`)
- ability cooldown ticks and effect ticks
- encounter scanning and item/projectile triggers
4. Unity rendering presents world + UI Toolkit documents + VFX.

### Exploration Flow
1. Hub (`Sect`) initializes recruitment/building interactions and hub UI.
2. Player recruits characters and equips gear.
3. `StartRun` loads `Level`, spawns party and encounter wave.
4. In-level exploration: right-click sets party waypoint; members move via party-position offsets.
5. Entering encounter detection radius starts battle.

### Combat Flow
1. `CombatManager.StartBattle(encounter)` sets party battle state and marks `inBattle = true`.
2. Party members and encounter enemies enter combat lanes.
3. Selected party member receives manual lateral (A/D) and vertical lane (W/S) input.
4. Target acquisition is trigger-based (`CharacterAttackRange`); nearest valid target is selected.
5. Attack state loops by attack speed; animation event invokes attack execution.
6. Damage, death, drops, XP, and effect hooks resolve.
7. Enemy depletion drives wave progression and results UI.

### Exploration <-> Combat Transition Model
- Exploration -> Combat: automatic on encounter overlap.
- Combat -> Exploration: no explicit in-scene transition path; battle mode flag is not reset in current runtime flow.
- Run end transitions back to hub scene via results/death actions.

## 3) Domain Model

### Core Entities
- `Character` (base combat entity)
- `PartyCharacter` (playable party member)
- `RecruitCharacter` (hub recruit interactable)
- `Party` (aggregate of party members + ability slots)
- `Lane` (`Top`, `Middle`, `Bottom`)
- `Encounter` + `EncounterData` (runtime group + asset template)
- `Item` / `ItemStats` / `ItemConfig`
- `Ability` / `AbilityData` / `AbilitySlot`
- `Effect` / `EffectData` / effect interfaces (`IOnHit`, `IOnTick`, etc.)
- `Stats` + `Buff`
- `Identity` + `Result`
- `BloodVaultEntry` / `BloodVaultData`
- `ArmouryData`
- `Building` subclasses (`BloodVault`, `Cart`)
- `SpawnPoint`, `IdlePosition`

### Key Relationships
- `PartyManager.partyIdentities` -> instantiated `PartyCharacter` members in `Party`.
- `Identity` owns persistent item configs and lifetime stats; runtime `weapon`/`armour` are non-serialized.
- `Character` composes `CharacterEquipment`, `CharacterAbilities`, `CharacterEffects`.
- `Character` belongs to one `Lane` in combat and tracks enemy `targets`.
- `Encounter` owns spawned enemy `Character` instances for battle initialization.
- `InventoryManager` separates `armouryInventory` (hub) and `runInventory` (in-run).
- `RefManager` resolves indices to concrete assets.

### Persistence Model
- JSON file persistence (`Application.persistentDataPath`):
- `bloodvault.json` via `BloodvaultManager`
- `armoury.json` via `InventoryManager`
- `PlayerPrefs`: `MusicVolumeKey`
- `ScriptableObject` assets for authored gameplay templates.
- `Identity.currentResult` is transient (`[NonSerialized]`) and rolled into `lifeTimeResult` on save.

## 4) Combat System Analysis

### Lane Logic
- Lane set is discovered from scene objects in `CombatManager.OnSceneLoaded`.
- Starting lane assignment chooses first lane without same-team occupants; fallback is first lane.
- Lane occupancy list is manually updated when assigning or moving characters.
- Vertical movement is lane-index clamp across exactly three lane enums.

### Movement Constraints
- Horizontal movement modifies `combatPositionIntent.x` for selected party character during battle.
- Vertical movement is blocked when `selectedCharacter.CanMove == false`.
- `CombatAttackState` sets `CanMove = false`; `CombatMoveState` restores `CanMove = true`.
- Character position converges to intent via `Vector3.SmoothDamp`.
- Lane change modifies Z to lane transform Z; X/Y intent preserved.

### Auto Behaviors
- Encounter auto-start: periodic overlap scan (`0.5s`) in `EncounterDetectionRange`.
- Target auto-acquisition: trigger enter/exit in `CharacterAttackRange`.
- Auto target selection: nearest alive target in allowed `targetTeams`.
- Auto attack loop: attack timer = `1 / attackSpeed`, repeated while target alive.
- Attack execution strategy:
- melee -> direct `Damage`
- ranged -> projectile spawn and collision damage
- Auto effect processing: per-frame effect timer tick and optional `IOnTick` behavior.

### Class Starting Logic
- Base character startup:
- obtains component references
- selects `IdleState` or `MoveState` from `startIdle`
- applies identity level/xp and identity equipment configs
- recalculates final stats
- `PartyCharacter` startup additionally binds UI and grants `startingAbilities` from reference indices.
- Recruitment startup creates/randomizes `Identity` and spawns recruit prefabs at building idle points.

### Combat Invariants (Must Hold)
- A dead character (`DeadState`) cannot exit state.
- `Character.IsAlive` is strictly `stats.health > 0`.
- Health is clamped to `[0, maxHealth]`.
- Combat lane enum domain is fixed to `{Top, Middle, Bottom}`.
- Attack execution requires non-null alive targets by contract.
- Party battle start assumes at least one party member for selected-character initialization.
- Combat intent object (`combatPositionIntent`) exists for characters after `EnterCombat`.

## 5) Data Flow

### Where Game State Is Stored
- Global/session: singleton managers (`GameManager`, `PartyManager`, `CombatManager`, `SpawnManager`, `InventoryManager`, UI managers).
- Per-entity runtime: `Character` fields (`stats`, `buffs`, `currentEffects`, `currentAbilities`, `target`, `lane`, state).
- Persistent meta-progression: `BloodvaultManager` + `InventoryManager` JSON.
- Authored static data: ScriptableObject assets.

### How State Is Mutated
- Direct method calls on managers and components.
- State transitions via `SetState(new ...)`.
- Stat/equipment/effect mutation through `CharacterEquipment`, `CharacterEffects`, and `Stats.Recalculate`.
- Scene transitions through `SceneManager.LoadScene`.
- UI toggles through visibility state and data binding.

### Event Dispatch Model
- No central event bus.
- Dispatch paths are:
- input callbacks (Input System delegates)
- Unity physics callbacks (`OnTriggerEnter/Exit`)
- animation events (`CharacterAnimAPI.OnAttack`, `.OnAbility`, `.OnDisappear`)
- scene loaded callback via `Singleton<T>.OnSceneLoaded`
- direct function invocation chains between managers/components

### AI Logic Location
- Encounter activation AI: `EncounterDetectionRange` overlap loop.
- Combat target selection AI: `Character.AddTarget/CalculateTarget`.
- Combat repetition AI: `CombatAttackState` timing and re-entry.
- Non-combat locomotion AI: `MoveState` destination to assigned idle position.
- No behavior tree/ECS planner; logic is component-local and trigger-driven.

## 6) Risk Points

### Tightly Coupled Areas
- Manager mesh coupling: managers call each other directly through global singletons.
- Character-to-manager coupling: death, drops, XP, UI side effects are initiated from `Character` paths.
- UI coupling: UI elements call hub/party/inventory managers directly.
- Shared gating booleans (`AtHub`, `AtMainMenu`, `inBattle`) affect many systems.

### Fragile Systems
- `CombatManager.inBattle` is set true on battle start and has no explicit reset path.
- Lane occupancy bookkeeping is manual; stale references are possible when entities disappear.
- Input unsubscription mismatch exists for `SelectBattleCharacter` (subscribed on `performed`, unsubscribed on `canceled`).
- `StateMachine.Update()` assumes a non-null current state.
- Ability input mapping depends on scaled float values in input bindings.

### Performance Hotspots
- Encounter scanning uses repeated `Physics.OverlapSphere` per encounter.
- Hover/select system raycasts each frame in `InteractionManager.Update`.
- Effect and ability tick loops run per character every frame.
- Frequent list filtering/order operations in target recalculation and stat recomposition paths.

## 7) Constitution

### Non-Negotiable System Rules
1. `Singleton<T>` managers are the authoritative cross-scene runtime services and are persistent across scene loads.
2. Scene role split is fixed: hub/menu logic in `Sect`, run/combat logic in `Level`.
3. Party composition is identity-driven (`PartyManager.partyIdentities`) and runtime party members are instantiated from it.
4. Character combat behavior is state-machine controlled; direct transform driving outside state logic breaks contract.
5. Lane combat model is three-lane (`Top/Middle/Bottom`) with explicit lane membership tracking.
6. Gear and effects apply through equipment/effect systems, not by direct stat-field edits.
7. Persistent progression data flows through JSON managers (`BloodvaultManager`, `InventoryManager`) and not scene objects.

### Architectural Boundaries
- Managers coordinate systems; domain components own local mechanics.
- `RefManager` is the canonical source for index-to-asset resolution.
- ScriptableObject classes define authored templates; runtime instances are created from them.
- UI documents and custom UI elements are presentation/adaptor layers; gameplay state authority remains in managers/entities.

### Gameplay Invariants
1. Character death is terminal for that runtime entity state (`DeadState.CanExit == false`).
2. HP never exceeds max HP and never drops below zero.
3. Targeting only considers alive members of allowed `targetTeams`.
4. Attack cadence is derived from attack speed timer and animation event invocation.
5. Battle entry assigns both teams into lanes before active combat loop.
6. Party XP sharing for party members occurs through `PartyCharacter.AddXp` routing.
7. Run completion routes through results/death flow and hub scene return.

### Extension Rules
1. New abilities must follow `AbilityData` (asset) + `Ability` (runtime) creation pattern.
2. New effects must follow `EffectData` + `Effect` runtime model and optional effect interfaces.
3. New attacks must implement `IAttackExecutor` and be selected through `AttackExecutorResolver`.
4. New recruit/playable variants must derive from existing `Character` variation hierarchy and preserve identity/equipment bootstrap path.
5. New persisted progression fields must be represented in existing JSON persistence models or their explicit successors.
6. New global systems requiring scene lifecycle hooks must use the singleton scene-load pattern already used by managers.

### Anti-Patterns to Avoid
- Bypassing manager orchestration and mutating global mode flags from arbitrary components.
- Writing to `stats` base fields directly after initialization instead of using buffs/effects/equipment recompute flow.
- Mixing hub inventory and run inventory semantics.
- Creating hidden alternate persistence sources outside current JSON + PlayerPrefs channels.
- Skipping lane membership updates when moving/assigning combatants.
- Introducing alternate combat loops that bypass target trigger acquisition and state-machine cadence.
