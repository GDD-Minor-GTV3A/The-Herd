using CustomEditor.Attributes;

using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Represents a drop target slot for inventory or equipment items.
/// Enforces category rules and swaps items if the slot already has an item.
/// Refreshes InventoryUI after placement or swap.
/// </summary>
public class InventorySlot : MonoBehaviour, IDropHandler
{
    [Header("Slot Configuration")]
    public bool isEquipmentSlot = false;
    public bool isTrinketSlot = false;
    [ShowIf("isTrinketSlot")]
    public int slotNumber = 0;
    public ItemCategory slotCategory; // Only relevant for equipment slots

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        InventoryItemSlot draggedItem = eventData.pointerDrag.GetComponent<InventoryItemSlot>();
        if (draggedItem == null || draggedItem.item == null) return;

        // Prevent dropping non-matching items into equipment/trinket slots
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

        // Get the visual InventoryItemSlot child for this slot
        InventoryItemSlot slotItem = GetComponentInChildren<InventoryItemSlot>();
        if (slotItem == null)
        {
            Debug.LogWarning("InventorySlot has no InventoryItemSlot child!");
            return;
        }

        bool successfulPlacement = false;

        // Equipment / Trinket slot logic (swap if needed)
        if (isEquipmentSlot || isTrinketSlot)
        {
            InventoryItem oldItem = slotItem.item;           // current item in slot
            Transform oldParent = draggedItem.originalParent; // where dragged item came from

            // Place new item in the target slot
            slotItem.InitializeItem(draggedItem.item);
            draggedItem.image.transform.SetParent(slotItem.transform, false);
            draggedItem.image.transform.localPosition = Vector3.zero;

            // Equip the new item
            PlayerInventory.Instance.UseItem(slotItem.item, slotNumber);

            // If there was an old item, return it to the original slot
            if (oldItem != null)
            {
                draggedItem.InitializeItem(oldItem);
                draggedItem.image.transform.SetParent(oldParent, false);
                draggedItem.image.transform.localPosition = Vector3.zero;
            }
            else
            {
                // Clear dragged slot if nothing was there
                draggedItem.InitializeItem(null);
            }

            successfulPlacement = true;
        }
        else
        {
            // Regular inventory slot logic (no swap)
            if (slotItem.item != null)
            {
                Debug.Log("Slot already has an item!");
                return;
            }

            slotItem.InitializeItem(draggedItem.item);
            draggedItem.image.transform.SetParent(slotItem.transform, false);
            draggedItem.image.transform.localPosition = Vector3.zero;
            draggedItem.InitializeItem(null);

            successfulPlacement = true;
        }

        // Refresh inventory UI once if placement/swap succeeded
        if (successfulPlacement && InventoryUI.Instance != null)
        {
            InventoryUI.Instance.RefreshInventoryGrid();
        }
    }
}