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
            EquipRandomWearable();

        if (Input.GetKeyDown(KeyCode.Alpha2))
            UseRandomActiveItem();

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

        if (Input.GetKeyDown(KeyCode.Alpha5))
            EquipRandomTrinket();

        if (Input.GetKeyDown(KeyCode.S))
            AddPersistentTestItems();
    }

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

            inv.Equip(toEquip);
            Debug.Log($"Equipped random wearable: {toEquip.itemName}");
        }
        else
        {
            Debug.Log("No wearable items available to equip.");
        }
    }

    private void EquipRandomTrinket()
    {
        var trinketItems = inv.data.items.FindAll(s => s.item.category == ItemCategory.Trinket);

        if (trinketItems.Count > 0)
        {
            var randomIndex = Random.Range(0, trinketItems.Count);
            var toEquip = trinketItems[randomIndex].item;

            inv.Equip(toEquip);
            Debug.Log($"Equipped random trinket: {toEquip.itemName}");
        }
        else
        {
            Debug.Log("No trinkets available to equip.");
        }
    }

    private void UseRandomActiveItem()
    {
        var activeStack = inv.data.items.Find(s => s.item.category == ItemCategory.Active);
        if (activeStack != null)
        {
            inv.UseActiveItem(activeStack.item);
            Debug.Log($"Used: {activeStack.item.itemName}, remaining uses: {inv.GetUses(activeStack.item)}");
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
            bool alreadyExists = inv.data.items.Exists(s => s.item == item);

            // Allow multiple copies for Active, Scroll, and ReviveTotem
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
}
