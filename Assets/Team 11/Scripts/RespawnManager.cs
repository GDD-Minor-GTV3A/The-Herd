using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance { get; private set; }

    [SerializeField, Tooltip("If true, prints debug logs to the Console.")]
    private bool debugLogs = false;

    private readonly List<SpawnPoint> allPoints = new List<SpawnPoint>();
    private readonly List<SpawnPoint> discoveredPoints = new List<SpawnPoint>();

    private SpawnPoint currentEntrancePoint;
    private SpawnPoint currentRespawnPoint;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void RegisterPoint(SpawnPoint p)
    {
        if (p == null) return;

        if (!allPoints.Contains(p))
            allPoints.Add(p);

        if (p.discovered && !discoveredPoints.Contains(p))
            discoveredPoints.Add(p);
    }

    public void UnregisterPoint(SpawnPoint p)
    {
        if (p == null) return;

        allPoints.Remove(p);
        discoveredPoints.Remove(p);

        if (currentEntrancePoint == p) currentEntrancePoint = null;
        if (currentRespawnPoint == p) currentRespawnPoint = null;
    }

    public void SetEntrancePoint(SpawnPoint entrance)
    {
        if (entrance == null) return;

        currentEntrancePoint = entrance;

        // entrances should always be valid
        entrance.discovered = true;
        if (!discoveredPoints.Contains(entrance))
            discoveredPoints.Add(entrance);

        // if we don't have any respawn yet, use entrance
        if (currentRespawnPoint == null)
            currentRespawnPoint = entrance;

        if (debugLogs)
            Debug.Log($"[RespawnManager] Entrance set to '{entrance.name}' (entryId='{entrance.entryId}')");
    }

    public void OnAnchorDiscovered(SpawnPoint anchor)
    {
        if (anchor == null) return;

        anchor.discovered = true;

        if (!discoveredPoints.Contains(anchor))
            discoveredPoints.Add(anchor);

        currentRespawnPoint = anchor;

        if (debugLogs)
            Debug.Log($"[RespawnManager] Anchor discovered -> current respawn = '{anchor.name}'");
    }

    /// <summary>
    /// Chooses the closest discovered SpawnPoint to a given position.
    /// </summary>
    public SpawnPoint GetBestRespawn(Vector3 fromPosition)
    {
        SpawnPoint best = null;
        float bestDist = float.MaxValue;

        // Prefer discovered points
        for (int i = 0; i < discoveredPoints.Count; i++)
        {
            var p = discoveredPoints[i];
            if (p == null) continue;

            float d = Vector3.Distance(fromPosition, p.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = p;
            }
        }

        // Fallbacks
        if (best == null && currentEntrancePoint != null)
            best = currentEntrancePoint;

        if (best == null)
        {
            // Try default spawn
            for (int i = 0; i < allPoints.Count; i++)
            {
                var p = allPoints[i];
                if (p != null && p.isDefault)
                {
                    best = p;
                    break;
                }
            }
        }

        if (debugLogs && best != null)
            Debug.Log($"[RespawnManager] Best respawn from {fromPosition} is '{best.name}'");

        return best;
    }

    /// <summary>
    /// Returns the currently tracked respawn point (last discovered or entrance).
    /// </summary>
    public SpawnPoint GetCurrentRespawn()
    {
        if (currentRespawnPoint != null) return currentRespawnPoint;
        return currentEntrancePoint;
    }
}
