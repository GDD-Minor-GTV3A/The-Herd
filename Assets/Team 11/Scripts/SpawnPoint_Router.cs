using UnityEngine;
using System.Collections;

public class SpawnPointRouter : MonoBehaviour
{
    [SerializeField] string playerTag = "Player";

    IEnumerator Start()
    {
        // Wait one frame so all SpawnPoints have registered
        yield return null;

        var player = GameObject.FindGameObjectWithTag(playerTag);
        if (!player) yield break;

        // Get all SpawnPoints in the scene (entrances + anchors)
        var points = FindObjectsOfType<SpawnPoint>(true);

        // We only care about LevelEntrance spawn points for scene entry
        SpawnPoint target = null;
        string desiredEntryId = TransitionMemory.LastExitId;

        Debug.Log($"[SpawnPointRouter] Desired entryId from TransitionMemory: '{desiredEntryId}'");

        // 1) Try to find a LevelEntrance with matching entryId from previous scene
        if (!string.IsNullOrEmpty(desiredEntryId))
        {
            foreach (var p in points)
            {
                if (p.type == SpawnPointType.LevelEntrance && p.entryId == desiredEntryId)
                {
                    target = p;
                    Debug.Log($"[SpawnPointRouter] Found matching LevelEntrance: '{p.name}' for entryId '{desiredEntryId}'.");
                    break;
                }
            }
        }

        // 2) If nothing matched, fall back to any LevelEntrance marked as default
        if (target == null)
        {
            foreach (var p in points)
            {
                if (p.type == SpawnPointType.LevelEntrance && p.isDefault)
                {
                    target = p;
                    Debug.Log($"[SpawnPointRouter] Using default LevelEntrance: '{p.name}'.");
                    break;
                }
            }
        }

        // 3) If still nothing, just take the first LevelEntrance we find
        if (target == null)
        {
            foreach (var p in points)
            {
                if (p.type == SpawnPointType.LevelEntrance)
                {
                    target = p;
                    Debug.LogWarning($"[SpawnPointRouter] No matching/default entry. Using first LevelEntrance: '{p.name}'.");
                    break;
                }
            }
        }

        if (target == null)
        {
            Debug.LogError("[SpawnPointRouter] No suitable LevelEntrance SpawnPoint found. Player not moved.");
            yield break;
        }

        // Move the player to the chosen spawn
        player.transform.SetPositionAndRotation(target.transform.position, target.transform.rotation);

        // Tell RespawnManager this is the entrance for respawn logic
        if (RespawnManager.Instance != null)
        {
            RespawnManager.Instance.SetEntrancePoint(target);
        }

        // Clear the memory so we don't reuse it accidentally
        TransitionMemory.LastExitId = null;
    }
}
