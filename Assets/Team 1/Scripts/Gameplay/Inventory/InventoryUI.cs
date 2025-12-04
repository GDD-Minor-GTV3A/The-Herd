using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Gameplay.Inventory
{
    /// <summary>
    /// Handles the visual representation of the player inventory, including wearables, trinkets, and the scrollable inventory grid.
    /// Compatible with InventoryItemSlot.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject rootPanel;
        public GameObject RootPanel { get => rootPanel; set => rootPanel = value; }

        [SerializeField] private TextMeshProUGUI scrolls;
        [SerializeField] private TextMeshProUGUI reviveTotems;

        [Header("Wearable Slots")]
        [SerializeField] private InventoryItemSlot headSlot;
        public InventoryItemSlot HeadSlot { get => headSlot; set => headSlot = value; }

        [SerializeField] private InventoryItemSlot chestSlot;
        public InventoryItemSlot ChestSlot { get => chestSlot; set => chestSlot = value; }

        [SerializeField] private InventoryItemSlot legsSlot;
        public InventoryItemSlot LegsSlot { get => legsSlot; set => legsSlot = value; }

        [SerializeField] private InventoryItemSlot bootsSlot;
        public InventoryItemSlot BootsSlot { get => bootsSlot; set => bootsSlot = value; }

        [Header("Trinket Slots (4)")]
        [SerializeField] private InventoryItemSlot[] trinketSlots = new InventoryItemSlot[4];
        public InventoryItemSlot[] TrinketSlots { get => trinketSlots; set => trinketSlots = value; }

        [Header("Inventory Grid")]
        [SerializeField] private Transform contentParent;   // Parent for grid rows
        public Transform ContentParent { get => contentParent; set => contentParent = value; }

        [SerializeField] private GameObject rowPrefab;      // Row prefab containing 4 slots
        public GameObject RowPrefab { get => rowPrefab; set => rowPrefab = value; }

        [SerializeField] private int itemsPerRow = 4;       // Slots per row
        public int ItemsPerRow { get => itemsPerRow; set => itemsPerRow = value; }

        private void Awake()
        {
            if (rootPanel != null)
                rootPanel.SetActive(false);

            if (PlayerInventory.Instance != null)
            {
                PlayerInventory.Instance.OnEquipmentChanged += RefreshWearables;
                PlayerInventory.Instance.OnInventoryChanged += RefreshInventoryGrid;
            }
            else
            {
                // Wait for the singleton to be initialized
                StartCoroutine(SubscribeWhenReady());
            }
        }

        private System.Collections.IEnumerator SubscribeWhenReady()
        {
            while (PlayerInventory.Instance == null)
                yield return null;

            PlayerInventory.Instance.OnEquipmentChanged += RefreshWearables;
            PlayerInventory.Instance.OnInventoryChanged += RefreshInventoryGrid;
        }

        public void ToggleOpen()
        {
            rootPanel.SetActive(!rootPanel.activeSelf);
            if (rootPanel.activeSelf)
            {
                RefreshWearables();
                RefreshInventoryGrid();
            }
        }

        #region Wearables + Trinkets

        private void RefreshWearables()
        {
            if (PlayerInventory.Instance?.data == null) return;
            var data = PlayerInventory.Instance.data;

            headSlot.InitializeItem(data.headgear);
            chestSlot.InitializeItem(data.chestwear);
            legsSlot.InitializeItem(data.legwear);
            bootsSlot.InitializeItem(data.boots);

            for (int i = 0; i < trinketSlots.Length; i++)
            {
                InventoryItem trinket = (data.trinkets != null && i < data.trinkets.Count) ? data.trinkets[i] : null;
                trinketSlots[i].InitializeItem(trinket);
            }
        }

        #endregion

        #region Inventory Grid

        private void RefreshInventoryGrid()
        {
            if (PlayerInventory.Instance?.data == null) return;
            var inv = PlayerInventory.Instance;

            // Build a flat list with item + amount
            List<(InventoryItem item, int amount)> inventoryItems = new List<(InventoryItem, int)>();
            foreach (var stack in inv.data.items)
                if (stack?.Item != null)
                    inventoryItems.Add((stack.Item, stack.Uses)); // <-- store amount too

            int totalItems = inventoryItems.Count;
            int totalRows = Mathf.CeilToInt(totalItems / (float)itemsPerRow);

            int minRows = 5;
            totalRows = Mathf.Max(totalRows, minRows);

            while (contentParent.childCount < totalRows)
                Instantiate(rowPrefab, contentParent);

            int itemIndex = 0;

            for (int row = 0; row < contentParent.childCount; row++)
            {
                Transform rowT = contentParent.GetChild(row);
                rowT.gameObject.SetActive(true);

                for (int col = 0; col < itemsPerRow; col++)
                {
                    Transform slotBG = rowT.GetChild(col);
                    InventoryItemSlot slotUI = slotBG.GetComponentInChildren<InventoryItemSlot>();
                    if (slotUI == null) continue;

                    if (itemIndex < totalItems)
                    {
                        var entry = inventoryItems[itemIndex];
                        slotUI.InitializeItem(entry.item, entry.amount); // <-- pass stack count
                        itemIndex++;
                    }
                    else
                    {
                        slotUI.InitializeItem(null, 0); // <-- empty slot, no stack
                    }
                }
            }
            if (PlayerInventory.Instance.data.scrolls > 1)
            {
                scrolls.gameObject.SetActive(true);
                scrolls.SetText(PlayerInventory.Instance.data.scrolls.ToString());
            }
            else
            {
                scrolls.gameObject.SetActive(false);
            }
            if (PlayerInventory.Instance.data.reviveTotems > 1)
            {
                reviveTotems.gameObject.SetActive(true);
                reviveTotems.SetText(PlayerInventory.Instance.data.reviveTotems.ToString());
            }
            else
            {
                reviveTotems.gameObject.SetActive(false);
            }
        }
        #endregion
    }
}