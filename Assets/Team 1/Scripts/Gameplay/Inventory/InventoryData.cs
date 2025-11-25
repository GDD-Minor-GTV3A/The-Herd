using System.Collections.Generic;

/// <summary>
/// Serializable container for all player inventory-related data.
/// This class holds both consumable items and equipped gear,
/// and is designed to be easily saved/loaded.
/// </summary>
[System.Serializable]
public class InventoryData
{
    /// <summary>
    /// List of general inventory items stacked with their quantities.
    /// </summary>
    public List<InventoryStack> items = new();

    /// <summary>
    /// Currently equipped headgear item. Null if nothing is equipped.
    /// </summary>
    public InventoryItem headgear;

    /// <summary>
    /// Currently equipped chestwear item. Null if nothing is equipped.
    /// </summary>
    public InventoryItem chestwear;

    /// <summary>
    /// Currently equipped legwear item. Null if nothing is equipped.
    /// </summary>
    public InventoryItem legwear;

    /// <summary>
    /// Currently equipped boots item. Null if nothing is equipped.
    /// </summary>
    public InventoryItem boots;

    /// <summary>
    /// List of equipped trinkets. Maximum size determined by PlayerInventory logic.
    /// </summary>
    public List<InventoryItem> trinkets = new();

    /// <summary>
    /// Count of scrolls the player currently possesses.
    /// Scrolls are considered a consumable currency item.
    /// </summary>
    public int scrolls;

    /// <summary>
    /// Count of revive totems the player currently possesses.
    /// Revive totems are consumable items used to revive the player.
    /// </summary>
    public int reviveTotems;
}
