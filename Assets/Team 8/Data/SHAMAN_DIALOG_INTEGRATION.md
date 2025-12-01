# Shaman Dialog Integration Guide

## For Scroll Exchange (Scrolls -> Items)

```csharp
using Gameplay.Inventory;

// check if player has enough scroll(s)
int scrollCount = PlayerInventory.Instance.data.scrolls;
if (scrollCount <= 0) return; // or whatever u want to do

// remove scrolls from inventory
PlayerInventory.Instance.RemoveFromInventory(scrollScriptableObject, amount);

// give items to player
PlayerInventory.Instance.AddItem(itemScriptableObject, amount);
```

**note:** Scroll ScriptableObject must have `category = ItemCategory.Scroll`

---

## For Totem Exchange (Totem -> Revive Sheep)

```csharp
using Gameplay.Inventory;
using Core.AI.Sheep;
using Core.AI.Sheep.Event;
using Core.Events;
using UnityEngine;

// check if player has totems
int totemCount = PlayerInventory.Instance.data.reviveTotems;
if (totemCount <= 0) return; // No totems

// get all dead sheep types
HashSet<SheepArchetype> deadSheepTypes = new HashSet<SheepArchetype>();
foreach (var sheep in SheepStateManager.AllSheep)
{
    var health = sheep.GetComponent<SheepHealth>();
    if (health != null && health.IsDead && sheep.Archetype != null)
    {
        deadSheepTypes.Add(sheep.Archetype);
    }
}

if (deadSheepTypes.Count == 0) return; // no dead sheep to revive

// pick a random dead sheep type
SheepArchetype typeToRevive = deadSheepTypes.ElementAt(Random.Range(0, deadSheepTypes.Count));

// use totem
PlayerInventory.Instance.UseReviveTotems(1);

// spawn new sheep (find prefab for that archetype and instantiate it)
// you need to load the correct prefab based on typeToRevive.PersonalityType
// for example: load from resources or reference a prefab mapping
GameObject sheepPrefab = /* load prefab for typeToRevive */;
Vector3 spawnPos = /* calculate spawn position near player */;
GameObject newSheepObj = Instantiate(sheepPrefab, spawnPos, Quaternion.identity);

SheepStateManager newSheep = newSheepObj.GetComponent<SheepStateManager>();
if (newSheep != null)
{
    newSheep.MarkAsStraggler(); // makes it join the herd
    EventManager.Broadcast(new SheepJoinEvent(newSheep));
}
```

**very important requirements:**

- Need mapping of `PersonalityType` → Sheep Prefab (ask team2? idk how we can do this)
- Spawn position should be near player (random offset)
- Must call `MarkAsStraggler()` and broadcast `SheepJoinEvent`
