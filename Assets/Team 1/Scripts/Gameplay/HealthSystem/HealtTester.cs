using UnityEngine;
using Gameplay; // <- make sure this matches your namespace

public class HealthTester : MonoBehaviour
{
    private Health playerHealth;

    void Start()
    {
        playerHealth = GetComponent<Health>();

        if (playerHealth != null)
        {
            // Subscribe to events
            playerHealth.OnHealthChanged.AddListener(OnHealthChanged);
            playerHealth.OnDeath.AddListener(OnDeath);

            // Test calls
            Debug.Log("Initial Health: " + playerHealth.CurrentHealth);
            playerHealth.TakeDamage(20f);
            playerHealth.Heal(10f);
            playerHealth.TakeDamage(200f); // should trigger death
        }
        else
        {
            Debug.LogError("No Health component found on this GameObject!");
        }
    }

    private void OnHealthChanged(float current, float max)
    {
        Debug.Log($"Health Updated: {current}/{max}");
    }

    private void OnDeath()
    {
        Debug.Log("Player has died!");
    }
}
