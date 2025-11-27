using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("UI References")]
    public Image image;                        // Icon child
    [SerializeField] private Transform dragCanvas;

    [SerializeField, HideInInspector] public InventoryItem item;

    [Header("Settings")]
    public bool draggable = true;              // Set to false for equipment/trinket slots

    public Transform originalParent;
    private int originalSiblingIndex;
    private Vector2 originalSizeDelta;

    public void InitializeItem(InventoryItem newItem)
    {
        this.item = newItem;
        if (image != null)
        {
            image.sprite = item?.icon;
            image.enabled = item != null;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!draggable || image == null || image.sprite == null) return;

        originalParent = image.transform.parent;
        originalSiblingIndex = image.transform.GetSiblingIndex();

        RectTransform rt = (RectTransform)image.transform;
        originalSizeDelta = rt.sizeDelta;

        image.transform.SetParent(dragCanvas, false);
        image.transform.SetAsLastSibling();
        image.raycastTarget = false;

        rt.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!draggable || image == null) return;
        ((RectTransform)image.transform).position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!draggable || image == null) return;

        // Restore to slot
        image.transform.SetParent(originalParent, false);
        image.transform.SetSiblingIndex(originalSiblingIndex);
        image.raycastTarget = true;

        RectTransform rt = (RectTransform)image.transform;
        rt.anchoredPosition = Vector2.zero;
        rt.localPosition = Vector3.zero;
        rt.sizeDelta = originalSizeDelta;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && item != null)
        {
            TryAutoEquip();
        }
    }

    private void TryAutoEquip()
    {

        if (item == null) return;

        if (!PlayerInventory.Instance.UseItem(item))
        {
            Debug.Log($"No valid equipment slot for {item.name}"); return;
        }

        Debug.Log($"Equipped {item.name} via right-click!");

        // Refresh UI
        InventoryUI.Instance.RefreshWearables();
        InventoryUI.Instance.RefreshInventoryGrid();
    }
}
