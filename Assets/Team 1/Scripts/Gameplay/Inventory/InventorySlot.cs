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
            if (isEquipmentSlot && draggedItem.Item.category != slotCategory)
            {
                return;
            }

            if (isTrinketSlot && draggedItem.Item.category != ItemCategory.Trinket)
            {
                return;
            }

            // Get the visual InventoryItemSlot child for this slot
            InventoryItemSlot slotItem = GetComponentInChildren<InventoryItemSlot>();
            if (slotItem == null)
            {
                return;
            }


            // Equipment / Trinket slot logic (swap if needed)
            if (isEquipmentSlot || isTrinketSlot)
            {
                InventoryItem oldItem = slotItem.Item;           // current Item in slot
                Transform oldParent = draggedItem.OriginalParent; // where dragged Item came from

                // Place new Item in the target slot
                slotItem.InitializeItem(draggedItem.Item);
                draggedItem.Image.transform.SetParent(slotItem.transform, false);
                draggedItem.Image.transform.localPosition = Vector3.zero;

                // Equip the new Item
                PlayerInventory.Instance.UseItem(slotItem.Item, slotNumber);

                // If there was an old Item, return it to the original slot
                if (oldItem != null)
                {
                    draggedItem.InitializeItem(oldItem);
                    draggedItem.Image.transform.SetParent(oldParent, false);
                    draggedItem.Image.transform.localPosition = Vector3.zero;
                }
                else
                {
                    // Clear dragged slot if nothing was there
                    draggedItem.InitializeItem(null);
                }
            }
            else
            {
                // Regular inventory slot logic (no swap)
                if (slotItem.Item != null)
                {
                    return;
                }

                slotItem.InitializeItem(draggedItem.Item);
                draggedItem.Image.transform.SetParent(slotItem.transform, false);
                draggedItem.Image.transform.localPosition = Vector3.zero;
                draggedItem.InitializeItem(null);
            }
        }
    }
}