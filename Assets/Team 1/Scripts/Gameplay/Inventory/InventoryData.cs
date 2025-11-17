using System.Collections.Generic;

[System.Serializable]
public class InventoryData
{
    public List<InventoryItem> items = new List<InventoryItem>();

    // Equipped slots
    public InventoryItem headgear;
    public InventoryItem chestwear;
    public InventoryItem legwear;
    public InventoryItem boots;

    // Multiple trinkets
    public List<InventoryItem> trinkets = new List<InventoryItem>();

    // Currencies
    public int scrolls = 0;
    public int reviveTotems = 0;
}
