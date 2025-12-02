using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.Inventory
{
    public class InventoryToggle : MonoBehaviour
    {
        [Header("UI References")]
        public InventoryUI inventoryPanel;


        InputAction inventoryAction;


        public void Initialize(InputAction inventoryAction)
        {
            this.inventoryAction = inventoryAction;

            this.inventoryAction.canceled += OnInventoryButtonPressed;
        }


        private void OnInventoryButtonPressed(InputAction.CallbackContext obj)
        {
            ToggleInventory();
        }


        public void ToggleInventory()
        {
            bool wasOpen = inventoryPanel.RootPanel.activeSelf;

            // Toggle UI
            inventoryPanel.ToggleOpen();

        }

        private void OnDestroy()
        {
            inventoryAction.canceled -= OnInventoryButtonPressed;
        }
    }
}