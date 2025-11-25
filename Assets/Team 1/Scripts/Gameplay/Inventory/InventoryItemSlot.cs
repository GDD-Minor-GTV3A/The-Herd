using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI References")]
    public Image image;                        // <-- The Icon child
    [SerializeField] private Transform dragCanvas;
    [SerializeField, HideInInspector] public InventoryItem item;

    public Transform originalParent;
    private int originalSiblingIndex;
    private Vector2 originalSizeDelta;

    public void InitializeItem(InventoryItem newItem)
    {
        this.item = newItem;
        if (newItem != null)
        {
            this.image.sprite = item.icon;
            this.image.enabled = true;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (image == null || image.sprite == null)
            return;

        originalParent = image.transform.parent;
        originalSiblingIndex = image.transform.GetSiblingIndex();

        RectTransform rt = (RectTransform)image.transform;
        originalSizeDelta = rt.sizeDelta;

        // Move to drag canvas
        image.transform.SetParent(dragCanvas, false);
        image.transform.SetAsLastSibling();
        image.raycastTarget = false;

        rt.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        ((RectTransform)image.transform).position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (image == null)
            return;

        // Restore to slot
        image.transform.SetParent(originalParent, false);
        image.transform.SetSiblingIndex(originalSiblingIndex);
        image.raycastTarget = true;

        // FIX: Layout Group messes up position → force reset
        RectTransform rt = (RectTransform)image.transform;
        rt.anchoredPosition = Vector2.zero;
        rt.localPosition = Vector3.zero;

        // FIX: Restore size broken by LayoutGroup
        rt.sizeDelta = originalSizeDelta;
    }
}