using System;
using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    [Header("Runtime Data")]
    public InventoryData data = new InventoryData();

    [Header("Settings")]
    [Tooltip("Maximum simultaneous trinkets equipped.")]
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

    // Currency
    public void AddScroll(int amount) { data.scrolls += amount; OnInventoryChanged?.Invoke(); }
    public bool SpendScroll(int amount) { if (data.scrolls < amount) return false; data.scrolls -= amount; OnInventoryChanged?.Invoke(); return true; }
    public void AddReviveTotem(int amount) { data.reviveTotems += amount; OnInventoryChanged?.Invoke(); }

    // Inventory content
    public void AddItem(InventoryItem item)
    {
        if (item == null) return;
        data.items.Add(item);
        OnInventoryChanged?.Invoke();
    }

    public void RemoveItem(InventoryItem item)
    {
        if (item == null) return;
        data.items.Remove(item);
        OnInventoryChanged?.Invoke();
    }

    // Use active item (from inventory)
    public void UseActiveItem(InventoryItem item)
    {
        if (item == null || !item.isActiveItem) return;

        // apply effect placeholder (hook into actual effect system)
        Debug.Log($"Using active item: {item.itemName}");

        item.activeUses--;
        if (item.activeUses <= 0)
            RemoveItem(item);

        OnInventoryChanged?.Invoke();
    }

    // Equipwearable/trinket
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
                    // default behavior: replace oldest trinket
                    var replaced = data.trinkets[0];
                    AddItem(replaced);
                    data.trinkets.RemoveAt(0);
                    data.trinkets.Add(item);
                }
                break;
            default:
                Debug.LogWarning("Equip called for non-equip category.");
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
    }

    // Helper: find slot name / debug
    public string GetEquippedSummary()
    {
        return $"Head:{data.headgear?.itemName} Chest:{data.chestwear?.itemName} Legs:{data.legwear?.itemName} Boots:{data.boots?.itemName} Trinkets:{data.trinkets.Count}";
    }
    public void RaiseInventoryChanged()
    {
        OnInventoryChanged?.Invoke();
    }

    public void RaiseEquipmentChanged()
    {
        OnEquipmentChanged?.Invoke();
    }
}