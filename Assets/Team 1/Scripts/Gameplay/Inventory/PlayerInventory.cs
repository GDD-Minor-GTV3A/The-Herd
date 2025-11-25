using System;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// Singleton manager for the player's inventory.
/// Handles items, equipment, trinkets, and currency (scrolls & revive totems).
/// Supports adding/removing, equipping/unequipping, and persistent save/load.
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    // -------------------------
    // Singleton
    // -------------------------
    public static PlayerInventory Instance { get; private set; }

    [Header("Inventory Data")]
    public InventoryData data = new();       // Core inventory data
    public int maxTrinkets = 3;              // Maximum number of equipped trinkets

    // Events to notify UI or systems of changes
    public event Action OnInventoryChanged;
    public event Action OnEquipmentChanged;

    // -------------------------
    // Unity Lifecycle
    // -------------------------
    private void Awake()
    {
        // Ensure singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Load inventory from persistent storage on game start
        InventorySaveManager.Load(this);
        RaiseInventoryChanged();
        RaiseEquipmentChanged();
    }

    private void OnApplicationQuit()
    {
        // Save inventory before exiting the game
        InventorySaveManager.Save(this);
    }

    // -------------------------
    // Currency Management
    // -------------------------
    public void AddScroll(int amount) => ChangeScroll(amount);

    public bool SpendScroll(int amount)
    {
        if (data.scrolls < amount) return false; // Not enough scrolls
        ChangeScroll(-amount);
        return true;
    }

    public void AddReviveTotem(int amount) => ChangeReviveTotem(amount);

    private void ChangeScroll(int amount)
    {
        data.scrolls = Mathf.Max(0, data.scrolls + amount); // Prevent negative
        RaiseInventoryChanged();
    }

    private void ChangeReviveTotem(int amount)
    {
        data.reviveTotems = Mathf.Max(0, data.reviveTotems + amount); // Prevent negative
        RaiseInventoryChanged();
    }

    // -------------------------
    // Item Management
    // -------------------------
    /// <summary>
    /// Adds an item to inventory, handling stacks, currency, or equippable items.
    /// </summary>
    public void AddItem(InventoryItem item)
    {
        if (item == null) return;

        switch (item.category)
        {
            case ItemCategory.Active:
                var stack = data.items.Find(s => s.item == item);
                if (stack != null) stack.uses++;
                else data.items.Add(new InventoryStack(item, 1));
                break;
            case ItemCategory.Scroll: AddScroll(1); break;
            case ItemCategory.ReviveTotem: AddReviveTotem(1); break;
            default: data.items.Add(new InventoryStack(item, 1)); break;
        }

        RaiseInventoryChanged();
    }

    /// <summary>
    /// Removes an item from inventory, reducing stacks or currency as appropriate.
    /// </summary>
    public void RemoveItem(InventoryItem item)
    {
        if (item == null) return;

        switch (item.category)
        {
            case ItemCategory.Active:
                var stack = data.items.Find(s => s.item == item);
                if (stack == null) return;
                stack.uses--;
                if (stack.uses <= 0) data.items.Remove(stack);
                break;
            case ItemCategory.Scroll:
                if (data.scrolls > 0) data.scrolls--;
                break;
            case ItemCategory.ReviveTotem:
                if (data.reviveTotems > 0) data.reviveTotems--;
                break;
            default:
                var wstack = data.items.Find(s => s.item == item);
                if (wstack != null) data.items.Remove(wstack);
                break;
        }

        RaiseInventoryChanged();
    }

    /// <summary>
    /// Uses an active item, decreasing its stack count.
    /// </summary>
    public void UseActiveItem(InventoryItem item)
    {
        if (item?.category != ItemCategory.Active) return;
        Debug.Log($"Used {item.itemName}");
        RemoveItem(item);
    }

    // -------------------------
    // Equip/Unequip
    // -------------------------
    /// <summary>
    /// Equip an item to the appropriate slot or trinket list.
    /// Automatically removes it from inventory.
    /// </summary>
    public void Equip(InventoryItem item)
    {
        if (item == null) return;

        switch (item.category)
        {
            case ItemCategory.Headgear: Replace(ref data.headgear, item); break;
            case ItemCategory.Chestwear: Replace(ref data.chestwear, item); break;
            case ItemCategory.Legwear: Replace(ref data.legwear, item); break;
            case ItemCategory.Boots: Replace(ref data.boots, item); break;
            case ItemCategory.Trinket: EquipTrinket(item); break;
            default:
                Debug.LogWarning("Trying to equip non-equippable item");
                return;
        }

        RemoveItem(item);
        RaiseEquipmentChanged();
        RaiseInventoryChanged();
    }

    /// <summary>
    /// Helper to replace a wearable slot while returning the old item to inventory.
    /// </summary>
    private void Replace(ref InventoryItem slot, InventoryItem newItem)
    {
        if (slot != null) AddItem(slot);
        slot = newItem;
    }

    /// <summary>
    /// Equip a trinket; replaces a random one if maxTrinkets reached.
    /// </summary>
    private void EquipTrinket(InventoryItem item)
    {
        if (data.trinkets.Count < maxTrinkets) data.trinkets.Add(item);
        else
        {
            int idx = UnityEngine.Random.Range(0, data.trinkets.Count);
            AddItem(data.trinkets[idx]); // return old trinket to inventory
            data.trinkets[idx] = item;
        }
    }

    /// <summary>
    /// Unequip an item, returning it to inventory.
    /// </summary>
    public void Unequip(InventoryItem item)
    {
        if (item == null) return;

        if (data.headgear == item) data.headgear = null;
        else if (data.chestwear == item) data.chestwear = null;
        else if (data.legwear == item) data.legwear = null;
        else if (data.boots == item) data.boots = null;
        else if (data.trinkets.Contains(item)) data.trinkets.Remove(item);

        AddItem(item);
        RaiseEquipmentChanged();
        RaiseInventoryChanged();
    }

    // -------------------------
    // Helpers / Utilities
    // -------------------------
    /// <summary>Returns remaining uses of an active item.</summary>
    public int GetUses(InventoryItem item) => data.items.Find(s => s.item == item)?.uses ?? 0;

    /// <summary>Raise inventory change event.</summary>
    public void RaiseInventoryChanged() => OnInventoryChanged?.Invoke();

    /// <summary>Raise equipment change event.</summary>
    public void RaiseEquipmentChanged() => OnEquipmentChanged?.Invoke();

    /// <summary>Quick summary of currently equipped items and trinkets count.</summary>
    public string GetEquippedSummary() =>
        $"Head:{data.headgear?.itemName} Chest:{data.chestwear?.itemName} Legs:{data.legwear?.itemName} Boots:{data.boots?.itemName} Trinkets:{data.trinkets.Count}";
}
