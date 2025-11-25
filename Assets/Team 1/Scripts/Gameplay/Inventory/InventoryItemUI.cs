using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UI representation of a single inventory item or equipment slot using ScriptableObjects.
/// Handles display of item icons, click interactions, and drag-and-drop functionality.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class InventoryItemUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Item Data")]
    public InventoryItem item;          // The ScriptableObject item represented by this slot
    public Image iconImage;             // Image component displaying the item's icon

    [Header("Slot Info")]
    public bool isEquipmentSlot = false;
    public ItemCategory slotCategory;

    // Private references
    private CanvasGroup cg;
    private RectTransform rect;
    private Transform originalParent;
    private Canvas rootCanvas;

    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        rect = GetComponent<RectTransform>();
        rootCanvas = GetComponentInParent<Canvas>();
    }

    /// <summary>
    /// Updates the UI slot with the given item.
    /// </summary>
    public void SetItem(InventoryItem newItem)
    {
        item = newItem;

        if (iconImage != null)
        {
            if (item != null)
                iconImage.sprite = item.icon;       // <-- Assign Sprite from ScriptableObject
            else
                iconImage.sprite = null;

            iconImage.enabled = true;
            iconImage.color = item != null ? Color.white : Color.clear;
        }
    }

    /// <summary>
    /// Handles right-click use/equip.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (item == null || eventData.button != PointerEventData.InputButton.Right) return;

        var inv = PlayerInventory.Instance;

        switch (item.category)
        {
            case ItemCategory.Active:
                inv.UseActiveItem(item);
                break;
            case ItemCategory.Scroll:
                inv.SpendScroll(1);
                break;
            case ItemCategory.ReviveTotem:
                inv.AddReviveTotem(-1);
                break;

            default:
                inv.Equip(item);
                break;
        }
    }

    #region Drag & Drop

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item == null) return;

        originalParent = transform.parent;
        transform.SetParent(rootCanvas.transform, true);
        cg.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item == null) return;
        rect.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        cg.blocksRaycasts = true;

        // Raycast for target
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var r in results)
        {
            var targetUI = r.gameObject.GetComponent<InventoryItemUI>();
            if (targetUI != null && targetUI != this)
            {
                HandleSwap(targetUI);
                return;
            }
        }

        // Revert if no valid target
        transform.SetParent(originalParent, true);
        rect.localPosition = Vector3.zero;
    }

    private void HandleSwap(InventoryItemUI target)
    {
        var inv = PlayerInventory.Instance;

        // Inventory <-> Inventory
        if (!isEquipmentSlot && !target.isEquipmentSlot)
        {
            int idxA = inv.data.items.FindIndex(s => s.item == item);
            int idxB = inv.data.items.FindIndex(s => s.item == target.item);
            if (idxA >= 0 && idxB >= 0)
            {
                var tmp = inv.data.items[idxA];
                inv.data.items[idxA] = inv.data.items[idxB];
                inv.data.items[idxB] = tmp;
            }
        }

        // Inventory <-> Equipment
        else if (!isEquipmentSlot && target.isEquipmentSlot)
        {
            inv.Equip(item);
            if (target.item != null) inv.AddItem(target.item);
        }
        else if (isEquipmentSlot && !target.isEquipmentSlot)
        {
            inv.Unequip(item);
            if (target.item != null) inv.Equip(target.item);
        }

        // Equipment <-> Equipment
        else if (isEquipmentSlot && target.isEquipmentSlot)
        {
            if (item != null) inv.Unequip(item);
            if (target.item != null) inv.Unequip(target.item);
            if (item != null) inv.Equip(item);
            if (target.item != null) inv.Equip(target.item);
        }

        // Swap visuals
        var tmpItem = target.item;
        target.SetItem(item);
        SetItem(tmpItem);

        // Notify inventory
        inv.RaiseInventoryChanged();
        inv.RaiseEquipmentChanged();

        // Reset positions
        transform.SetParent(originalParent, true);
        rect.localPosition = Vector3.zero;
        target.rect.localPosition = Vector3.zero;
    }

    #endregion
}
