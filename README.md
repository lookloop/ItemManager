# Item Manager

**A code-driven multi-container inventory framework** — backpacks, warehouses, equipment slots, shops… the shared foundation for every grid-based item system.

Zero prefab dependency. Async Addressables loading. Multiple independent containers. Cross-container drag-and-drop.

[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Unity](https://img.shields.io/badge/Unity-2022.3%2B-black)](package.json)

---

## Architecture

```
Core.cs                     ← Empty‑space clicks → close detail panels & bring containers to front
├── Core_Fields.cs          ← Public settings (specs[], font, shadowColor, pressTime, …)
├── Core_Init.cs            ← Startup sequence (Awake + Start + expiry‑check coroutine)
├── Core_Addressables.cs    ← Async ItemTable loading + 30‑min cache with automatic expiry
├── Core_ContainerBuilder.cs← Container → Mask → Grid → Cell — full procedural build + handler wiring
├── Core_SetItem.cs         ← Write item data, refresh the view, or hide off‑page cells
├── Core_SetPage.cs         ← Page‑flip logic, last‑page height adjustment, slide animation
├── Core_Exchange.cs        ← Cross‑container item swap with bidirectional admission checks
├── Core_RectUtility.cs     ← Procedural RectTransform creation (two overloads)
├── Core_Tip.cs             ← On‑screen temporary toast messages
└── Core_Watchman.cs        ← Fire‑and‑forget exception guard (Launch)

Data Layer
├── Item.cs                 ← Runtime item data (readonly struct: Id / Type / Tier / Count / Data)
├── ItemTable.cs            ← ScriptableObject lookup table (icon / border / name / description)
├── Cell.cs                 ← Cell UI references (cell / item icon / edge border / count label)
├── Container.cs            ← Runtime container instance (Rect refs + items[] + cells[] + page state)
├── ContainerSpec.cs        ← Container blueprint (dimensions / cells per page / rows / sprites / filter)
└── SetItemBase.cs          ← Admission‑filter base class [SerializeReference]

Base Classes
├── TouchBase.cs            ← Interaction base (IPointerDown/Drag/UpHandler) — CellTouch / ContainerTouch / TurnPageTouch inherit it
└── DetailBase.cs           ← Abstract detail panel — override Fill() for custom item detail rendering

Touch Layer
├── CellTouch.cs            ← Cell interaction state machine (PointerDown → Drag → Up routing)
├── CellTouch_GridScroll.cs ← Short‑drag grid scrolling
├── CellTouch_LongPress.cs  ← Long‑press timer + item extraction + drag ghost + edge‑scroll + edge‑turn‑page
├── CellTouch_Detail.cs     ← Pure tap → show detail panel
├── ContainerTouch.cs       ← Full‑container drag to reposition on canvas
└── TurnPageTouch.cs        ← Previous / Next page button clicks

Detail Layer
└── DetailFiller.cs         ← Default DetailBase implementation — async ItemTable load + auto‑position

Filters
├── TypeRestrictFilter.cs   ← Restricts a container to specific Item Types (e.g. equipment slots)
└── TestFilter.cs           ← Demo filter (odd Ids only — verifies the pipeline works)

Testing
└── Test.cs                 ← Startup random item fill (debug only, 1‑in‑3 chance per slot)

Editor
├── ContainerSpecDrawer.cs  ← Custom foldable Inspector for ContainerSpec + [SerializeReference] type drop‑down
└── ItemDataEditorTools.cs  ← Project right‑click → Create → ItemTable
```

---

## Features

- **Zero prefabs** — Every piece of UI is built procedurally via `new GameObject`. Container → Mask → Grid → Cell, the full hierarchy.
- **Multi‑container** — One `Core` component, a `specs[]` array that drives any number of independent container panels.
- **Cross‑container exchange** — Long‑press drag items between different containers. Bidirectional admission filters on both sides.
- **Async Addressables** — `ItemTable` loads on demand with a 30‑minute cache and automatic expiry.
- **Long‑press + edge scrolling** — After extracting an item, hold near the top/bottom of the mask to auto‑scroll the grid, or near the left/right edges to flip pages.
- **Custom Inspector** — `ContainerSpec` has a collapsible parameter panel; new elements auto‑fill defaults. `[SerializeReference]` drop‑down for filter selection.
- **Two build modes** — Pure data‑driven (`Build`) or prefab instantiation (`BuildPrefab`), switched automatically based on whether `prefabRect` is set.
- **Detail interface** — Subclass `DetailBase`, override `Fill(Core, Container, int)`, and attach it to your `detailRect` prefab.
- **Admission filters** — `SetItemBase.CanExchange(incoming, outgoing)` is called bidirectionally during a swap. Perfect for restricted slots (equipment, consumables, etc.).

---

## Installation

### Unity Package Manager (Git URL)

```
https://github.com/lookloop/ItemManager.git
```

### Dependencies

| Package | Minimum Version |
|---|---|
| `com.unity.addressables` | 1.21.0 |
| `com.unity.textmeshpro` | 3.0.0 |
| `com.unity.ugui` | 1.0.0 |

---

## Quick Start

### Data‑driven mode (no prefabs)

1. Create an empty GameObject under your Canvas, add the `Core` component.
2. Assign a `Font` (TMP Font Asset).
3. Fill the `Specs` array with your container parameters:

```
specs[0]:
    Total Items        = 80        // total item capacity
    Every Page Cells   = 40        // cells visible per page
    Row                = 5         // cells per row
    Cell Width         = 10        // cell side length
    Mask Height        = 40        // visible area height
    Container Fill Up  = 8         // top padding
    Container Fill Down = 4        // bottom padding
```

4. Press Play → the full container UI is generated automatically and populated with random test items.

### Prefab mode

Drag a `Prefab Rect` into the `ContainerSpec`:
- The prefab is instantiated automatically.
- All child transforms tagged `"Cell"` are detected as the cell registry.
- The `items` array length equals the number of cells — single‑page mode (no pagination).

### Creating an Item Table

Right‑click in the Project window → `Create → ItemTable`, then fill in:
- `Id` — unique item identifier
- `Item Sprite` — item icon
- `Glow Sprite` — border / glow effect
- `Item Name` / `Item Description` — name and description

Mark the `ItemTable` as Addressable; use `Id.ToString()` as the Addressables key.

---

## Touch Interactions

| Action | Behavior |
|---|---|
| Tap a cell (no drag) | Show the detail panel (calls `DetailBase.Fill`) |
| Drag a cell (no long‑press) | Scroll the grid |
| Long‑press a cell (0.3 s) | Extract the item as a drag ghost; source cell is hidden |
| Drag over another cell | Hover shadow appears on the target cell |
| Hold near mask top/bottom edge | Auto‑scroll the grid |
| Hold near mask left/right edge | Auto‑flip pages (with cooldown) |
| Release after long‑press + drag | Swap source ↔ target items; UI refreshes automatically |
| Tap a page‑flip button | Previous / Next page |
| Tap the page‑number input | Type a page number to jump |
| Tap empty space | Close all open detail panels |

---

## API

### Core — Public Fields

| Field | Type | Description |
|---|---|---|
| `specs` | `ContainerSpec[]` | Container blueprints — one container per entry |
| `font` | `TMP_FontAsset` | Global TMP font |
| `fontSize` | `float` | Global font size (default 3.9) |
| `pressTime` | `float` | Long‑press threshold in seconds (default 0.3) |
| `scrollSpeed` | `float` | Edge‑scroll speed (default 60) |
| `flipDistance` | `float` | Edge‑trigger distance (default 3) |
| `flipCool` | `float` | Page‑flip cooldown in seconds (default 0.5) |
| `flipDuration` | `float` | Page‑flip animation duration (default 0.5) |
| `shadowColor` | `Color` | Drop‑target hover shadow color (default black, 90% alpha) |

### Core.SetItem — Writing Item Data

```csharp
// Create an item and refresh the view (only refreshes if on the current page)
core.SetItem(container, itemKey, id, type, tier, count, data);

// Or pass an Item struct directly
core.SetItem(container, itemKey, new Item(id, type, tier, count, data));
```

### Core.View / Core.NoView — View Refresh

```csharp
// Async‑load the ItemTable and refresh a single cell's icon / border / count
await core.View(container, itemKey);

// Hide a cell without touching the data
core.NoView(container, itemKey);
```

### Core.SetPage — Page Navigation

```csharp
// Jump to a specific page and refresh all visible cells
core.SetPage(container, page);
```

### Core.GetItemTable — Loading an Item Table

```csharp
// Async‑load an ItemTable (uses the in‑memory cache)
var table = await core.GetItemTable(itemId.ToString());
```

### SetItemBase — Admission Filters

```csharp
[Serializable]
public class SetItemBase
{
    // Pre‑swap admission check (called bidirectionally)
    public virtual bool CanExchange(Item incoming, Item outgoing) => true;

    // Callback invoked after SetItem writes data
    public virtual void OnItemSet(Container container, int itemKey) { }
}
```

Built‑in implementations: `TypeRestrictFilter` (restricts by `Type`), `OddIdOnlyFilter` (demo — odd Ids only).

### DetailBase — Detail Panel

```csharp
public abstract class DetailBase : MonoBehaviour
{
    public abstract Task Fill(Core core, Container container, int itemKey);
}
```

Attach a class that inherits from `DetailBase` to your `detailRect` prefab. It will be called automatically when the player taps an item. Default implementation: `DetailFiller`.

---

## Data Flow

```
Pointer down on a cell
  → OnPointerDown: start LongPressTimer coroutine (counts down pressTime)
    also capture the Grid's starting position for short‑drag scrolling
  → OnDrag:
      - Long‑press already triggered → DragItem (ghost follows finger + raycast target cell + shadow)
      - Long‑press not yet triggered → cancel timer → ScrollGrid (Grid follows finger)
  → LongPressTimer per‑frame check:
      - Finger near mask top/bottom → auto‑scroll Grid
      - Finger near mask left/right → auto‑flip page (with cooldown)
  → OnPointerUp:
      - Long‑press + drag → Exchange
          → calculate srcKey / tgtKey
          → read srcItem / tgtItem
          → bidirectional CanExchange check (src container filter + tgt container filter)
          → core.SetItem swaps the data
      - No long‑press & no drag → ShowDetail (calls DetailBase.Fill)
      - Unified Reset:
          → restore source cell visibility (if on the current page)
          → hide drag ghost and shadow
          → clear all session state
```

---

## License

MIT © 2026 Qin Jianpei
