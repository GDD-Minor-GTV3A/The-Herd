using UnityEngine;

/// <summary>
/// Simple manager to control the visibility of the inventory panel.
/// Provides a single toggle method for UI or input binding.
/// </summary>
public class InventoryUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject inventoryPanel; // The main inventory UI panel

    private bool isOpen = false; // Tracks whether the inventory panel is currently open

    private void Start()
    {
        // Ensure inventory panel is hidden on game start
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
        else
            Debug.LogWarning("Inventory panel reference is missing in InventoryUIManager.");
    }

    /// <summary>
    /// Toggles the inventory panel's visibility.
    /// Can be called from UI buttons or input handlers.
    /// </summary>
    public void ToggleInventory()
    {
        isOpen = !isOpen;

        if (inventoryPanel != null)
            inventoryPanel.SetActive(isOpen);
        else
            Debug.LogWarning("Attempted to toggle inventory panel, but reference is missing.");
    }
}
