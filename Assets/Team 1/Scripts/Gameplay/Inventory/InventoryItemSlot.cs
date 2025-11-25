using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Handles drag-and-drop behavior for an inventory item slot.
/// Creates a temporary drag icon that follows the mouse cursor while dragging,
/// and destroys it when the drag ends.
/// </summary>
public class InventoryItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI References")]
    public Image image;                     // The original slot image in the inventory UI
    [SerializeField] private Transform dragCanvas; // Canvas to render the drag icon above all UI elements

    // Runtime references for the drag icon
    private GameObject dragIcon;            // Temporary clone of the slot image while dragging
    private RectTransform dragRect;         // RectTransform of the drag icon

    /// <summary>
    /// Called when the user starts dragging this inventory slot.
    /// Creates a clone of the slot image to visually follow the cursor.
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (image == null || image.sprite == null)
            return; // No image to drag

        // Create a new GameObject for the drag icon
        dragIcon = new GameObject("DragIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        dragIcon.transform.SetParent(dragCanvas, false); // Keep local scale and position correct
        dragIcon.transform.SetAsLastSibling();           // Ensure it renders above all other UI

        dragRect = dragIcon.GetComponent<RectTransform>();
        dragRect.sizeDelta = ((RectTransform)image.transform).sizeDelta; // Match slot size
        dragRect.localScale = Vector3.one;                                // Ensure no scaling issues

        // Copy the sprite and disable raycast targeting so it doesn’t block pointer events
        Image dragImage = dragIcon.GetComponent<Image>();
        dragImage.sprite = image.sprite;
        dragImage.raycastTarget = false;

        // Initialize position to match the original slot
        dragRect.position = image.transform.position;
    }

    /// <summary>
    /// Called each frame while dragging.
    /// Moves the drag icon to follow the cursor.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
            dragRect.position = eventData.position;
    }

    /// <summary>
    /// Called when the user releases the drag.
    /// Destroys the temporary drag icon.
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
            Destroy(dragIcon);
    }
}
