using UnityEngine;

/// <summary>
/// ScriptableObject representing a single inventory item.
/// Can be equipped, consumed, or stored in the inventory depending on its category.
/// Using ScriptableObject allows items to be created as assets in the editor.
/// </summary>
[CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject
{
    // ==============================
    // Identity
    // ==============================

    /// <summary>
    /// Display name of the item.
    /// Used in UI, tooltips, and logs.
    /// </summary>
    public string itemName;

    /// <summary>
    /// Icon representing the item visually in the UI.
    /// </summary>
    public Sprite icon;

    // ==============================
    // Category
    // ==============================

    /// <summary>
    /// Determines how the item behaves in inventory:
    /// e.g., equippable gear, consumable, trinket, or currency.
    /// </summary>
    public ItemCategory category;

    // ==============================
    // Optional Stats
    // ==============================

    /// <summary>
    /// Optional bonus health provided by this item when equipped.
    /// </summary>
    public int bonusHealth;

    /// <summary>
    /// Optional bonus damage provided by this item when equipped.
    /// </summary>
    public int bonusDamage;

    /// <summary>
    /// Optional bonus speed provided by this item when equipped.
    /// </summary>
    public int bonusSpeed;

    // ==============================
    // Internal Save Data
    // ==============================

    /// <summary>
    /// Persistent globally unique identifier (GUID) for this item.
    /// Used for saving/loading inventory data to ensure the correct item is referenced.
    /// </summary>
    [SerializeField, HideInInspector]
    private string guid;

    /// <summary>
    /// Public read-only access to the item's GUID.
    /// </summary>
    public string GUID => guid;

#if UNITY_EDITOR
    /// <summary>
    /// Automatically assigns a GUID when the item is first created or edited in the editor.
    /// Ensures each item asset is uniquely identifiable.
    /// </summary>
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(guid))
            guid = System.Guid.NewGuid().ToString();
    }
#endif
}
