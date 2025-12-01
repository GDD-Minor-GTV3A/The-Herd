using System.Collections.Generic;

using UnityEngine;

namespace Gameplay.Inventory
{
    [System.Serializable]
    public class InventorySaveData
    {
        [SerializeField] private List<string> inventoryItemIDs = new List<string>();
        public List<string> InventoryItemIDs{ get => inventoryItemIDs; set => inventoryItemIDs = value; }

        [SerializeField] private List<int> activeItemUses = new List<int>();
        public List<int> ActiveItemUses { get => activeItemUses; set => activeItemUses = value; }

        [SerializeField] private string headID;
        public string HeadID { get => headID; set => headID = value; }

        [SerializeField] private string chestID;
        public string ChestID { get => chestID; set => chestID = value; }

        [SerializeField] private string legsID;
        public string LegsID { get => legsID; set => legsID = value; }

        [SerializeField] private string bootsID;
        public string BootsID { get => bootsID; set => bootsID = value; }

        [SerializeField] private List<string> trinketIDs = new List<string>();
        public List<string> TrinketIDs { get => trinketIDs; set => trinketIDs = value; }

        [SerializeField] private int scrolls;
        public int Scrolls { get => scrolls; set => scrolls = value; }

        [SerializeField] private int reviveTotems;
        public int ReviveTotems { get => reviveTotems; set => reviveTotems = value; }
    }
}