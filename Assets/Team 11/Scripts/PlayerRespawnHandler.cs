using UnityEngine;

public class PlayerRespawnHandler : MonoBehaviour
{
    [Header("Debug Logs")]
    [SerializeField] private bool debugLogs = true;

    public void HandleDeath()
    {
        if (debugLogs)
            Debug.Log("☠️ [PlayerRespawnHandler] HandleDeath() called");

        if (RespawnManager.Instance == null)
        {
            Debug.LogWarning("[PlayerRespawnHandler] No RespawnManager.Instance found. Cannot respawn.");
            return;
        }

        // Prefer nearest discovered point to where we died.
        SpawnPoint target = RespawnManager.Instance.GetBestRespawn(transform.position);

        // Fallback: whatever RespawnManager considers “current”.
        if (target == null)
            target = RespawnManager.Instance.GetCurrentRespawn();

        if (target == null)
        {
            Debug.LogWarning("[PlayerRespawnHandler] No valid respawn point found.");
            return;
        }

        RespawnAtPoint(target);
    }

    private void RespawnAtPoint(SpawnPoint point)
    {
        Transform t = point.transform;

        if (debugLogs)
            Debug.Log($"🚀 [PlayerRespawnHandler] Respawning at '{point.name}' (type={point.type}) pos={t.position}");

        // CharacterController teleport fix
        var cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        transform.SetPositionAndRotation(t.position, t.rotation);
        Physics.SyncTransforms();

        if (cc != null) cc.enabled = true;

        // Clear Rigidbody motion (if you have one)
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            if (debugLogs)
                Debug.Log("🧊 [PlayerRespawnHandler] Rigidbody velocity cleared");
        }
    }
}
