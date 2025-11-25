using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the visual representation of the player inventory, including wearables, trinkets, and the scrollable inventory grid.
/// Provides debug shortcuts for toggling the inventory panel and updating UI in real-time.
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject rootPanel; // Main inventory UI panel

    [Header("Wearable Slot Images")]
    public Image headSlotImage;
    public Image chestSlotImage;
    public Image legsSlotImage;
    public Image bootsSlotImage;

    [Header("Trinket Slot Images (4 slots)")]
    public Image[] trinketSlotImages = new Image[4]; // Array of trinket UI slots

    [Header("Wearable Placeholders")]
    public Sprite headPlaceholder;
    public Sprite chestPlaceholder;
    public Sprite legsPlaceholder;
    public Sprite bootsPlaceholder;

    [Header("Wearable Equipped Generic Icons")]
    public Sprite headEquipped;
    public Sprite chestEquipped;
    public Sprite legsEquipped;
    public Sprite bootsEquipped;

    [Header("Trinket Placeholders")]
    public Sprite trinketPlaceholder;

    [Header("Trinket Equipped Generic Icon")]
    public Sprite trinketEquipped;

    [Header("Inventory Grid")]
    public Transform contentParent;   // Parent transform for grid rows
    public GameObject rowPrefab;      // Prefab containing 4 inventory slots
    public Sprite itemPlaceholderSprite; // Generic icon for inventory items

    private const int ITEMS_PER_ROW = 4; // Each row in the grid contains 4 slots

    private void Awake()
    {
        // Hide inventory panel initially
        if (rootPanel != null)
            rootPanel.SetActive(false);
    }

    #region UI Binding

    private void OnEnable()
    {
        if (PlayerInventory.Instance == null) return;

        // Subscribe to inventory and equipment change events
        PlayerInventory.Instance.OnEquipmentChanged += RefreshWearables;
        PlayerInventory.Instance.OnInventoryChanged += RefreshInventoryGrid;

        // Refresh UI immediately on enable
        RefreshWearables();
        RefreshInventoryGrid();
    }

    private void OnDisable()
    {
        if (PlayerInventory.Instance == null) return;

        // Unsubscribe to prevent memory leaks
        PlayerInventory.Instance.OnEquipmentChanged -= RefreshWearables;
        PlayerInventory.Instance.OnInventoryChanged -= RefreshInventoryGrid;
    }

    /// <summary>
    /// Toggles the main inventory panel visibility
    /// </summary>
    public void ToggleOpen() => rootPanel.SetActive(!rootPanel.activeSelf);

    private void Update()
    {
        // Shortcut: toggle inventory panel with I key
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleOpen();
            RefreshWearables();
            RefreshInventoryGrid();
        }
    }

    #endregion

    #region Wearables + Trinkets

    /// <summary>
    /// Updates UI to reflect currently equipped wearables and trinkets
    /// </summary>
    private void RefreshWearables()
    {
        if (PlayerInventory.Instance == null || PlayerInventory.Instance.data == null)
            return;

        var data = PlayerInventory.Instance.data;

        // Update wearable slots
        headSlotImage.sprite = data.headgear != null ? headEquipped : headPlaceholder;
        chestSlotImage.sprite = data.chestwear != null ? chestEquipped : chestPlaceholder;
        legsSlotImage.sprite = data.legwear != null ? legsEquipped : legsPlaceholder;
        bootsSlotImage.sprite = data.boots != null ? bootsEquipped : bootsPlaceholder;

        // Update trinket slots
        for (int i = 0; i < 4; i++)
        {
            Image slot = trinketSlotImages[i];
            if (slot == null) continue;

            bool hasTrinket = data.trinkets != null &&
                              i < data.trinkets.Count &&
                              data.trinkets[i] != null;

            slot.sprite = hasTrinket ? trinketEquipped : trinketPlaceholder;
        }
    }

    #endregion

    #region Inventory Grid (Scroll List)

    /// <summary>
    /// Populates the inventory grid with the current items and trinkets.
    /// Adds rows if needed and assigns item placeholders for all items.
    /// </summary>
    public void RefreshInventoryGrid()
    {
        var inv = PlayerInventory.Instance;
        if (inv == null || inv.data == null) return;

        // Combine inventory items and trinkets into a single list
        List<InventoryItem> inventoryItems = new List<InventoryItem>();
        foreach (var item in inv.data.items)
            if (item != null) inventoryItems.Add(item.item);

        foreach (var tr in inv.data.trinkets)
            if (tr != null) inventoryItems.Add(tr);

        int totalItems = inventoryItems.Count;
        int neededRows = Mathf.CeilToInt(totalItems / (float)ITEMS_PER_ROW);

        // Ensure enough rows exist
        while (contentParent.childCount < neededRows)
            Instantiate(rowPrefab, contentParent);

        int itemIndex = 0;

        // Fill each row with items
        for (int row = 0; row < neededRows; row++)
        {
            Transform rowT = contentParent.GetChild(row);
            rowT.gameObject.SetActive(true);

            // Array to store slot Image references
            Image[] icons = new Image[ITEMS_PER_ROW];
            for (int i = 0; i < ITEMS_PER_ROW; i++)
                icons[i] = rowT.GetChild(i).GetComponent<Image>();

            // Assign item sprites
            for (int i = 0; i < ITEMS_PER_ROW; i++)
            {
                Image img = icons[i];

                if (itemIndex < totalItems)
                {
                    img.sprite = itemPlaceholderSprite; // Use generic placeholder for now
                    img.color = Color.white;            // Ensure visible
                    itemIndex++;
                }
            }
        }
    }

    #endregion
}
