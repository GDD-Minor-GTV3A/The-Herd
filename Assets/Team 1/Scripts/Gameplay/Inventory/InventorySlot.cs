using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Represents a drop target slot for inventory or equipment items.
/// Enforces category rules and swaps items if equipment slot already has an item.
/// </summary>
public class InventorySlot : MonoBehaviour, IDropHandler
{
    [Header("Slot Configuration")]
    public bool isEquipmentSlot = false;
    public bool isTrinketSlot = false;
    public ItemCategory slotCategory;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        InventoryItemSlot draggedItem = eventData.pointerDrag.GetComponent<InventoryItemSlot>();
        if (draggedItem == null || draggedItem.item == null) return;

        // Check category rules
        if (isEquipmentSlot && draggedItem.item.category != slotCategory)
        {
            Debug.Log("Wrong category for this equipment slot!");
            return;
        }

        if (isTrinketSlot && draggedItem.item.category != ItemCategory.Trinket)
        {
            Debug.Log("Only trinkets can be placed in trinket slots!");
            return;
        }

        // Target slot visual
        InventoryItemSlot slotItem = GetComponentInChildren<InventoryItemSlot>();
        if (slotItem == null)
        {
            Debug.LogWarning("InventorySlot has no InventoryItemSlot child!");
            return;
        }

        // SWAP logic for equipment/trinkets
        if (isEquipmentSlot || isTrinketSlot)
        {
            InventoryItem oldItem = slotItem.item;  // the item currently in the slot
            Transform oldParent = draggedItem.originalParent; // where dragged item came from

            // Move dragged item into target slot
            slotItem.InitializeItem(draggedItem.item);
            draggedItem.image.transform.SetParent(slotItem.transform, false);
            draggedItem.image.transform.localPosition = Vector3.zero;

            // Equip the new item
            PlayerInventory.Instance.Equip(slotItem.item);

            // Put old item back into original slot
            if (oldItem != null)
            {
                draggedItem.InitializeItem(oldItem);
                draggedItem.image.transform.SetParent(oldParent, false);
                draggedItem.image.transform.localPosition = Vector3.zero;
            }
            else
            {
                // Clear dragged slot if there was no previous item
                draggedItem.InitializeItem(null);
            }
        }
        else
        {
            // Regular inventory slots
            if (slotItem.item != null)
            {
                Debug.Log("Slot already has an item!");
                return;
            }

            slotItem.InitializeItem(draggedItem.item);
            draggedItem.image.transform.SetParent(slotItem.transform, false);
            draggedItem.image.transform.localPosition = Vector3.zero;
            draggedItem.InitializeItem(null);
        }
    }
}
