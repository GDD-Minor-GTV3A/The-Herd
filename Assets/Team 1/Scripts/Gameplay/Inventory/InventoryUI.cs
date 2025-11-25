using System.Collections.Generic;

using UnityEngine;

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
    }

    private void OnEnable()
    {
        if (PlayerInventory.Instance == null) return;

        PlayerInventory.Instance.OnEquipmentChanged += RefreshWearables;
        PlayerInventory.Instance.OnInventoryChanged += RefreshInventoryGrid;

        RefreshWearables();
        RefreshInventoryGrid();
    }

    private void OnDisable()
    {
        if (PlayerInventory.Instance == null) return;

        PlayerInventory.Instance.OnEquipmentChanged -= RefreshWearables;
        PlayerInventory.Instance.OnInventoryChanged -= RefreshInventoryGrid;
    }

    public void ToggleOpen()
    {
        rootPanel.SetActive(!rootPanel.activeSelf);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleOpen();
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

    public void RefreshInventoryGrid()
    {
        if (PlayerInventory.Instance?.data == null) return;
        var inv = PlayerInventory.Instance;

        // Flatten inventory items (ignore stack counts for UI)
        List<InventoryItem> inventoryItems = new List<InventoryItem>();
        foreach (var stack in inv.data.items)
            if (stack?.item != null) inventoryItems.Add(stack.item);

        int totalItems = inventoryItems.Count;
        int neededRows = Mathf.CeilToInt(totalItems / (float)itemsPerRow);

        // Instantiate rows if needed
        while (contentParent.childCount < neededRows)
            Instantiate(rowPrefab, contentParent);

        int itemIndex = 0;

        for (int row = 0; row < contentParent.childCount; row++)
        {
            Transform rowT = contentParent.GetChild(row);
            rowT.gameObject.SetActive(row < neededRows);

            for (int col = 0; col < itemsPerRow; col++)
            {
                Transform slotBG = rowT.GetChild(col);      // Each slot in row
                InventoryItemSlot slotUI = slotBG.GetComponentInChildren<InventoryItemSlot>();
                if (slotUI == null) continue;

                if (itemIndex < totalItems)
                {
                    slotUI.InitializeItem(inventoryItems[itemIndex]);
                    itemIndex++;
                }
                else
                {
                    slotUI.InitializeItem(null); // empty slot
                }
            }
        }
    }

    #endregion
}
