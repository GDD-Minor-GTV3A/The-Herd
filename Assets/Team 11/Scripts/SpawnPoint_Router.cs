using UnityEngine;
using System.Collections;

public class SpawnPointRouter : MonoBehaviour
{
    [SerializeField] string playerTag = "Player";

    IEnumerator Start()
    {
        yield return null;
        var player = GameObject.FindGameObjectWithTag(playerTag);
        if (!player) yield break;

        var points = FindObjectsOfType<SpawnPoint>(true);
        SpawnPoint target = null;

        if (!string.IsNullOrEmpty(TransitionMemory.LastExitId))
            foreach (var p in points)
                if (p.entryId == TransitionMemory.LastExitId) { target = p; break; }

        if (!target)
            foreach (var p in points)
                if (p.isDefault) { target = p; break; }

        if (target)
        {
            player.transform.SetPositionAndRotation(target.transform.position, target.transform.rotation);
            TransitionMemory.LastExitId = null;
        }
    }
}
