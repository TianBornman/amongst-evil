# Singleton Managers

All managers live in `Assets/Scripts/Managers/` and extend `Singleton<T>`.
Access pattern: `ManagerName.Instance.Method()`

| Manager | File | Responsibility |
|---------|------|----------------|
| `GameManager` | GameManager.cs | Game state: menu / hub / pause |
| `PartyManager` | PartyManager.cs | Party recruitment + run lifecycle |
| `SpawnManager` | SpawnManager.cs | Enemy wave spawning + boss logic |
| `InventoryManager` | InventoryManager.cs | Item armoury (armoury.json) |
| `InputManager` | InputManager.cs | Input action → `public Action` delegates |
| `UiManager` | UiManager.cs | UIDocument management + binding |
| `AudioManager` | AudioManager.cs | Music volume + PlayerPrefs |
| `LevelUpManager` | LevelUpManager.cs | **Deprecated stub.** Brother level-ups are passive in `Character.LevelUp` (stat scale + heal). The card UI is now the boon picker. |
| `RunBoonManager` | Boons/RunBoonManager.cs | In-run boon pool + offer/pick lifecycle. Cleared on `EndRun`. Hooked from mission runners' beats and from `PartyManager.StartRun/EndRun`. |
| `RecruitManager` | RecruitManager.cs | NPC spawning in hub |
| `BloodvaultManager` | BloodvaultManager.cs | Persistent dead-char storage (bloodvault.json) |
| `DamageNumberManager` | DamageNumberManager.cs | Floating damage text pool |
| `RefManager` | RefManager.cs | Centralized asset refs (items, effects, abilities, icons) |
| `InteractionManager` | InteractionManager.cs | Hover / click on `IInteractable` objects |
| `HubManager` | HubManager.cs | Hub scene management; `StartRun(Mission)` sets mission + loads Level |
| `HubUiManager` | HubUiManager.cs | Hub UI state; owns Mission Board (`ShowMissionBoardUI` / `HideMissionBoardUI` / `MissionBoardOpen`) and Spiral UI (`ShowSpiralUI` / `HideSpiralUI` / `SpiralOpen`) |
| `SectProgressManager` | Progression/SectProgressManager.cs | Sect-wide progression on the Spiral of the Veil — Standing accumulator, requirement evaluation, ascension ceremony. Persists via `BloodvaultManager` (same `bloodvault.json`). |

## InputManager Action Delegates
Subscribe to these for input events (no polling):
- `MenuToggleAction`
- `AbilityAction`
- `SelectionAction`
- `EscapeAction`
- `CameraToggleAction`
- `SetPartyTargetAction`
- `MapViewAction`
