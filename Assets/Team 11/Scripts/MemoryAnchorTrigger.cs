using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MemoryAnchorTrigger : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The SpawnPoint this trigger will mark as discovered (type should be MemoryAnchor).")]
    private SpawnPoint spawnPoint;

    [TextArea]
    [SerializeField]
    [Tooltip("Optional custom memory text to show when this anchor is discovered.")]
    private string memoryText;

    [SerializeField]
    private bool showDebugLog = true;

    private MemoryAnchorLocalUI localUI;

    private void Reset()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void Awake()
    {
        if (spawnPoint == null)
        {
            spawnPoint = GetComponentInParent<SpawnPoint>();
        }

        // look for local UI on the same anchor (parent)
        if (spawnPoint != null)
        {
            localUI = spawnPoint.GetComponentInChildren<MemoryAnchorLocalUI>(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (spawnPoint == null)
        {
            if (showDebugLog)
                Debug.LogWarning($"[MemoryAnchorTrigger] No SpawnPoint assigned on '{name}'.");
            return;
        }

        if (spawnPoint.discovered) return;

        spawnPoint.discovered = true;

        if (RespawnManager.Instance != null)
        {
            RespawnManager.Instance.OnAnchorDiscovered(spawnPoint);
        }

        if (showDebugLog)
        {
            Debug.Log($"[MemoryAnchorTrigger] Memory Anchor discovered: {spawnPoint.name}");
        }

        // Show local popup if it exists
        if (localUI != null)
        {
            localUI.Show(memoryText);
        }
    }
}
