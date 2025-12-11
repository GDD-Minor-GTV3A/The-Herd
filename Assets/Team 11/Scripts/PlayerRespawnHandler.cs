using UnityEngine;

public class PlayerRespawnHandler : MonoBehaviour
{
    [Header("Optional health settings")]
    [SerializeField]
    [Tooltip("If you have a health system, use this as the value to restore on respawn.")]
    private float healthOnRespawn = 100f;

    public void HandleDeath()
    {
        if (RespawnManager.Instance == null)
        {
            Debug.LogWarning("[PlayerRespawnHandler] No RespawnManager in scene. Cannot respawn.");
            return;
        }

        SpawnPoint target = RespawnManager.Instance.GetBestRespawn(transform.position);
        if (target == null)
        {
            Debug.LogWarning("[PlayerRespawnHandler] No valid respawn point found. Staying in place.");
            return;
        }

        RespawnAtPoint(target);
    }

    public void RespawnAtLastMemoryAnchor()
    {
        if (RespawnManager.Instance == null)
        {
            Debug.LogWarning("[PlayerRespawnHandler] No RespawnManager in scene. Cannot respawn.");
            return;
        }

        SpawnPoint target = RespawnManager.Instance.GetCurrentRespawn();
        if (target == null)
        {
            Debug.LogWarning("[PlayerRespawnHandler] No current respawn point. Cannot respawn.");
            return;
        }

        RespawnAtPoint(target);
    }

    private void RespawnAtPoint(SpawnPoint point)
    {
        Transform t = point.transform;
        transform.SetPositionAndRotation(t.position, t.rotation);

        // Plug in your own health/sanity reset here if you want:
        // var health = GetComponent<YourHealthComponent>();
        // if (health != null) health.SetHealth(healthOnRespawn);

        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
