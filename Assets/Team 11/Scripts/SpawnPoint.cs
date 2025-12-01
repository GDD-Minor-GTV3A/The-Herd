using UnityEngine;

public enum SpawnPointType
{
    LevelEntrance,   // used for scene transitions / entry points
    MemoryAnchor     // used as in-world checkpoints
}

public class SpawnPoint : MonoBehaviour
{
    [Header("Transition / Scene Entry")]
    [Tooltip("Must match the exitId from the previous scene (used by your level transition system).")]
    public string entryId;

    [Tooltip("Fallback spawn if no entryId matches.")]
    public bool isDefault = false;

    [Header("Respawn / Checkpoint")]
    [Tooltip("LevelEntrance = spawn after scene load. MemoryAnchor = in-world checkpoint.")]
    public SpawnPointType type = SpawnPointType.LevelEntrance;

    [Tooltip("Level entrances should usually be TRUE, memory anchors start FALSE until discovered.")]
    public bool discovered = false;

    private void Awake()
    {
        // For convenience in editor: level entrances are discovered by default
        if (type == SpawnPointType.LevelEntrance && !Application.isPlaying)
        {
            discovered = true;
        }
    }

    private void Start()
    {
        // Register this point with the RespawnManager if it exists in the scene.
        if (RespawnManager.Instance != null)
        {
            RespawnManager.Instance.RegisterPoint(this);
        }
        else
        {
            Debug.LogWarning($"[SpawnPoint] No RespawnManager present in this scene for '{name}'.");
        }
    }

    private void OnValidate()
    {
        // Whenever you switch to LevelEntrance in the inspector (in edit mode),
        // auto-mark it as discovered so it's a valid respawn from the start.
        if (type == SpawnPointType.LevelEntrance && !Application.isPlaying)
        {
            discovered = true;
        }
    }
}
