using System.Collections.Generic;

using CustomEditor.Attributes;

using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Represents a drop target slot for inventory or equipment Items.
/// Enforces category rules and swaps Items if the slot already has an Item.
/// Refreshes InventoryUI after placement or swap.
/// </summary>
namespace Gameplay.Inventory
{
    public class InventorySlot : MonoBehaviour, IDropHandler
    {
        [Header("Slot Configuration")]
        [SerializeField] private bool isEquipmentSlot = false;
        public bool IsEquipmentSlot { get => isEquipmentSlot; set => isEquipmentSlot = value; }

        [SerializeField] private bool isTrinketSlot = false;
        public bool IsTrinketSlot { get => isTrinketSlot; set => isTrinketSlot = value; }

        [SerializeField]
        [ShowIf("isTrinketSlot")]
        private int slotNumber = 0;
        public int SlotNumber { get => slotNumber; set => slotNumber = value; }

        [SerializeField] private ItemCategory slotCategory; // Only relevant for equipment slots
        public ItemCategory SlotCategory { get => slotCategory; set => slotCategory = value; }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null) return;

            InventoryItemSlot draggedItem = eventData.pointerDrag.GetComponent<InventoryItemSlot>();
            if (draggedItem == null || draggedItem.Item == null) return;

            // Prevent dropping non-matching items into equipment/trinket slots
            if (isEquipmentSlot && draggedItem.Item.category != slotCategory) return;
            if (isTrinketSlot && draggedItem.Item.category != ItemCategory.Trinket) return;

            // Get the visual InventoryItemSlot child for this slot
            InventoryItemSlot slotItem = GetComponentInChildren<InventoryItemSlot>();
            if (slotItem == null) return;

            // === Swap / Equip via PlayerInventory ===
            if (isEquipmentSlot || isTrinketSlot)
            {
                InventoryItem dragged = draggedItem.Item;
                InventoryItem target = slotItem.Item;

                // Ask the inventory to equip the dragged item
                bool success = isTrinketSlot
                    ? PlayerInventory.Instance.UseItem(dragged, slotNumber)   // use slotNumber for trinkets
                    : PlayerInventory.Instance.UseItem(dragged);              // equip other categories

                if (success)
                {
                    // The inventory will automatically refresh via OnInventoryChanged / OnEquipmentChanged events
                    // Just update the dragged slot visually
                    draggedItem.InitializeItem(target); // old item goes back into dragged slot
                }
            }
            else
            {
                // Regular inventory slot (no swap)
                if (slotItem.Item != null) return;

                slotItem.InitializeItem(draggedItem.Item);
                draggedItem.InitializeItem(null);
            }
        }
    }
}