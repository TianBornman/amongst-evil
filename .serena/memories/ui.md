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

## Custom UI Elements (extend VisualElement)
- `ItemElement` — renders an equippable item
- `AbilityElement` — renders an ability + cooldown
- `EffectElement` — renders an active status effect

## Patterns
- `UiManager.Instance` toggles visibility of UIDocuments
- `Query<T>()` used to find elements by type within a document
- Effects/abilities update their own elements via event callbacks
- `AbilitySlot` uses GUID to rebind after scene reload

## Settings (SettingsUI)
- Music volume persisted via `AudioManager` → `PlayerPrefs`
- Show/hide via `UiManager`
