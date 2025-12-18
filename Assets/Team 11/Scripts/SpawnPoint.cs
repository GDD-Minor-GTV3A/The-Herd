using UnityEngine;

public enum SpawnPointType
{
    LevelEntrance,   // used for scene transitions / entry points
    MemoryAnchor     // used as in-world checkpoints
}

public class SpawnPoint : MonoBehaviour
{
    [Header("Type")]
    public SpawnPointType type = SpawnPointType.MemoryAnchor;

    [Header("Transition / Scene Entry")]
    [Tooltip("Must match the exitId from the previous scene (used by your level transition system).")]
    public string entryId;

    [Tooltip("Fallback spawn if no entryId matches.")]
    public bool isDefault = false;

    [Header("Respawn")]
    [Tooltip("If true, this point is eligible for respawn selection.")]
    public bool discovered = false;

    private void Awake()
    {
        // Register as early as possible so respawn works even if player dies immediately on load.
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.RegisterPoint(this);
        else
            Debug.LogWarning($"[SpawnPoint] No RespawnManager present in this scene for '{name}'.");
    }

    private void OnEnable()
    {
        // In case objects get enabled later
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.RegisterPoint(this);
    }

    private void OnDisable()
    {
        if (RespawnManager.Instance != null)
            RespawnManager.Instance.UnregisterPoint(this);
    }

    private void OnValidate()
    {
        // Level entrances should be valid from the start
        if (type == SpawnPointType.LevelEntrance && !Application.isPlaying)
            discovered = true;
    }
}
