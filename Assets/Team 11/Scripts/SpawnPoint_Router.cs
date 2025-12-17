using UnityEngine;
using System.Collections;

public class SpawnPointRouter : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    private IEnumerator Start()
    {
        yield return null;

        var player = GameObject.FindGameObjectWithTag(playerTag);
        if (!player) yield break;

        var points = FindObjectsOfType<SpawnPoint>(true);
        if (points == null || points.Length == 0) yield break;

        SpawnPoint target = null;

        var lastExitId = TransitionMemory.LastExitId;
        if (!string.IsNullOrEmpty(lastExitId))
        {
            foreach (var p in points)
            {
                if (p == null) continue;
                if (p.type != SpawnPointType.LevelEntrance) continue;

                if (string.Equals(p.entryId, lastExitId, System.StringComparison.Ordinal))
                {
                    target = p;
                    break;
                }
            }
        }

        if (target == null)
        {
            foreach (var p in points)
            {
                if (p == null) continue;
                if (p.type != SpawnPointType.LevelEntrance) continue;

                if (p.discovered)
                {
                    target = p;
                    break;
                }
            }
        }

        if (target == null)
        {
            foreach (var p in points)
            {
                if (p == null) continue;
                if (p.discovered)
                {
                    target = p;
                    break;
                }
            }
        }

        if (target == null)
            target = points[0];

        if (target == null) yield break;

        var cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.transform.SetPositionAndRotation(target.transform.position, target.transform.rotation);
        Physics.SyncTransforms();

        if (cc != null) cc.enabled = true;

        if (RespawnManager.Instance != null)
            RespawnManager.Instance.SetEntrancePoint(target);

        // ensure dog + same sheep instances end up near the player after a transition
        var prh = player.GetComponent<PlayerRespawnHandler>();
        if (prh != null)
            prh.RepositionFollowersNow();

        TransitionMemory.LastExitId = null;
    }
}
