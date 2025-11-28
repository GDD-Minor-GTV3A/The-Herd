namespace Gameplay.Inventory
{
    [System.Serializable]
    public class InventoryStack
    {
        public InventoryItem item;
        public int uses;   // Only relevant for active items

        public InventoryStack(InventoryItem item, int uses)
        {
            this.item = item;
            this.uses = uses;
        }
    }
}