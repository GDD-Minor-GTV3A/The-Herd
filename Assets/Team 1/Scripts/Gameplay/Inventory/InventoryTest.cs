using UnityEngine;
using System.Collections.Generic;

public class InventoryTest : MonoBehaviour
{
    private PlayerInventory inv;
    private InventoryItem[] testItems;

    private void Start()
    {
        inv = PlayerInventory.Instance;
        inv.OnInventoryChanged += OnInventoryChanged;
        inv.OnEquipmentChanged += OnEquipmentChanged;

        Debug.Log("Inventory test started.");

        // Load persistent test items from Resources
        testItems = Resources.LoadAll<InventoryItem>("InventoryTestItems");
        Debug.Log($"Loaded {testItems.Length} persistent test items for later adding.");
    }

    private void OnInventoryChanged() => Debug.Log("Inventory Changed!");
    private void OnEquipmentChanged() => Debug.Log("Equipment Changed!");

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            PrintInventory();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (inv.data.items.Count > 0)
            {
                inv.Equip(inv.data.items[0]);
                Debug.Log($"Equipped: {inv.data.items[0].itemName}");
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            var active = inv.data.items.Find(i => i.isActiveItem);
            if (active != null)
            {
                inv.UseActiveItem(active);
                Debug.Log($"Used: {active.itemName}, remaining uses: {active.activeUses}");
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (inv.SpendScroll(1)) Debug.Log("Used 1 scroll");
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (inv.data.reviveTotems > 0)
            {
                inv.data.reviveTotems--;
                inv.RaiseInventoryChanged();
                Debug.Log("Used 1 revive totem");
            }
        }

        // --- Add persistent test items to inventory when pressing "S" ---
        if (Input.GetKeyDown(KeyCode.S))
        {
            AddPersistentTestItems();
        }
    }

    private void AddPersistentTestItems()
    {
        if (testItems == null || testItems.Length == 0)
        {
            Debug.LogWarning("No persistent test items loaded.");
            return;
        }

        foreach (var item in testItems)
        {
            if (!inv.data.items.Contains(item))
            {
                // Active items: ensure positive activeUses
                if (item.isActiveItem && item.activeUses <= 0)
                {
                    item.activeUses = Random.Range(1, 5); // or a default positive value
                }

                // Non-active items: set to -1
                if (!item.isActiveItem)
                {
                    item.activeUses = -1;
                }

                inv.AddItem(item);
                Debug.Log($"Added persistent item: {item.itemName} | Active: {item.isActiveItem} | ActiveUses: {item.activeUses} | GUID: {item.GUID}");
            }
        }

        inv.RaiseInventoryChanged();
        Debug.Log("Persistent test items added to inventory.");
    }

    private void PrintInventory()
    {
        Debug.Log("=== Inventory ===");
        foreach (var item in inv.data.items)
        {
            int uses = item.isActiveItem ? item.activeUses : -1; // Ensure display matches save convention
            Debug.Log($"Item: {item.itemName} | Category: {item.category} | Active: {item.isActiveItem} | ActiveUses: {uses} | GUID: {item.GUID}");
        }

        Debug.Log(inv.GetEquippedSummary());
        Debug.Log($"Scrolls: {inv.data.scrolls} | Revive Totems: {inv.data.reviveTotems}");
    }
}
