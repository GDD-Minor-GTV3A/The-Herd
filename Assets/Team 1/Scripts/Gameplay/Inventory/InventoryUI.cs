using System.Collections.Generic;

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
        public GameObject rootPanel;

        [Header("Wearable Slots")]
        public InventoryItemSlot headSlot;
        public InventoryItemSlot chestSlot;
        public InventoryItemSlot legsSlot;
        public InventoryItemSlot bootsSlot;

        [Header("Trinket Slots (4)")]
        public InventoryItemSlot[] trinketSlots = new InventoryItemSlot[4];

        [Header("Inventory Grid")]
        public Transform contentParent;   // Parent for grid rows
        public GameObject rowPrefab;      // Row prefab containing 4 slots
        public int itemsPerRow = 4;       // Slots per row

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

        public static InventoryUI Instance { get; private set; }

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
                if (stack?.item != null)
                    inventoryItems.Add((stack.item, stack.uses)); // <-- store amount too

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
        }

        #endregion
    }
}