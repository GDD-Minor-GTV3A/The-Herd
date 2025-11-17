using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject rootPanel;

    [Header("Wearable Slot Images")]
    public Image headSlotImage;
    public Image chestSlotImage;
    public Image legsSlotImage;
    public Image bootsSlotImage;

    [Header("Trinket Slot Images (4 slots)")]
    public Image[] trinketSlotImages = new Image[4];

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
    public Transform contentParent;
    public GameObject rowPrefab;
    public Sprite itemPlaceholderSprite;

    private const int ITEMS_PER_ROW = 4;

    private void Awake()
    {
        if (rootPanel != null)
            rootPanel.SetActive(false); // UI starts hidden
    }

    // ==============================
    // UI Update Binding
    // ==============================
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

    public void ToggleOpen() => rootPanel.SetActive(!rootPanel.activeSelf);

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            this.ToggleOpen();
            RefreshWearables();
            RefreshInventoryGrid();
        }
    }

    // ==============================
    // Wearables + Trinkets UI
    // ==============================
    private void RefreshWearables()
    {
        if (PlayerInventory.Instance == null || PlayerInventory.Instance.data == null)
            return;

        var data = PlayerInventory.Instance.data;

        // Wearables
        headSlotImage.sprite = data.headgear != null ? headEquipped : headPlaceholder;
        chestSlotImage.sprite = data.chestwear != null ? chestEquipped : chestPlaceholder;
        legsSlotImage.sprite = data.legwear != null ? legsEquipped : legsPlaceholder;
        bootsSlotImage.sprite = data.boots != null ? bootsEquipped : bootsPlaceholder;

        // Trinkets
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

    // ==============================
    // INVENTORY GRID (scroll list)
    // ==============================

    public void RefreshInventoryGrid()
    {
        var inv = PlayerInventory.Instance;
        if (inv == null || inv.data == null) return;

        // Build list of inventory items
        List<InventoryItem> inventoryItems = new List<InventoryItem>();

        foreach (var item in inv.data.items)
            if (item != null)
                inventoryItems.Add(item);

        foreach (var tr in inv.data.trinkets)
            if (tr != null)
                inventoryItems.Add(tr);

        int totalItems = inventoryItems.Count;
        int neededRows = Mathf.CeilToInt(totalItems / (float)ITEMS_PER_ROW);

        // Ensure enough rows exist
        while (contentParent.childCount < neededRows)
            Instantiate(rowPrefab, contentParent);

        int itemIndex = 0;

        // Fill rows
        for (int row = 0; row < neededRows; row++)
        {
            Transform rowT = contentParent.GetChild(row);
            rowT.gameObject.SetActive(true);

            // Exactly 4 slots
            Image[] icons = new Image[ITEMS_PER_ROW];
            for (int i = 0; i < ITEMS_PER_ROW; i++)
                icons[i] = rowT.GetChild(i).GetComponent<Image>();

            for (int i = 0; i < ITEMS_PER_ROW; i++)
            {
                Image img = icons[i];

                if (itemIndex < totalItems)
                {
                    img.sprite = itemPlaceholderSprite;
                    img.color = Color.white;
                    itemIndex++;
                }
            }
        }
    }
}