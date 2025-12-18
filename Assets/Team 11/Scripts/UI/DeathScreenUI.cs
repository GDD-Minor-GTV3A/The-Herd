using UnityEngine;

public class DeathScreenUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject deathPanel;

    [Header("Respawn")]
    [SerializeField] private PlayerRespawnHandler respawnHandler;

    private bool isOpen;

    private void Awake()
    {
        if (deathPanel != null)
            deathPanel.SetActive(false);
    }

    public void Show()
    {
        Debug.Log($"[DeathScreenUI] Show() on '{name}' (id={GetInstanceID()}) panel='{deathPanel?.name}'");

        if (isOpen) return;
        isOpen = true;

        Time.timeScale = 0f;
        if (deathPanel != null) deathPanel.SetActive(true);

    }

    public void OnRetry()
    {
        Debug.Log($"[DeathScreenUI] OnRetry() on '{name}' (id={GetInstanceID()}) panel='{deathPanel?.name}'");

        Time.timeScale = 1f;

        isOpen = false;
        if (deathPanel != null) deathPanel.SetActive(false);

        if (respawnHandler == null)
        {
            Debug.LogError("[DeathScreenUI] respawnHandler is not assigned.");
            return;
        }

        respawnHandler.HandleDeath();
    }
}
