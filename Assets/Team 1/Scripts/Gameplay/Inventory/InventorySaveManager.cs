using System.IO;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public static class InventorySaveManager
{
    private static readonly string FileName = "inventory.json";
    private static string SavePath => Path.Combine(Application.persistentDataPath, FileName);

    // cache
    private static Dictionary<string, InventoryItem> lookup;

    private static void BuildLookup()
    {
        if (lookup != null) return;
        // NOTE: Resources.LoadAll requires assets to be in a Resources folder.
        // If you don't want Resources, replace with Addressables or manual registration.
        var all = Resources.LoadAll<InventoryItem>("");
        lookup = all.ToDictionary(i => i.GUID, i => i);
    }

    public static void Save(PlayerInventory inv)
    {
        if (inv == null) return;
        BuildLookup();

        var data = new InventorySaveData();

        // inventory items
        foreach (var it in inv.data.items)
        {
            data.inventoryItemIDs.Add(it.GUID);
            data.activeItemUses.Add(it.isActiveItem ? it.activeUses : -1);
        }

        // equipped
        data.headID = inv.data.headgear?.GUID ?? "";
        data.chestID = inv.data.chestwear?.GUID ?? "";
        data.legsID = inv.data.legwear?.GUID ?? "";
        data.bootsID = inv.data.boots?.GUID ?? "";

        foreach (var t in inv.data.trinkets) data.trinketIDs.Add(t.GUID);

        data.scrolls = inv.data.scrolls;
        data.reviveTotems = inv.data.reviveTotems;

        var json = JsonUtility.ToJson(data, true);
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

        var json = File.ReadAllText(SavePath);
        var data = JsonUtility.FromJson<InventorySaveData>(json);

        inv.data.items.Clear();
        inv.data.trinkets.Clear();

        // restore inventory items (order preserved)
        for (int i = 0; i < data.inventoryItemIDs.Count; i++)
        {
            var id = data.inventoryItemIDs[i];
            if (lookup.TryGetValue(id, out var item))
            {
                // clone activeUses into the asset instance (note: ScriptableObject sharing caveat)
                inv.data.items.Add(item);
                if (item.isActiveItem && data.activeItemUses.Count > i && data.activeItemUses[i] >= 0)
                    item.activeUses = data.activeItemUses[i];
            }
        }

        // equipped
        inv.data.headgear = Find(data.headID);
        inv.data.chestwear = Find(data.chestID);
        inv.data.legwear = Find(data.legsID);
        inv.data.boots = Find(data.bootsID);

        foreach (var tid in data.trinketIDs)
        {
            var tr = Find(tid);
            if (tr != null) inv.data.trinkets.Add(tr);
        }

        inv.data.scrolls = data.scrolls;
        inv.data.reviveTotems = data.reviveTotems;

        inv.RaiseInventoryChanged();
        inv.RaiseEquipmentChanged();

        Debug.Log("Inventory loaded.");
    }

    private static InventoryItem Find(string guid)
    {
        if (string.IsNullOrEmpty(guid)) return null;
        return lookup.TryGetValue(guid, out var it) ? it : null;
    }
}
