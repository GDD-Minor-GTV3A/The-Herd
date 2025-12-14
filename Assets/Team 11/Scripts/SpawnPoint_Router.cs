using UnityEngine;
using System.Collections;

public class SpawnPointRouter : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    private IEnumerator Start()
    {
        // Wait a frame so RespawnManager + SpawnPoints Awake/OnEnable have run.
        yield return null;

        var player = GameObject.FindGameObjectWithTag(playerTag);
        if (!player) yield break;

        var points = FindObjectsOfType<SpawnPoint>(true);

        string desiredEntryId = TransitionMemory.LastExitId;
        SpawnPoint target = null;

        // 1) Exact match by entryId among LevelEntrances
        if (!string.IsNullOrEmpty(desiredEntryId))
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i] != null &&
                    points[i].type == SpawnPointType.LevelEntrance &&
                    points[i].entryId == desiredEntryId)
                {
                    target = points[i];
                    break;
                }
            }
        }

        // 2) Default entrance
        if (target == null)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i] != null &&
                    points[i].type == SpawnPointType.LevelEntrance &&
                    points[i].isDefault)
                {
                    target = points[i];
                    break;
                }
            }
        }

        // 3) Any entrance
        if (target == null)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i] != null && points[i].type == SpawnPointType.LevelEntrance)
                {
                    target = points[i];
                    break;
                }
            }
        }

        if (target == null)
        {
            Debug.LogWarning("[SpawnPointRouter] No SpawnPoint found to place player.");
            yield break;
        }

        // CharacterController teleport fix
        var cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.transform.SetPositionAndRotation(target.transform.position, target.transform.rotation);
        Physics.SyncTransforms();

        if (cc != null) cc.enabled = true;

        if (RespawnManager.Instance != null)
            RespawnManager.Instance.SetEntrancePoint(target);

        TransitionMemory.LastExitId = null;
    }
}
