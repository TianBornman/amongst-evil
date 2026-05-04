# UI System

## Technology
- **UI Toolkit (UIElements)** — no UGUI Canvas/Text anywhere
- Data binding via `SetBinding` / `SetValueWithoutNotify`
- UIDocuments are prefabs, managed by `UiManager`

## Key UI Documents (Prefabs/UI/)
- `GameUI` — in-run HUD (health bars, ability slots, etc.)
- `StatsUI` — character stats panel
- `LevelUpUI` — upgrade card selection (shown on level-up)
- `ItemPickUpUI` — item interaction / equip panel
- `ResultsUI` — end-of-run results screen
- `SettingsUI` — settings screen (music volume, etc.)
- `MissionBoardUI` — mission selection shown when interacting with the cart in the hub. Three `Card` children inside `#MissionList`, each with `#Title` / `#Type` / `#Threat` / `#Flavor` labels. Click → `HubManager.StartRun(mission)`. UXML: `Assets/UI/Mission Board UI.uxml`, USS: `Assets/UI/Style Sheets/Mission Board UI.uss`.

## Custom UI Elements (extend VisualElement)
- `ItemElement` — renders an equippable item
- `AbilityElement` — renders an ability + cooldown
- `EffectElement` — renders an active status effect
- `StatRowElement` — one row of the character stats sheet: label / base value / +gear delta / total. Uses `StatFormat` enum (Flat / Percent / PerSecond / Int) for value formatting. Delta is color-coded green/red/dim via `stat-row__delta--positive/--negative/--neutral` USS classes.

## Character Gear Panel (tabbed)
- `Assets/UI/Elements/Character Gear.uxml` is a TWO-TAB sheet shared by the recruit screen (HubUiManager) and the in-run StatsUI (UiManager). Tabs: **Gear** (slots + HP/XP bars) and **Stats** (per-stat base / gear / total breakdown).
- Tab wiring + stats population is centralized in `Midevil.UI.Elements.CharacterGearPanel` static helper:
  - `WireTabs(VisualElement root)` — wires the `GearTab` / `StatsTab` click handlers (call once per template instance, before any panel rename).
  - `PopulateStats(VisualElement root, Character character)` — pushes `character.baseScaledStats` and `character.stats` into all `StatRowElement`s in the stats panel.
  - `ResetToGearTab(VisualElement root)` — switches back to the Gear tab (used when the player re-opens the panel).
- HubUiManager calls `PopulateStats` from `ShowRecruitGearUI` (every time a recruit is shown or an item is equipped/unequipped — `RecruitCharacterEquipment` triggers `UpdateRecruitmentUI`).
- UiManager calls `PopulateStats` for every party member from `MenuToggle` whenever the StatsUI becomes visible.

## Patterns
- `UiManager.Instance` toggles visibility of UIDocuments
- `Query<T>()` used to find elements by type within a document
- Effects/abilities update their own elements via event callbacks
- `AbilitySlot` uses GUID to rebind after scene reload

## Settings (SettingsUI)
- Music volume persisted via `AudioManager` → `PlayerPrefs`
- Show/hide via `UiManager`
