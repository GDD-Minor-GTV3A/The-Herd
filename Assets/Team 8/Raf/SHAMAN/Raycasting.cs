using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    public GameObject projectilePrefab; // Assign in Inspector
    public Transform playerTarget;      // Assign player in Inspector
    public float spawnInterval = 2f;    // Time between spawns
    public float spawnOffset = 1f;      // Offset from spawner position
    public bool lockPosition = true;    // Lock spawner position to prevent movement

    private float timer;
    private Vector3 initialPosition;

    private void Start()
    {
        // Store initial position for locking
        initialPosition = transform.position;
        Debug.Log($"[ProjectileSpawner] Initialized at position {initialPosition}");
    }

    private void Update()
    {
        // Lock spawner position if enabled
        if (lockPosition && transform.position != initialPosition)
        {
            Debug.LogWarning($"[ProjectileSpawner] Position changed to {transform.position}. Forcing back to {initialPosition}.");
            transform.position = initialPosition;
        }

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnProjectile();
            timer = 0f;
        }
    }

    private void SpawnProjectile()
    {
        if (playerTarget == null)
        {
            Debug.LogWarning("[ProjectileSpawner] No player target assigned. Skipping spawn.");
            return;
        }

        // Calculate direction to player
        Vector3 direction = (playerTarget.position - transform.position).normalized;
        Vector3 spawnPos = transform.position + direction * spawnOffset;

        // Instantiate and set rotation
        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(direction));
        HomingProjectile homing = proj.GetComponent<HomingProjectile>();
        if (homing != null)
        {
            homing.SetTarget(playerTarget, transform.position);
            Debug.Log($"[ProjectileSpawner] Spawned projectile at {spawnPos} targeting player at {playerTarget.position}");
        }
        else
        {
            Debug.LogError("[ProjectileSpawner] HomingProjectile component missing on projectile prefab!");
        }
    }
}