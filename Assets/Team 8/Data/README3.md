# Team 8 - Items & Collectables

## Items (ScriptableObjects in `Assets/Team 8/Data/Items/`)

### Passive (Equipment)
- **Old Boots** (Boots) - +15% walking speed → `bonusSpeed: 15`
- **Flashlight** (Headgear) - +20% sight range → `playerVisionRange: 20`
- **Fetching Bone** (Trinket) - +20% dog whistle speed (not yet impl., needs custom code)
- **Enchanted Collar** (Trinket) - Dog light cone (not yet impl., needs custom code)

### Active (Consumables)
- **Flare Gun** - Area lighting (not yet impl., needs custom code, max stack: 3)
- **Tranquilizing Mist** - Reduce sheep fear 15% (not yet impl., needs custom code, max stack: 3)

## Collectables (Scripts in `Assets/Team 8/Scripts/Collectables/`)

- **ScrollCollectable.cs** - Pickup scrolls, adds to inventory
- **TotemCollectable.cs** - Pickup totems, adds to inventory

## Usage

```csharp
using Gameplay.Inventory;

// Add/remove items
PlayerInventory.Instance.AddItem(item, amount);
PlayerInventory.Instance.RemoveFromInventory(item, amount);

// Use items
PlayerInventory.Instance.UseItem(item); // equip or consume
PlayerInventory.Instance.UseScrolls(amount); // use scrolls
PlayerInventory.Instance.UseReviveTotems(amount); // use totems

// Check inventory
bool hasItem = PlayerInventory.Instance.IsItemInInventory(item);
```

## Setup Collectables in Scene

1. Add GameObject with 3D model
2. Add `ScrollCollectable` or `TotemCollectable` component
3. Assign item ScriptableObject in inspector
4. Set collider to **Trigger**
5. Player needs **"Player"** tag

Test items available:
- `Assets/Resources/InventoryTestItems/Scroll_1.asset`
- `Assets/Resources/InventoryTestItems/ReviveTotem_5.asset`
