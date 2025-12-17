using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Core.AI.Sheep;
using Gameplay.Dog;

public class PlayerRespawnHandler : MonoBehaviour
{
    [Header("Debug Logs")]
    [SerializeField] private bool debugLogs = true;

    [Header("Follower Repositioning")]
    [Tooltip("How far from the player sheep will be placed after respawn/scene transition.")]
    [SerializeField] private float sheepSpawnRadius = 2.5f;

    [Tooltip("How far to the side sheep can spread (in degrees) behind the player.")]
    [Range(10f, 180f)]
    [SerializeField] private float sheepArcDegrees = 120f;

    [Tooltip("How far from the player the dog will be placed after respawn/scene transition.")]
    [SerializeField] private float dogDistanceFromPlayer = 1.5f;

    public void HandleDeath()
    {
        if (debugLogs)
            Debug.Log("☠️ [PlayerRespawnHandler] HandleDeath() called");

        if (RespawnManager.Instance == null)
        {
            Debug.LogWarning("[PlayerRespawnHandler] No RespawnManager.Instance found. Cannot respawn.");
            return;
        }

        // Prefer nearest discovered point to where we died.
        SpawnPoint target = RespawnManager.Instance.GetBestRespawn(transform.position);

        // Fallback: whatever RespawnManager considers “current”.
        if (target == null)
            target = RespawnManager.Instance.GetCurrentRespawn();

        if (target == null)
        {
            Debug.LogWarning("[PlayerRespawnHandler] No valid respawn point found.");
            return;
        }

        RespawnAtPoint(target);
    }

    private void RespawnAtPoint(SpawnPoint point)
    {
        Transform t = point.transform;

        if (debugLogs)
            Debug.Log($"🚀 [PlayerRespawnHandler] Respawning at '{point.name}' (type={point.type}) pos={t.position}");

        // CharacterController teleport fix
        var cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        transform.SetPositionAndRotation(t.position, t.rotation);
        Physics.SyncTransforms();

        if (cc != null) cc.enabled = true;

        // Clear Rigidbody motion (if you have one)
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            if (debugLogs)
                Debug.Log("🧊 [PlayerRespawnHandler] Rigidbody velocity cleared");
        }

        // Move dog + SAME sheep instances near player after any respawn.
        RepositionFollowersNow();
    }

    public void RepositionFollowersNow()
    {
        RepositionDogNearPlayer();
        RepositionAliveSheepNearPlayer();
    }

    private void RepositionDogNearPlayer()
    {
        // Using FindObjectOfType for max compatibility (yes it warns, but it compiles everywhere)
        var dog = Object.FindObjectOfType<DogStateManager>();
        if (dog == null) return;

        Vector3 targetPos = transform.position + (-transform.forward * dogDistanceFromPlayer);
        targetPos.y = transform.position.y;

        var agent = dog.GetComponent<NavMeshAgent>();
        if (agent != null && agent.enabled)
        {
            if (NavMesh.SamplePosition(targetPos, out var hit, 2.0f, NavMesh.AllAreas))
                targetPos = hit.position;

            agent.Warp(targetPos);
        }
        else
        {
            dog.transform.position = targetPos;
            Physics.SyncTransforms();
        }

        if (debugLogs)
            Debug.Log($"🐶 [PlayerRespawnHandler] Dog moved near player: {targetPos}");
    }

    private void RepositionAliveSheepNearPlayer()
    {
        IReadOnlyList<SheepStateManager> sheepList = null;

        if (SheepTracker.Instance != null)
            sheepList = SheepTracker.Instance.GetOrderedSheepList();

        if (sheepList == null || sheepList.Count == 0)
            sheepList = Object.FindObjectsByType<SheepStateManager>(FindObjectsSortMode.None);

        if (sheepList == null || sheepList.Count == 0) return;

        int count = sheepList.Count;
        float halfArc = Mathf.Clamp(sheepArcDegrees, 10f, 180f) * 0.5f;

        for (int i = 0; i < count; i++)
        {
            var sheep = sheepList[i];
            if (sheep == null || !sheep.isActiveAndEnabled) continue;

            float tt = (count == 1) ? 0.5f : (i / (float)(count - 1));
            float angle = Mathf.Lerp(-halfArc, halfArc, tt);

            Vector3 dir = Quaternion.Euler(0f, angle, 0f) * (-transform.forward);
            Vector3 targetPos = transform.position + (dir.normalized * sheepSpawnRadius);

            if (NavMesh.SamplePosition(targetPos, out var hit, 2.5f, NavMesh.AllAreas))
                targetPos = hit.position;

            // Minimal dependency: uses the tiny helper you added to SheepStateManager
            sheep.WarpTo(targetPos);
        }

        if (debugLogs)
            Debug.Log($"🐑 [PlayerRespawnHandler] Repositioned {count} sheep near player.");
    }
}
