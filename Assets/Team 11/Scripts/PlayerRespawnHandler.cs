using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Core.AI.Sheep;
using Gameplay.Dog;

public class PlayerRespawnHandler : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool debugLogs = true;

    [Header("Sheep Placement")]
    [SerializeField] private float sheepSpawnRadius = 2.5f;
    [Range(10f, 180f)]
    [SerializeField] private float sheepArcDegrees = 120f;

    [Header("Dog Placement")]
    [SerializeField] private float dogDistanceFromPlayer = 1.5f;

    /* =========================
     * DEATH ENTRY POINT
     * ========================= */
    public void HandleDeath()
    {
        if (RespawnManager.Instance == null)
        {
            Debug.LogWarning("[PlayerRespawnHandler] No RespawnManager found.");
            return;
        }

        SpawnPoint target =
            RespawnManager.Instance.GetBestRespawn(transform.position)
            ?? RespawnManager.Instance.GetCurrentRespawn();

        if (target == null)
        {
            Debug.LogWarning("[PlayerRespawnHandler] No valid respawn point.");
            return;
        }

        RespawnAtPoint(target);
    }

    /* =========================
     * CORE RESPAWN
     * ========================= */
    private void RespawnAtPoint(SpawnPoint point)
    {
        Transform t = point.transform;

        // Safely teleport player
        var cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        transform.SetPositionAndRotation(t.position, t.rotation);
        Physics.SyncTransforms();

        if (cc != null) cc.enabled = true;

        // Clear motion if Rigidbody exists
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Move followers AFTER player is placed
        RepositionFollowersNow();
    }

    /* =========================
     * FOLLOWER REPOSITION
     * ========================= */
    public void RepositionFollowersNow()
    {
        RepositionDog();
        RepositionOwnedSheep();
    }

    /* ---------- DOG ---------- */
    private void RepositionDog()
    {
        var dog = Object.FindObjectOfType<DogStateManager>();
        if (dog == null) return;

        Vector3 targetPos =
            transform.position - transform.forward * dogDistanceFromPlayer;
        targetPos.y = transform.position.y;

        var agent = dog.GetComponent<NavMeshAgent>();
        if (agent != null && agent.enabled &&
            NavMesh.SamplePosition(targetPos, out var hit, 2f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }
        else
        {
            dog.transform.position = targetPos;
            Physics.SyncTransforms();
        }

        if (debugLogs)
            Debug.Log("🐶 Dog repositioned near player.");
    }

    /* ---------- SHEEP ---------- */
    private void RepositionOwnedSheep()
    {
        if (SheepTracker.Instance == null) return;

        IReadOnlyList<SheepStateManager> sheep =
            SheepTracker.Instance.GetOrderedSheepList();

        // 🔒 CRITICAL SAFETY CHECK
        // If player had no sheep, we move NONE.
        if (sheep == null || sheep.Count == 0) return;

        int count = sheep.Count;
        float halfArc = Mathf.Clamp(sheepArcDegrees, 10f, 180f) * 0.5f;

        for (int i = 0; i < count; i++)
        {
            var s = sheep[i];
            if (s == null || !s.isActiveAndEnabled) continue;

            float t = (count == 1) ? 0.5f : i / (float)(count - 1);
            float angle = Mathf.Lerp(-halfArc, halfArc, t);

            Vector3 dir =
                Quaternion.Euler(0f, angle, 0f) * (-transform.forward);
            Vector3 targetPos =
                transform.position + dir.normalized * sheepSpawnRadius;

            if (NavMesh.SamplePosition(targetPos, out var hit, 2.5f, NavMesh.AllAreas))
                targetPos = hit.position;

            // Uses your EXISTING helper — no AI reset
            s.WarpTo(targetPos);
        }

        if (debugLogs)
            Debug.Log($"🐑 Repositioned {count} owned sheep.");
    }
}
