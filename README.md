# Smart Store Organizer

A simple drag-and-drop simulation game where players organize store items into their correct zones (shelves or coolers).

---

[Download Smart Store Organizer (Android APK)](./Builds/SmartStoreOrganizer.apk)

---
## Scene Descriptions

| Scene | Description |
|--------|--------------|
| **ShelfScene** | Player organizes shelf-type items (e.g., cans, Jam, toilet rolls). |
| **CoolerScene** | Player organizes cooler-type items (e.g., yogurt, water, Apple). |


Each scene contains multiple drop zones that accept specific item categories.

---

## Drag & Drop System

- Tap or click on an item to **start dragging**.  
- While holding, move your finger to drag.  
- When you hover over a **valid drop zone**, the item will **snap** into place automatically.  
- Releasing the button/finger outside a valid zone will **cancel** the placement.  
- On mobile, only **one finger** drag is supported.

**How it works (technically):**
- A raycast from the camera detects draggable items (via `itemsLayer`).
- A `Plane` is created at the item’s depth for smooth world-space dragging.
- `ItemDragger` triggers events:  
  `OnDragStarted`, `OnZoneEntered`, `OnZoneExited`, `OnDragEnded`.

---

## Language Toggle Instructions

The game uses **Unity Localization** for multilingual support.

- Languages supported: **English**, **German**, **Spanish**.
- Language can be changed via the **dropdown** top of game scenes.
- Localization affects all UI text and item display names.

---

## Code Structure Overview

| Script | Purpose |
|---------|----------|
| **ItemDragger** | Handles item dragging logic, raycasts, and drop zone detection. |
| **ItemPlacer** | Handles item placement and registration in DropZones |
| **DropZone** | Represents a valid placement area for items. |
| **DraggableItem** | Implements `IDraggable`, holds reference to its `ItemDataSO`. |
| **ItemDataSO** | ScriptableObject defining item data (name, category, prefab, storage area). |
| **SODatabase** | Holds all available items and provides lookup by ID. |
| **SaveManager** | Saves and loads zone/item states via `PlayerPrefs`. |
| **LanguageDropdown** | Controls the language selection UI using Localization API. |
| **ConveyorBelt & ItemSpawner** | Handles item movement and spawning logic. |
| **SFXHandler** | Plays sound effects. |


---

## Known Issues

- Item spawn randomization is sometimes frustrating
- Performance may vary on low-end devices

---

### Author
Developed by **Ahmed Alfiki**  
