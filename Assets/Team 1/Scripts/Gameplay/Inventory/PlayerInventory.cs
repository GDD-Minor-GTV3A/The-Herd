using System;
using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    [Header("Runtime Data")]
    public InventoryData data = new InventoryData();

    [Header("Settings")]
    public int maxTrinkets = 3;

    public event Action OnInventoryChanged;
    public event Action OnEquipmentChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InventorySaveManager.Load(this);
        OnInventoryChanged?.Invoke();
        OnEquipmentChanged?.Invoke();
    }

    private void OnApplicationQuit()
    {
        InventorySaveManager.Save(this);
    }

    // =======================
    // CURRENCY
    // =======================
    public void AddScroll(int amount) { data.scrolls += amount; OnInventoryChanged?.Invoke(); }
    public bool SpendScroll(int amount)
    {
        if (data.scrolls < amount) return false;
        data.scrolls -= amount;
        OnInventoryChanged?.Invoke();
        return true;
    }
    public void AddReviveTotem(int amount) { data.reviveTotems += amount; OnInventoryChanged?.Invoke(); }

    // =======================
    // ITEM MANAGEMENT
    // =======================
    public void AddItem(InventoryItem item)
    {
        if (item == null) return;

        switch (item.category)
        {
            case ItemCategory.Active:
                var existing = data.items.Find(s => s.item == item);
                if (existing != null)
                    existing.uses++;
                else
                    data.items.Add(new InventoryStack(item, 1));
                break;

            case ItemCategory.Scroll:
                AddScroll(1);
                break;

            case ItemCategory.ReviveTotem:
                AddReviveTotem(1);
                break;

            default: // wearable / trinket / etc
                data.items.Add(new InventoryStack(item, 1));
                break;
        }

        OnInventoryChanged?.Invoke();
    }

    public void RemoveItem(InventoryItem item)
    {
        if (item == null) return;

        switch (item.category)
        {
            case ItemCategory.Active:
                var stack = data.items.Find(s => s.item == item);
                if (stack == null) return;
                stack.uses--;
                if (stack.uses <= 0)
                    data.items.Remove(stack);
                break;

            case ItemCategory.Scroll:
                if (data.scrolls > 0) data.scrolls--;
                break;

            case ItemCategory.ReviveTotem:
                if (data.reviveTotems > 0) data.reviveTotems--;
                break;

            default: // wearable / trinket
                var wstack = data.items.Find(s => s.item == item);
                if (wstack != null) data.items.Remove(wstack);
                break;
        }

        OnInventoryChanged?.Invoke();
    }

    public void UseActiveItem(InventoryItem item)
    {
        if (item == null || item.category != ItemCategory.Active) return;

        Debug.Log($"Used: {item.itemName}");
        RemoveItem(item);
    }

    // =======================
    // EQUIP / UNEQUIP
    // =======================
    public void Equip(InventoryItem item)
    {
        if (item == null) return;

        switch (item.category)
        {
            case ItemCategory.Headgear:
                if (data.headgear != null) AddItem(data.headgear);
                data.headgear = item;
                break;

            case ItemCategory.Chestwear:
                if (data.chestwear != null) AddItem(data.chestwear);
                data.chestwear = item;
                break;

            case ItemCategory.Legwear:
                if (data.legwear != null) AddItem(data.legwear);
                data.legwear = item;
                break;

            case ItemCategory.Boots:
                if (data.boots != null) AddItem(data.boots);
                data.boots = item;
                break;

            case ItemCategory.Trinket:
                if (data.trinkets.Count < maxTrinkets)
                {
                    data.trinkets.Add(item);
                }
                else
                {
                    // Randomly choose a slot to replace
                    int randomIndex = UnityEngine.Random.Range(0, data.trinkets.Count);
                    var replaced = data.trinkets[randomIndex];

                    // Return the replaced trinket to inventory
                    AddItem(replaced);

                    // Replace with new trinket
                    data.trinkets[randomIndex] = item;
                }
                break;

            default:
                Debug.LogWarning("Equip() called on non-equip item.");
                return;
        }

        RemoveItem(item);
        OnEquipmentChanged?.Invoke();
        OnInventoryChanged?.Invoke();
    }

    public void Unequip(InventoryItem item)
    {
        if (item == null) return;

        if (data.headgear == item) data.headgear = null;
        if (data.chestwear == item) data.chestwear = null;
        if (data.legwear == item) data.legwear = null;
        if (data.boots == item) data.boots = null;
        if (data.trinkets.Contains(item)) data.trinkets.Remove(item);

        AddItem(item);
        OnEquipmentChanged?.Invoke();
        OnInventoryChanged?.Invoke();
    }

    public int GetUses(InventoryItem item)
    {
        if (item == null) return 0;

        InventoryStack stack = data.items.Find(s => s.item == item);
        return stack != null ? stack.uses : 0;
    }

    // Helper
    public string GetEquippedSummary()
    {
        return $"Head:{data.headgear?.itemName} Chest:{data.chestwear?.itemName} Legs:{data.legwear?.itemName} Boots:{data.boots?.itemName} Trinkets:{data.trinkets.Count}";
    }

    public void RaiseInventoryChanged() => OnInventoryChanged?.Invoke();
    public void RaiseEquipmentChanged() => OnEquipmentChanged?.Invoke();
}
