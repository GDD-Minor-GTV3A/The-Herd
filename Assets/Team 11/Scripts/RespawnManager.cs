using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance { get; private set; }

    [SerializeField]
    [Tooltip("If true, prints debug logs to the Console.")]
    private bool debugLogs = false;

    private readonly List<SpawnPoint> allPoints = new List<SpawnPoint>();
    private readonly List<SpawnPoint> discoveredPoints = new List<SpawnPoint>();

    private SpawnPoint currentEntrancePoint;
    private SpawnPoint currentRespawnPoint;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[RespawnManager] Multiple instances detected. Destroying the newest one.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void RegisterPoint(SpawnPoint point)
    {
        if (point == null) return;

        if (!allPoints.Contains(point))
        {
            allPoints.Add(point);

            if (point.discovered)
            {
                AddDiscoveredPoint(point);
            }

            if (debugLogs)
            {
                Debug.Log($"[RespawnManager] Registered point '{point.name}' ({point.type}).");
            }
        }
    }

    private void AddDiscoveredPoint(SpawnPoint point)
    {
        if (!discoveredPoints.Contains(point))
        {
            discoveredPoints.Add(point);
        }

        // Simple rule: last discovered becomes the current respawn
        currentRespawnPoint = point;

        if (debugLogs)
        {
            Debug.Log($"[RespawnManager] Discovered point '{point.name}'.");
        }
    }

    public void OnAnchorDiscovered(SpawnPoint point)
    {
        if (point == null) return;
        point.discovered = true;
        AddDiscoveredPoint(point);
    }

    public void SetEntrancePoint(SpawnPoint entrance)
    {
        if (entrance == null) return;

        currentEntrancePoint = entrance;

        entrance.discovered = true;
        AddDiscoveredPoint(entrance);

        if (currentRespawnPoint == null)
        {
            currentRespawnPoint = entrance;
        }

        if (debugLogs)
        {
            Debug.Log($"[RespawnManager] Entrance point set to '{entrance.name}'.");
        }
    }

    /// <summary>
    /// Returns the nearest discovered respawn point from a given position.
    /// If none are discovered, falls back to the current entrance point.
    /// </summary>
    public SpawnPoint GetBestRespawn(Vector3 fromPosition)
    {
        if (discoveredPoints.Count == 0)
        {
            if (debugLogs)
            {
                Debug.LogWarning("[RespawnManager] No discovered points. Falling back to entrance.");
            }
            return currentEntrancePoint;
        }

        SpawnPoint best = discoveredPoints[0];
        float bestDist = Vector3.SqrMagnitude(best.transform.position - fromPosition);

        for (int i = 1; i < discoveredPoints.Count; i++)
        {
            SpawnPoint candidate = discoveredPoints[i];
            float d = Vector3.SqrMagnitude(candidate.transform.position - fromPosition);
            if (d < bestDist)
            {
                best = candidate;
                bestDist = d;
            }
        }

        currentRespawnPoint = best;

        if (debugLogs)
        {
            Debug.Log($"[RespawnManager] Best respawn from {fromPosition} is '{best.name}'.");
        }

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
