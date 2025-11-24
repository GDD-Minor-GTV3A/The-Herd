using System.Collections.Generic;

[System.Serializable]
public class InventoryData
{
    public List<InventoryStack> items = new List<InventoryStack>();

    public InventoryItem headgear;
    public InventoryItem chestwear;
    public InventoryItem legwear;
    public InventoryItem boots;

    public List<InventoryItem> trinkets = new List<InventoryItem>();

    public int scrolls = 0;
    public int reviveTotems = 0;
}