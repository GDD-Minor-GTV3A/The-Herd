using System.Collections.Generic;

[System.Serializable]
public class InventorySaveData
{
    public List<string> inventoryItemIDs = new List<string>();
    public List<int> activeItemUses = new List<int>();

    public string headID;
    public string chestID;
    public string legsID;
    public string bootsID;

    public List<string> trinketIDs = new List<string>();

    public int scrolls;
    public int reviveTotems;
}