using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Utility MonoBehaviour for testing PlayerInventory functionality in-editor or during runtime.
/// Provides shortcuts for equipping items, using active items, spending currency, and adding test items.
/// </summary>
public class InventoryTest : MonoBehaviour
{
    private PlayerInventory inv;          // Reference to the singleton PlayerInventory
    private InventoryItem[] testItems;    // Preloaded test items from Resources folder

    private void Start()
    {
        // Cache reference to the player inventory
        inv = PlayerInventory.Instance;

        // Subscribe to inventory and equipment change events for debugging/logging
        inv.OnInventoryChanged += OnInventoryChanged;
        inv.OnEquipmentChanged += OnEquipmentChanged;

        Debug.Log("Inventory test started.");

        // Load persistent test items from Resources/InventoryTestItems
        testItems = Resources.LoadAll<InventoryItem>("InventoryTestItems");
        Debug.Log($"Loaded {testItems.Length} persistent test items for later adding.");
    }

    #region Event Handlers

    /// <summary>
    /// Called whenever the inventory changes (items added/removed/swapped)
    /// </summary>
    private void OnInventoryChanged() => Debug.Log("Inventory Changed!");

    /// <summary>
    /// Called whenever equipment (wearables or trinkets) changes
    /// </summary>
    private void OnEquipmentChanged() => Debug.Log("Equipment Changed!");

    #endregion

    private void Update()
    {
        // Debug key bindings

        // Print current inventory state
        if (Input.GetKeyDown(KeyCode.I))
            PrintInventory();

        // Equip a random wearable (head, chest, legs, boots)
        if (Input.GetKeyDown(KeyCode.Alpha1))
            EquipRandomWearable();

        // Use a random active item
        if (Input.GetKeyDown(KeyCode.Alpha2))
            UseRandomActiveItem();

        // Spend one scroll
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (inv.UseScrolls(1)) Debug.Log("Used 1 scroll");
        }

        // Use one revive totem
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (inv.data.reviveTotems > 0)
            {
                inv.data.reviveTotems--;
                inv.RaiseInventoryChanged();
                Debug.Log("Used 1 revive totem");
            }
        }

        // Equip a random trinket
        if (Input.GetKeyDown(KeyCode.Alpha5))
            EquipRandomTrinket();

        // Add preloaded persistent test items
        if (Input.GetKeyDown(KeyCode.S))
            AddPersistentTestItems();
    }

    #region Item Actions

    /// <summary>
    /// Equips a random wearable item from the inventory (head, chest, legs, boots)
    /// </summary>
    private void EquipRandomWearable()
    {
        var equipableItems = inv.data.items.FindAll(s =>
            s.item.category == ItemCategory.Headgear ||
            s.item.category == ItemCategory.Chestwear ||
            s.item.category == ItemCategory.Legwear ||
            s.item.category == ItemCategory.Boots
        );

        if (equipableItems.Count > 0)
        {
            var randomIndex = Random.Range(0, equipableItems.Count);
            var toEquip = equipableItems[randomIndex].item;
            inv.UseItem(toEquip);
            Debug.Log($"Equipped random wearable: {toEquip.itemName}");
        }
        else
        {
            Debug.Log("No wearable items available to equip.");
        }
    }

    /// <summary>
    /// Equips a random trinket from the inventory
    /// </summary>
    private void EquipRandomTrinket()
    {
        var trinketItems = inv.data.items.FindAll(s => s.item.category == ItemCategory.Trinket);

        if (trinketItems.Count > 0)
        {
            var randomIndex = Random.Range(0, trinketItems.Count);
            var toEquip = trinketItems[randomIndex].item;
            inv.UseItem(toEquip);
            Debug.Log($"Equipped random trinket: {toEquip.itemName}");
        }
        else
        {
            Debug.Log("No trinkets available to equip.");
        }
    }

    /// <summary>
    /// Uses a random active item from the inventory, if one exists
    /// </summary>
    private void UseRandomActiveItem()
    {
        var activeStack = inv.data.items.Find(s => s.item.category == ItemCategory.Active);
        if (activeStack != null)
        {
            inv.UseItem(activeStack.item);
            Debug.Log($"Used: {activeStack.item.itemName}, remaining uses: {inv.GetUses(activeStack.item)}");
        }
    }

    /// <summary>
    /// Adds all preloaded test items to the inventory
    /// </summary>
    private void AddPersistentTestItems()
    {
        if (testItems == null || testItems.Length == 0)
        {
            Debug.LogWarning("No persistent test items loaded.");
            return;
        }

        foreach (var item in testItems)
        {
            bool alreadyExists = inv.data.items.Exists(s => s.item == item);

            // Allow multiple copies for Active items or special categories
            if (!alreadyExists || item.category == ItemCategory.Active ||
                item.category == ItemCategory.Scroll ||
                item.category == ItemCategory.ReviveTotem)
            {
                inv.AddItem(item);
                Debug.Log($"Added persistent item: {item.itemName} | Category: {item.category} | GUID: {item.GUID}");
            }
        }

        inv.RaiseInventoryChanged();
        Debug.Log("Persistent test items added to inventory.");
    }

    #endregion

    #region Debug Helpers

    /// <summary>
    /// Prints current inventory, equipped items, and currency to the console
    /// </summary>
    private void PrintInventory()
    {
        Debug.Log("=== Inventory ===");
        foreach (var stack in inv.data.items)
        {
            int uses = stack.item.category == ItemCategory.Active ? stack.uses : -1;
            Debug.Log($"Item: {stack.item.itemName} | Category: {stack.item.category} | Uses: {uses} | GUID: {stack.item.GUID}");
        }

        Debug.Log(inv.GetEquippedSummary());
        Debug.Log($"Scrolls: {inv.data.scrolls} | Revive Totems: {inv.data.reviveTotems}");
    }

    #endregion
}
