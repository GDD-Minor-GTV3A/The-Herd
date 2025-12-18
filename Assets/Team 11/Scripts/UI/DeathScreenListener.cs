using UnityEngine;

public class DeathScreenListener : MonoBehaviour
{
    [SerializeField] private DeathScreenUI deathScreenUI;

    private void Reset()
    {
        deathScreenUI = GetComponent<DeathScreenUI>();
    }

    // Call this from whatever currently fires your DeathEvent / death pipeline
    public void OnPlayerDied()
    {
        if (deathScreenUI != null)
            deathScreenUI.Show();
    }
}
