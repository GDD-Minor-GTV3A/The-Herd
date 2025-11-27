using UnityEngine;

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
        bool wasOpen = inventoryPanel.rootPanel.activeSelf;

        // Toggle UI
        inventoryPanel.ToggleOpen();

        // If it was closed before → now opening → refresh
        if (!wasOpen)
        {
            inventoryPanel.RefreshWearables();
            inventoryPanel.RefreshInventoryGrid();
        }
    }
}
