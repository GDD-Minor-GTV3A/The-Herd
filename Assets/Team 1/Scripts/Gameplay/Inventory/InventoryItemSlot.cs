using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gameplay.Inventory
{

    public class InventoryItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        [Header("UI References")]
        [SerializeField] private Image image;                        // Icon child
        public Image Image { get => image; set => image = value; }

        [SerializeField] private TextMeshProUGUI countText;
        public TextMeshProUGUI CountText { get => countText; set => countText = value; }

        [SerializeField] private Transform dragCanvas;
        public Transform DragCanvas { get => dragCanvas; set => dragCanvas = value; }

        [SerializeField, HideInInspector] private InventoryItem item;
        public InventoryItem Item { get => item; set => item = value; }

        [SerializeField, HideInInspector] private int count;
        public int Count { get => count; set => count = value; }

        [Header("Settings")]
        [SerializeField] private bool _draggable = true;              // Set to false for equipment/trinket slots

        [SerializeField] private Transform originalParent;
        public Transform OriginalParent { get => originalParent; set => originalParent = value; }

        [SerializeField] private int originalSiblingIndex;
        public int OriginalSiblingIndex { get => originalSiblingIndex; set => originalSiblingIndex = value; }

        public void InitializeItem(InventoryItem newItem, int count = -1)
        {
            this.count = count;
            this.item = newItem;
            image.sprite = null;
            if (image != null)
            {
                image.sprite = item?.icon;
                image.enabled = item != null;
                RefreshCount();
            }
        }

        public void RefreshCount()
        {
            if (countText == null) return;

            if (item != null && count > 1 && item.stackable)
            {
                countText.gameObject.SetActive(true);
                countText.SetText(count.ToString());
            }
            else
            {
                countText.gameObject.SetActive(false);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!_draggable || image == null || image.sprite == null) return;

            originalParent = image.transform.parent;
            image.transform.SetParent(dragCanvas, false);
            image.transform.SetAsLastSibling();
            image.raycastTarget = false;

        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_draggable || image == null) return;

            Vector2 localPoint;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    (RectTransform)dragCanvas.transform,
                    eventData.position,
                    eventData.pressEventCamera,
                    out localPoint))
            {
                ((RectTransform)image.transform).localPosition = new Vector3(localPoint.x, localPoint.y, 0);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_draggable || image == null) return;

            // Restore to slot
            RestoreToSlot();
        }

        public void RestoreToSlot()
        {
            image.transform.SetParent(originalParent, false);
            image.transform.SetSiblingIndex(originalSiblingIndex);
            image.raycastTarget = true;
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
            PlayerInventory.Instance.UseItem(item);
        }
    }
}