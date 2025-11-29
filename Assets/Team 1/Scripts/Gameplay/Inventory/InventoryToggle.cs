using UnityEngine;

namespace Gameplay.Inventory
{
    public class InventoryToggle : MonoBehaviour
    {
        [Header("UI References")]
        public InventoryUI inventoryPanel;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                ToggleInventory();
            }
        }

        public void ToggleInventory()
        {
            bool wasOpen = inventoryPanel.RootPanel.activeSelf;

            // Toggle UI
            inventoryPanel.ToggleOpen();

        }
    }
}