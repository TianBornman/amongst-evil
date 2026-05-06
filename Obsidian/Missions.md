---
tags: [systems, missions, vermeil-creed]
status: active
---
# Missions & The Mission Board

The **Mission Board** is the player's pre-run choice screen. It surfaces in the hub when the player interacts with the cart and is the first place the [[Combat#2.2 Mission Briefing|Mission Briefing]] from the combat doc becomes a tangible system in the game.

> [!note] Status
> v0.2 — types and difficulties drive the run. Same map for every mission. No Recommended Creed Size lock, no Grand Clock Pressure modifier yet, no Relic Warden (relic is touch-to-win for now), no Breaths or extraction button.

---

## What a Mission Is

A `Mission` is a plain data record with four fields:

| Field | Meaning |
|---|---|
| `title` | Flavor name shown on the card (e.g. "Burn the Nest") |
| `type` | `Purge` · `RelicRecovery` · `Chaos` — see [[Combat#2.3 Mission Types]] |
| `difficulty` | `I`–`X` — Threat Rating mapped to the [[Brotherhood Progression#The Spiral of the Veil|Spiral of the Veil]] |
| `flavorText` | Short briefing line shown beneath the threat |

Missions are not persisted. Each time the board opens, a fresh batch is rolled.

## The Board

- The **cart** in the hub is the entry point. Interacting with it opens the board UI.
- The board generates **3 missions** by default (configurable on `HubUiManager`).
- Difficulty is currently rolled in the range **I–III**, since the Spiral rank progression isn't implemented yet. The full I–X range is supported in code; widen the range when ranks land.
- Each card is clickable. Selecting a mission stores it on `PartyManager.CurrentMission`, leaves the hub, and loads `Level.unity`.
- Cancel or Esc closes the board with no run started.

## Generation Rules (current)

- `type` — uniform random across the three [[Combat#2.3 Mission Types|mission types]].
- `difficulty` — uniform random across the configured min/max range.
- `title` and `flavorText` — picked from per-type pools (see `MissionGenerator.cs`). Pools draw directly from the examples in the Combat doc:
  - **Purge** — *Clear the Rift*, *Purify the Cursed Site*, *Burn the Nest* …
  - **Relic Recovery** — *Recover the Sealed Tome*, *Retrieve the Warden's Sigil* …
  - **Chaos** — *The Grand Toll*, *Last Stand at the Veil*, *Blood Tithe* …

## How Each Type Runs (current implementation)

`SpawnManager` reads `PartyManager.Instance.CurrentMission` on scene load, builds a `MissionConfig` from it, and instantiates an `IMissionRunner` via `MissionRunnerFactory`. The runner owns the win condition.

| Type | Spawn behaviour | Win condition |
|---|---|---|
| **Purge** | Fixed wave count from `MissionConfig.waveCount`; clears wave by wave | All waves cleared |
| **Chaos** | Endless spawning, no wave cap, faster spawn rate | Mission timer (`chaosTimerSeconds`) reaches 0 |
| **Relic Recovery** | Endless spawning; one `Relic` placed by NavMesh sampling around the party (`relicMinDistance` … `relicMaxDistance`) | Player walks into the relic (touch-to-win — Warden deferred) |

Lose condition is unchanged: full party wipe ends the run.

## Difficulty Scaling

`MissionConfig.Build(mission)` computes per-run scaling. Threat I–X drive every value:

| Field | Formula |
|---|---|
| `waveCount` *(Purge)* | `4 + 2 × d` |
| `baseEnemyCount` | `15 + 5 × d` |
| `enemyCountScalingPerWave` | `8 + d` |
| `healthMul` | `1 + 0.20 × (d − 1)` |
| `damageMul` | `1 + 0.15 × (d − 1)` |
| `spawnIntervalMul` *(Grand Clock Pressure)* | `Lerp(1.0 → 0.5)` across I → X |
| `chaosTimerSeconds` | `120 + 30 × d` |

`SpawnManager.healthScalingPerWave` still applies — enemies grow tougher within a run *and* across difficulties.

## HUD

`GameUI.uxml` carries an always-present `#Timer` and `#Objective` label, both hidden by default:

- `UiManager.SetObjectiveText(string)` — runner sets it on `Begin` (and again at every wave start)
- `UiManager.SetTimerText(float)` — only Chaos uses this; ticks every frame from the runner

## Post-Run — The Reckoning

After a run ends, the player sees the **Results** screen (per-character + total stats). Pressing **Continue** swaps to a second page: **The Reckoning** — the Sect's accounting of what just happened.

The Reckoning shows:

- **Standing With the Sect** — the value before the run, the value after, and the delta (gain/loss). Coloured green for gain, red for loss.
- **Rank** — current Sect rank name. If the run met the requirements, an ascension banner appears: *"You ascend the Spiral. <Old Rank> → <New Rank>"* or, when ascension is unlocked but pending the tent ceremony, *"The Spiral beckons — ascension to <Next Rank> awaits at the tent."*
- **The Blood Vault** — a list of fallen members ("Bound to the Vault — their names endure") and survivors ("Returned to the Sect"). On a clean run with no losses: *"None were claimed. The Vault stays silent."*

A single **Return to the Sect** button closes both pages, ends the run, and loads `Sect.unity`.

Code path: `PartyManager.FinalizeMission(success)` snapshots Sect standing/rank, calls `SectProgressManager.RecordMissionCompleted`, snapshots the post-state, and stores a `MissionAftermath` model on the party manager. `UiManager.PopulateAftermath` reads it.

Currency / resource rewards are deliberately not shown — none exist in the game yet. When loot or essence systems land, add them as a third panel next to Standing and Blood Vault.

## Spiral Tie-in

Threat Ratings on the briefing map directly to the [[Brotherhood Progression#The Spiral of the Veil|ten Spiral ranks]]. When the Creed rank system lands, the difficulty range on the board should be gated by the Creed's current rank — attempting higher than Creed rank is *not forbidden*, just likely fatal (see Brotherhood Progression).

---

*See also: [[Combat]] · [[Brotherhood Progression]] · [[Blood Vault]]*
