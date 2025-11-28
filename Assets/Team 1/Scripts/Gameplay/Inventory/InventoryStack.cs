namespace Gameplay.Inventory
{
    [System.Serializable]
    public class InventoryStack
    {
        private InventoryItem item;
        public InventoryItem Item { get => item; set => item = value; }

        private int uses;   // Only relevant for active items
        public int Uses { get => uses; set => uses = value; }

        public InventoryStack(InventoryItem item, int uses)
        {
            this.item = item;
            this.uses = uses;
        }
    }
}