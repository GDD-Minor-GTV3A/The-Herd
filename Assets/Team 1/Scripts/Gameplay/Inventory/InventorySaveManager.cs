using System.IO;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public static class InventorySaveManager
{
    private static readonly string FileName = "inventory.json";
    private static string SavePath => Path.Combine(Application.persistentDataPath, FileName);

    // cache for ScriptableObject lookup
    private static Dictionary<string, InventoryItem> lookup;

    private static void BuildLookup()
    {
        if (lookup != null) return;

        var all = Resources.LoadAll<InventoryItem>(""); // assets must be in Resources
        lookup = all.ToDictionary(i => i.GUID, i => i);
    }

    public static void Save(PlayerInventory inv)
    {
        if (inv == null) return;
        BuildLookup();

        InventorySaveData data = new InventorySaveData();

        // Save inventory stacks
        foreach (var stack in inv.data.items)
        {
            data.inventoryItemIDs.Add(stack.item.GUID);
            data.activeItemUses.Add(stack.item.category == ItemCategory.Active ? stack.uses : -1);
        }

        // Equipped
        data.headID = inv.data.headgear?.GUID ?? "";
        data.chestID = inv.data.chestwear?.GUID ?? "";
        data.legsID = inv.data.legwear?.GUID ?? "";
        data.bootsID = inv.data.boots?.GUID ?? "";

        // Trinkets
        foreach (var t in inv.data.trinkets)
            data.trinketIDs.Add(t.GUID);

        // Currency
        data.scrolls = inv.data.scrolls;
        data.reviveTotems = inv.data.reviveTotems;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"Inventory saved to {SavePath}");
    }

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

        // Equipped
        inv.data.headgear = Find(data.headID);
        inv.data.chestwear = Find(data.chestID);
        inv.data.legwear = Find(data.legsID);
        inv.data.boots = Find(data.bootsID);

        // Trinkets
        foreach (var tid in data.trinketIDs)
        {
            var tr = Find(tid);
            if (tr != null) inv.data.trinkets.Add(tr);
        }

        // Currency
        inv.data.scrolls = data.scrolls;
        inv.data.reviveTotems = data.reviveTotems;

        inv.RaiseInventoryChanged();
        inv.RaiseEquipmentChanged();

        Debug.Log("Inventory loaded.");
    }

    private static InventoryItem Find(string guid)
    {
        if (string.IsNullOrEmpty(guid)) return null;
        return lookup.TryGetValue(guid, out var item) ? item : null;
    }
}
