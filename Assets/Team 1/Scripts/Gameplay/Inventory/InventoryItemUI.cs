using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class InventoryItemUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image iconImage;
    public InventoryItem item;

    private Transform originalParent;
    private CanvasGroup group;
    private RectTransform rect;
    private Canvas rootCanvas;

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
        rect = GetComponent<RectTransform>();
        rootCanvas = GetComponentInParent<Canvas>();
    }

    public void SetItem(InventoryItem newItem)
    {
        item = newItem;
        iconImage.sprite = item ? item.icon : null;
        iconImage.enabled = item != null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right) return;
        if (item == null) return;

        // Right click: use if active, else quick equip
        if (item.isActiveItem)
        {
            PlayerInventory.Instance.UseActiveItem(item);
        }
        else
        {
            PlayerInventory.Instance.Equip(item);
        }
    }

    // ----- Drag & Drop (simple) -----
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item == null) return;
        originalParent = transform.parent;
        transform.SetParent(rootCanvas.transform, true);
        group.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item == null) return;
        rect.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        group.blocksRaycasts = true;

        // Check what's under pointer
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        foreach (var r in results)
        {
            var other = r.gameObject.GetComponent<InventoryItemUI>();
            if (other != null && other != this)
            {
                // swap items between slots (simple swap logic)
                SwapWith(other);
                return;
            }
        }

        // revert
        transform.SetParent(originalParent, true);
        rect.localPosition = Vector3.zero;
    }

    private void SwapWith(InventoryItemUI other)
    {
        // If either is wearable slot (parent name indicates slot), treat accordingly.
        // Basic behaviour: swap inventory items between parents in the runtime model.
        var myParentName = originalParent.name.ToLower();
        var otherParentName = other.transform.parent.name.ToLower();

        // For simplicity: operate on PlayerInventory.data lists/slots.

        // If both are inventory grid slots: swap positions in data.items
        if (originalParent.name.Contains("ItemsGrid") && other.transform.parent.name.Contains("ItemsGrid"))
        {
            var inv = PlayerInventory.Instance;
            int a = inv.data.items.IndexOf(item);
            int b = inv.data.items.IndexOf(other.item);
            if (a >= 0 && b >= 0)
            {
                var tmp = inv.data.items[a];
                inv.data.items[a] = inv.data.items[b];
                inv.data.items[b] = tmp;
                inv.RaiseInventoryChanged();
            }
        }
        else
        {
            // Generic: if other is a wearable slot (wearablesParent), then equip/unequip logic:
            // If other is an equip slot (has item and belongs to wearables): perform quick-equip
            // We'll implement simple fallback: if this.item is equipable to the other slot, swap via Equip/Unequip.
            // Quick solution: unequip other.item (put into inventory) then equip this.item
            if (other.item != null) PlayerInventory.Instance.AddItem(other.item);
            PlayerInventory.Instance.Equip(item);
            // remove original from inventory list
            PlayerInventory.Instance.RemoveItem(item);
        }

        // ui refresh
        PlayerInventory.Instance.RaiseInventoryChanged();
        PlayerInventory.Instance.RaiseEquipmentChanged();
    }
}
