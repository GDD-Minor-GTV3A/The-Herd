using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Represents a drop target slot for inventory or equipment items.
/// Enforces category rules and prevents overwriting existing items.
/// Works with InventoryItemSlot.
/// </summary>
public class InventorySlot : MonoBehaviour, IDropHandler
{
    [Header("Slot Configuration")]
    public bool isEquipmentSlot = false;    // true for head/chest/legs/boots
    public bool isTrinketSlot = false;      // true for trinket slots
    public ItemCategory slotCategory;       // Relevant only if isEquipmentSlot

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        InventoryItemSlot draggedItem = eventData.pointerDrag.GetComponent<InventoryItemSlot>();
        if (draggedItem == null || draggedItem.item == null) return;

        // Check if drop is valid
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

        if (!isEquipmentSlot && !isTrinketSlot)
        {
            // Regular inventory slots can accept anything
        }

        // Find the InventoryItemSlot child in this slot (the slot visual)
        InventoryItemSlot slotItem = GetComponentInChildren<InventoryItemSlot>();
        if (slotItem == null)
        {
            Debug.LogWarning("InventorySlot has no InventoryItemSlot child!");
            return;
        }

        // Prevent overwriting existing item
        if (slotItem.item != null)
        {
            Debug.Log("Slot already has an item!");
            return;
        }

        // Assign item to slot visually
        slotItem.InitializeItem(draggedItem.item);

        // Move the icon to the slot
        draggedItem.image.transform.SetParent(slotItem.transform, false);
        draggedItem.image.transform.localPosition = Vector3.zero;

        // Update dragged item's original parent
        draggedItem.originalParent = slotItem.transform;

        // Equip if relevant
        if (isEquipmentSlot || isTrinketSlot)
        {
            PlayerInventory.Instance.Equip(draggedItem.item);
        }
    }
}