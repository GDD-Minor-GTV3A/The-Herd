using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using Core.AI.Sheep;

public class SheepPenController : MonoBehaviour
{
    [Header("Exit / Return Points")]
    public Transform exitPoint;
    public Transform penPoint; // where sheep should regroup inside

    private HashSet<SheepStateManager> sheepInPen = new();
    private bool playerInRange = false;
    private bool sheepOutside = false;
    private bool isMovingSheep = false;

    public bool PlayerInRange => playerInRange;
    public bool SheepOutside => sheepOutside;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sheep"))
        {
            var sheep = other.GetComponentInParent<SheepStateManager>();
            if (sheep != null)
            {
                sheepInPen.Add(sheep);
                Debug.Log($"[SheepPen] Sheep added: {sheep.name}");
            }
        }

        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("[SheepPen] Player entered pen range");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Sheep"))
        {
            var sheep = other.GetComponentInParent<SheepStateManager>();
            if (sheep != null)
            {
                sheepInPen.Remove(sheep);
                Debug.Log($"[SheepPen] Sheep removed: {sheep.name}");
            }
        }

        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("[SheepPen] Player left pen range");
        }
    }

    private void Update()
    {
        if (!playerInRange || isMovingSheep) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!sheepOutside)
            {
                sheepOutside = true;
                Debug.Log("[SheepPen] Releasing all sheep");
                StartCoroutine(MoveAllSheep(exitPoint.position));
            }
            else
            {
                sheepOutside = false;
                Debug.Log("[SheepPen] Calling back all sheep");
                StartCoroutine(MoveAllSheep(penPoint.position));
            }
        }
    }

    private IEnumerator MoveAllSheep(Vector3 target)
    {
        isMovingSheep = true;

        List<Coroutine> coroutines = new();

        // Move all sheep at once
        foreach (var sheep in sheepInPen)
        {
            if (sheep == null) continue;

            sheep.OnSheepUnfreeze();
            sheep.SetState<SheepFollowState>();
            coroutines.Add(StartCoroutine(MoveSheepToPoint(sheep, target)));
        }

        // Also include any sheep outside when calling back
        if (!sheepOutside)
        {
            var allSheep = GameObject.FindGameObjectsWithTag("Sheep");
            foreach (var obj in allSheep)
            {
                var sheep = obj.GetComponentInParent<SheepStateManager>();
                if (sheep == null) continue;
                if (sheepInPen.Contains(sheep)) continue; // skip ones already handled

                sheep.OnSheepUnfreeze();
                sheep.SetState<SheepFollowState>();
                coroutines.Add(StartCoroutine(MoveSheepToPoint(sheep, target)));
            }
        }

        // Wait for all sheep to finish moving
        foreach (var c in coroutines)
            yield return c;

        Debug.Log("[SheepPen] All sheep finished moving");
        isMovingSheep = false;
    }

    private IEnumerator MoveSheepToPoint(SheepStateManager sheep, Vector3 target)
    {
        if (sheep == null) yield break;

        NavMeshAgent agent = sheep.Agent;
        if (agent == null) yield break;

        agent.isStopped = false;
        agent.SetDestination(target);

        float timeout = 10f; // safety timeout
        float startTime = Time.time;

        while (true)
        {
            if (!agent.pathPending && agent.remainingDistance <= 0.3f)
                break;

            if (Time.time - startTime > timeout)
            {
                Debug.LogWarning($"[SheepPen] Sheep {sheep.name} stuck while moving to target!");
                break;
            }

            yield return null;
        }

        yield return new WaitForSeconds(0.2f);
        Debug.Log($"[SheepPen] Sheep {sheep.name} reached target");
    }
}
