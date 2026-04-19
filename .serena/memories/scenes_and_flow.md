# Scenes & Game Flow

## Scenes
- **`Sect.unity`** — Hub world
  - Recruit new characters at `RecruitManager` spawn points
  - Manage armoury at Cart building (`InventoryManager`)
  - Store/retrieve dead characters at BloodVault building (`BloodvaultManager`)
  - Hub buildings: Tent, Blacksmith, BloodVault, Fire Place, Horse Carriage, Archer Tower
  - Managed by `HubManager`, `HubUiManager`

- **`Level.unity`** — Dungeon run
  - Party explores map; enemies spawn from `SpawnManager`
  - Combat resolves via Character state machines
  - Boss spawns after all waves cleared
  - On run end: `ResultsUI` shown, stats saved, dead chars go to BloodVault
  - Managed by `GameManager`, `SpawnManager`, `PartyManager`

## Run Lifecycle
1. Start run from hub → load `Level` scene
2. `SpawnManager` spawns 2–4 encounter waves
3. Party fights; XP shared; level-ups trigger card selection
4. After all waves: boss encounter spawns
5. Boss defeated → results screen
6. Return to `Sect` hub; dead chars stored in bloodvault

## Character Types
- **PartyCharacter** — player-controlled; XP, abilities, equipment
- **RecruitCharacter** — hub NPC awaiting recruitment
- **Enemies** — Slime, Zombie, Barbarian Boss (prefabs in `Prefabs/Characters/`)

## Identity & Persistence
- `Identity` model — GUID, name, stats snapshot, `Result` combat record
- `BloodVaultData` — list of `Identity` objects, JSON-serialized
- `ArmouryData` — list of `ItemConfig` objects, JSON-serialized
