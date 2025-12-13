using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Core.AI.Sheep;
using System.Collections.Generic;

public class SheepPenController : MonoBehaviour
{
    [Header("Pen Behavior")]
    [SerializeField] private Transform penPoint;
    [SerializeField] private float penRadius = 5f;
    [SerializeField] private float sheepStopDistance = 0.5f;

    [Header("Interaction")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private bool playerInRange = false;
    private bool sheepOutside = false;
    private bool isProcessing = false;

    public bool PlayerInRange => playerInRange;
    public bool SheepOutside => sheepOutside;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("[SheepPen] Player entered pen range");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("[SheepPen] Player left pen range");
        }
    }

    private void Update()
    {
        if (!playerInRange || isProcessing) return;

        if (Input.GetKeyDown(interactKey))
        {
            if (!sheepOutside)
            {
                sheepOutside = true;
                Debug.Log("[SheepPen] Player requested: STORE ALL SHEEP");
                StartCoroutine(SendAllSheepToPen());
            }
            else
            {
                sheepOutside = false;
                Debug.Log("[SheepPen] Player requested: RELEASE ALL SHEEP");
                ReleaseAllSheep();
            }
        }
    }

    // Send all sheep to random points inside the pen area
    private IEnumerator SendAllSheepToPen()
    {
        if (penPoint == null)
        {
            Debug.LogError("[SheepPen] penPoint is not assigned!");
            yield break;
        }

        isProcessing = true;

        var all = SheepStateManager.AllSheep;
        Debug.Log($"[SheepPen] Sending {all.Count} sheep to pen (radius={penRadius})");

        foreach (var sheep in all)
        {
            if (sheep == null) continue;

            // ensure the agent can be controlled / is on NavMesh
            NavMeshAgent agent = sheep.Agent;
            if (agent == null)
            {
                Debug.LogWarning($"[SheepPen] {sheep.name} has no NavMeshAgent!");
                continue;
            }

            if (!sheep.CanControlAgent() || !agent.isOnNavMesh)
            {
                Debug.Log($"[SheepPen] {sheep.name} agent not ready (isOnNavMesh={agent.isOnNavMesh}, enabled={agent.enabled}). Attempting to fix...");

                // try to sample a nearby NavMesh position and warp agent there
                NavMeshHit hit;
                float sampleRadius = 5f;
                if (NavMesh.SamplePosition(sheep.transform.position, out hit, sampleRadius, NavMesh.AllAreas))
                {
                    agent.Warp(hit.position); // warp onto navmesh
                    Debug.Log($"[SheepPen] Warped {sheep.name} to nearest NavMesh at {hit.position}");
                }
                else
                {
                    Debug.LogWarning($"[SheepPen] Failed to find NavMesh near {sheep.name} within {sampleRadius}m. Skipping sheep.");
                    continue;
                }
            }

            // pick random offset inside pen circle
            Vector2 offset = Random.insideUnitCircle * penRadius;
            Vector3 target = penPoint.position + new Vector3(offset.x, 0f, offset.y);

            Debug.Log($"[SheepPen] Commanding {sheep.name} -> MoveToPoint({target}, {sheepStopDistance})");

            // call Team2 API
            sheep.MoveToPoint(target, sheepStopDistance);

            // debug: show agent destination and path status (may be set by the Move state)
            if (agent.isOnNavMesh)
            {
                // if MoveToPoint immediately sets the agent destination via state, it may appear in agent.destination
                Debug.Log($"[SheepPen] {sheep.name} agent.isOnNavMesh=true, agent.destination={agent.destination}, pathStatus={agent.pathStatus}");
            }

            // small stagger to avoid performance spikes and give sheep time to register state
            yield return null;
        }

        Debug.Log("[SheepPen] SendAllSheepToPen finished issuing commands.");
        isProcessing = false;
    }

    // Call all sheep back to the herd
    private void ReleaseAllSheep()
    {
        var all = SheepStateManager.AllSheep;
        Debug.Log($"[SheepPen] Releasing {all.Count} sheep to herd.");

        foreach (var sheep in all)
        {
            if (sheep == null) continue;

            // Clear any temporary flags and summon to herd (Team2 API)
            Debug.Log($"[SheepPen] Calling SummonToHerd on {sheep.name}");
            sheep.SummonToHerd(3f, clearThreats: true);

            // SummonToHerd already calls SetState<SheepFollowState>() internally,
            // so no need to force a state change here.
        }
    }
}
