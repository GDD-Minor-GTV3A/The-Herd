using System;
using UnityEngine;
using System.Collections.Generic;

namespace Gameplay.Inventory
{
    public class PlayerInventory : MonoBehaviour
    {
        public static PlayerInventory Instance { get; private set; }

        [Header("Inventory Data")]
        public InventoryData data = new();
        public int maxTrinkets = 3;

        public event Action OnInventoryChanged;
        public event Action OnEquipmentChanged;

        //──────────────────────────────────────────────────────
        private void Awake()
        {
            if (Instance && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            InventorySaveManager.Load(this);
            RaiseInventoryChanged();
            RaiseEquipmentChanged();
        }

        private void OnApplicationQuit() => InventorySaveManager.Save(this);

        //======================================================================
        // 🔥  ONE METHOD TO ADD ITEMS
        //======================================================================
        public void AddItem(InventoryItem item, int quantity = 1)
        {
            if (!item || quantity <= 0) return;

            // Equipment uniqueness check
            if (item.category == ItemCategory.Headgear && data.headgear == item) return;
            if (item.category == ItemCategory.Chestwear && data.chestwear == item) return;
            if (item.category == ItemCategory.Legwear && data.legwear == item) return;
            if (item.category == ItemCategory.Boots && data.boots == item) return;
            if (item.category == ItemCategory.Trinket && data.trinkets.Contains(item)) return;

            switch (item.category)
            {
                case ItemCategory.Scroll: AddScrolls(quantity); return;
                case ItemCategory.ReviveTotem: AddReviveTotems(quantity); return;
            }

            InventoryStack stack = data.items.Find(s => s.item == item);

            if (stack != null) stack.uses += quantity;
            else data.items.Add(new InventoryStack(item, quantity));

            RaiseInventoryChanged();
        }

        //======================================================================
        // 🔥  ONE METHOD TO USE ITEMS (except scrolls + revive totems)
        //======================================================================
        public bool UseItem(InventoryItem item, int slotIndex = 0)
        {
            if (!item) return false;

            switch (item.category)
            {
                case ItemCategory.Active: return ConsumeItem(item);

                case ItemCategory.Headgear or ItemCategory.Chestwear or
                ItemCategory.Legwear or ItemCategory.Boots or
                ItemCategory.Trinket:
                    return EquipItem(item, slotIndex);

                default: return false;
            }
            ;
        }

        //======================================================================
        // ACTIVE ITEM USE
        //======================================================================
        private bool ConsumeItem(InventoryItem item)
        {
            InventoryStack stack = data.items.Find(s => s.item == item);
            if (stack == null) return false;

            stack.uses--;

            if (stack.uses <= 0) data.items.Remove(stack);

            RaiseInventoryChanged();
            return true;
        }

        //======================================================================
        // EQUIPPABLE HANDLING (HEAD/CHEST/LEGS/BOOTS/TRINKET)
        //======================================================================
        private bool EquipItem(InventoryItem item, int trinketSlot = 0)
        {
            switch (item.category)
            {
                case ItemCategory.Headgear: SwapItem(ref data.headgear, item); break;
                case ItemCategory.Chestwear: SwapItem(ref data.chestwear, item); break;
                case ItemCategory.Legwear: SwapItem(ref data.legwear, item); break;
                case ItemCategory.Boots: SwapItem(ref data.boots, item); break;

                case ItemCategory.Trinket:
                    if (data.trinkets.Count < maxTrinkets)
                    {
                        data.trinkets.Add(item);
                    }
                    else
                    {
                        InventoryItem oldTrinket = data.trinkets[trinketSlot]; // store old trinket
                        data.trinkets[trinketSlot] = null;                     // clear slot
                        if (oldTrinket != null) AddItem(oldTrinket);           // put old trinket back in inventory
                        data.trinkets[trinketSlot] = item;                     // equip new trinket
                    }
                    break;
            }

            RemoveFromInventory(item);
            RaiseEquipmentChanged();
            RaiseInventoryChanged();
            return true;
        }

        private void SwapItem(ref InventoryItem slot, InventoryItem item)
        {
            InventoryItem old = slot;  // store old equipped item
            slot = null;               // clear slot so AddItem uniqueness check passes
            if (old != null) AddItem(old);
            slot = item;
        }

        //======================================================================
        // REMOVE Helper
        //======================================================================
        private void RemoveFromInventory(InventoryItem item)
        {
            InventoryStack s = data.items.Find(x => x.item == item);
            if (s != null)
            {
                s.uses--;
                if (s.uses <= 0) data.items.Remove(s);
                RaiseInventoryChanged();
            }
        }

        //======================================================================
        // SCROLLS + REVIVE TOTEMS (separate by request)
        //======================================================================

        public bool UseScrolls(int amt, InventoryItem item = null) => amt > 0 && data.scrolls >= amt && (data.scrolls -= amt) >= 0 && RaiseInv();

        public bool UseReviveTotems(int amt) => amt > 0 && data.reviveTotems >= amt && (data.reviveTotems -= amt) >= 0 && RaiseInv();

        private void AddScrolls(int amt)
        {
            data.scrolls += amt;
            RaiseInventoryChanged();
        }

        private void AddReviveTotems(int amt)
        {
            data.reviveTotems += amt;
            RaiseInventoryChanged();
        }

        //======================================================================
        bool RaiseInv() { RaiseInventoryChanged(); return true; }
        public void RaiseInventoryChanged() => OnInventoryChanged?.Invoke();
        public void RaiseEquipmentChanged() => OnEquipmentChanged?.Invoke();

        public int GetUses(InventoryItem item) => data.items.Find(s => s.item == item)?.uses ?? 0;
        public string GetEquippedSummary() => $"Head:{data.headgear?.itemName} Chest:{data.chestwear?.itemName} Legs:{data.legwear?.itemName} Boots:{data.boots?.itemName} Trinkets:{data.trinkets.Count}";
    }
}