using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// UI representation of a single inventory item or equipment slot.
/// Handles display of item icons, placeholder visuals, click interactions, and drag-and-drop functionality.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class InventoryItemUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Item Data")]
    public InventoryItem item;          // The item represented by this UI slot
    public Image iconImage;             // Image component displaying the item's icon

    [Header("Slot Info (optional)")]
    public bool isEquipmentSlot = false; // True if this UI slot represents equipment/trinkets
    public ItemCategory slotCategory;    // Category of equipment this slot represents

    // Private references
    private CanvasGroup cg;
    private RectTransform rect;
    private Transform originalParent;   // Store original parent for drag revert
    private Canvas rootCanvas;          // Top-level canvas reference for dragging

    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        rect = GetComponent<RectTransform>();
        rootCanvas = GetComponentInParent<Canvas>();
    }

    /// <summary>
    /// Updates this slot's item and visual representation.
    /// </summary>
    /// <param name="newItem">The new item to display</param>
    /// <param name="placeholder">Optional placeholder sprite when item is null</param>
    public void SetItem(InventoryItem newItem, Sprite placeholder = null)
    {
        item = newItem;

        if (iconImage != null)
        {
            if (item != null)
                iconImage.sprite = item.icon;
            else if (placeholder != null)
                iconImage.sprite = placeholder;

            iconImage.enabled = true; // Always show the slot
        }
    }

    /// <summary>
    /// Handles right-click usage or equipping of the item.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (item == null || eventData.button != PointerEventData.InputButton.Right) return;

        if (item.category == ItemCategory.Active)
            PlayerInventory.Instance.UseActiveItem(item);
        else
            PlayerInventory.Instance.Equip(item);
    }

    #region Drag & Drop

    /// <summary>
    /// Called when dragging starts.
    /// Temporarily moves the slot to the root canvas and disables raycasts.
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item == null) return;

        originalParent = transform.parent;
        transform.SetParent(rootCanvas.transform, true); // Drag above all UI
        cg.blocksRaycasts = false;
    }

    /// <summary>
    /// Moves the slot with the pointer during dragging.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (item == null) return;
        rect.position = eventData.position;
    }

    /// <summary>
    /// Ends dragging. Checks for a valid target to swap items or reverts to original position.
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        cg.blocksRaycasts = true;

        // Detect UI elements under pointer
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var r in results)
        {
            var targetUI = r.gameObject.GetComponent<InventoryItemUI>();
            if (targetUI != null && targetUI != this)
            {
                HandleSwap(targetUI); // Swap items if valid target
                return;
            }
        }

        // No valid target: revert to original parent and position
        transform.SetParent(originalParent, true);
        rect.localPosition = Vector3.zero;
    }

    /// <summary>
    /// Handles swapping of items between two inventory slots.
    /// Handles all combinations of inventory vs equipment slots.
    /// </summary>
    private void HandleSwap(InventoryItemUI target)
    {
        var inv = PlayerInventory.Instance;

        // -------- Equipment -> Inventory --------
        if (isEquipmentSlot && !target.isEquipmentSlot)
        {
            inv.Unequip(item);

            if (target.item != null)
                inv.Equip(target.item);

            var tmp = target.item;
            target.SetItem(item);
            SetItem(tmp);
        }
        // -------- Inventory -> Equipment --------
        else if (!isEquipmentSlot && target.isEquipmentSlot)
        {
            inv.Equip(item);

            if (target.item != null)
                inv.AddItem(target.item);

            var tmp = target.item;
            target.SetItem(item);
            SetItem(tmp);
        }
        // -------- Inventory -> Inventory --------
        else if (!isEquipmentSlot && !target.isEquipmentSlot)
        {
            int idxA = inv.data.items.FindIndex(s => s.item == item);
            int idxB = inv.data.items.FindIndex(s => s.item == target.item);
            if (idxA >= 0 && idxB >= 0)
            {
                var tmp = inv.data.items[idxA];
                inv.data.items[idxA] = inv.data.items[idxB];
                inv.data.items[idxB] = tmp;
            }

            var tmpItem = target.item;
            target.SetItem(item);
            SetItem(tmpItem);
        }
        // -------- Equipment -> Equipment --------
        else if (isEquipmentSlot && target.isEquipmentSlot)
        {
            if (item != null) inv.Unequip(item);
            if (target.item != null) inv.Unequip(target.item);

            if (item != null) inv.Equip(item);
            if (target.item != null) inv.Equip(target.item);

            var tmpItem = target.item;
            target.SetItem(item);
            SetItem(tmpItem);
        }

        // Notify inventory and equipment listeners
        inv.RaiseInventoryChanged();
        inv.RaiseEquipmentChanged();

        // Reset both slots to original positions
        transform.SetParent(originalParent, true);
        rect.localPosition = Vector3.zero;
        target.rect.localPosition = Vector3.zero;
    }

    #endregion
}
