using System.IO;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Handles saving and loading of player inventory to persistent storage.
/// Uses JSON serialization and ScriptableObject GUIDs for item reference.
/// </summary>
public static class InventorySaveManager
{
    private static readonly string FileName = "inventory.json"; // Name of the save file
    private static string SavePath => Path.Combine(Application.persistentDataPath, FileName);

    // Cache for quick lookup of InventoryItem ScriptableObjects by GUID
    private static Dictionary<string, InventoryItem> lookup;

    /// <summary>
    /// Builds a lookup dictionary for all InventoryItem ScriptableObjects in Resources.
    /// Only called once.
    /// </summary>
    private static void BuildLookup()
    {
        if (lookup != null) return;

        var all = Resources.LoadAll<InventoryItem>(""); // Make sure all items are in a Resources folder
        lookup = all.ToDictionary(i => i.GUID, i => i);
    }

    /// <summary>
    /// Saves the inventory state to a JSON file.
    /// </summary>
    /// <param name="inv">Player inventory instance</param>
    public static void Save(PlayerInventory inv)
    {
        if (inv == null) return;
        BuildLookup();

        InventorySaveData data = new InventorySaveData();

        // Save inventory items and active item uses
        foreach (var stack in inv.data.items)
        {
            data.inventoryItemIDs.Add(stack.item.GUID);
            data.activeItemUses.Add(stack.item.category == ItemCategory.Active ? stack.uses : -1);
        }

        // Save equipped items (head, chest, legs, boots)
        data.headID = inv.data.headgear?.GUID ?? "";
        data.chestID = inv.data.chestwear?.GUID ?? "";
        data.legsID = inv.data.legwear?.GUID ?? "";
        data.bootsID = inv.data.boots?.GUID ?? "";

        // Save trinkets
        foreach (var t in inv.data.trinkets)
            data.trinketIDs.Add(t.GUID);

        // Save currency
        data.scrolls = inv.data.scrolls;
        data.reviveTotems = inv.data.reviveTotems;

        // Serialize to JSON and write to file
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);

        Debug.Log($"Inventory saved to {SavePath}");
    }

    /// <summary>
    /// Loads the inventory state from a JSON file.
    /// </summary>
    /// <param name="inv">Player inventory instance</param>
    public static void Load(PlayerInventory inv)
    {
        if (inv == null) return;
        BuildLookup();

        if (!File.Exists(SavePath))
        {
            Debug.Log("No inventory save found.");
            return;
        }

        string json = File.ReadAllText(SavePath);
        InventorySaveData data = JsonUtility.FromJson<InventorySaveData>(json);

        inv.data.items.Clear();
        inv.data.trinkets.Clear();

        // Restore inventory stacks
        for (int i = 0; i < data.inventoryItemIDs.Count; i++)
        {
            string id = data.inventoryItemIDs[i];
            if (lookup.TryGetValue(id, out var item))
            {
                int uses = (data.activeItemUses.Count > i && data.activeItemUses[i] >= 0)
                    ? data.activeItemUses[i]
                    : 1;

                inv.data.items.Add(new InventoryStack(item, item.category == ItemCategory.Active ? uses : 1));
            }
        }

        // Restore equipped items
        inv.data.headgear = Find(data.headID);
        inv.data.chestwear = Find(data.chestID);
        inv.data.legwear = Find(data.legsID);
        inv.data.boots = Find(data.bootsID);

        // Restore trinkets
        foreach (var tid in data.trinketIDs)
        {
            var tr = Find(tid);
            if (tr != null) inv.data.trinkets.Add(tr);
        }

        // Restore currency
        inv.data.scrolls = data.scrolls;
        inv.data.reviveTotems = data.reviveTotems;

        // Notify UI and systems that inventory/equipment changed
        inv.RaiseInventoryChanged();
        inv.RaiseEquipmentChanged();

        Debug.Log("Inventory loaded.");
    }

    /// <summary>
    /// Helper method to lookup an InventoryItem by GUID.
    /// </summary>
    /// <param name="guid">Item GUID</param>
    /// <returns>InventoryItem if found, null otherwise</returns>
    private static InventoryItem Find(string guid)
    {
        if (string.IsNullOrEmpty(guid)) return null;
        return lookup.TryGetValue(guid, out var item) ? item : null;
    }
}
