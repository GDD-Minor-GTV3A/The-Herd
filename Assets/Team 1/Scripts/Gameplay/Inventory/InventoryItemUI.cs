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
        if (item.category == ItemCategory.Active)
        {
            PlayerInventory.Instance.UseActiveItem(item);
        }
        else
        {
            PlayerInventory.Instance.Equip(item);
        }
    }

    // ----- Drag & Drop -----
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
        var inv = PlayerInventory.Instance;

        // Inventory grid swap
        bool thisInGrid = originalParent.name.ToLower().Contains("grid");
        bool otherInGrid = other.transform.parent.name.ToLower().Contains("grid");

        if (thisInGrid && otherInGrid)
        {
            int a = inv.data.items.FindIndex(s => s.item == item);
            int b = inv.data.items.FindIndex(s => s.item == other.item);
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
            // Equip/unequip swap
            if (other.item != null)
                inv.AddItem(other.item); // put existing into inventory
            inv.Equip(item);
        }

        // Refresh UI
        inv.RaiseInventoryChanged();
        inv.RaiseEquipmentChanged();
    }
}
