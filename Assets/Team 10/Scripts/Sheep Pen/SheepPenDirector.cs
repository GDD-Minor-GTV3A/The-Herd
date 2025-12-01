using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;
using Core.AI.Sheep;

public class SheepPenDirector : MonoBehaviour
{
    [Header("Sheep to Release")]
    public List<SheepStateManager> sheepInPen = new();

    [Header("Path")]
    public Transform[] waypoints;

    [Header("Settings")]
    public float releaseInterval = 1.0f;
    public float waypointArrivalThreshold = 0.4f;

    private bool _running = false;

    private void Start()
    {
        // Freeze all sheep at start
        foreach (var s in sheepInPen)
        {
            if (s != null) s.SetState<SheepFreezeState>();
        }

        StartCoroutine(ReleaseFlow());
    }

    private IEnumerator ReleaseFlow()
    {
        if (_running) yield break;
        _running = true;

        foreach (var sheep in sheepInPen)
        {
            if (sheep == null) continue;

            // Unfreeze so we can control them
            sheep.OnSheepUnfreeze();

            yield return StartCoroutine(SendSheepAlongPath(sheep));

            // Final handoff: let normal AI resume grazing/following
            sheep.SetState<SheepGrazeState>();

            yield return new WaitForSeconds(releaseInterval);
        }
    }

    private IEnumerator SendSheepAlongPath(SheepStateManager sheep)
    {
        NavMeshAgent agent = sheep.Agent;

        foreach (var wp in waypoints)
        {
            if (wp == null) continue;

            agent.isStopped = false;
            agent.SetDestination(wp.position);

            // wait to reach this waypoint
            while (true)
            {
                if (!agent.pathPending &&
                    agent.remainingDistance <= waypointArrivalThreshold)
                {
                    break;
                }
                yield return null;
            }

            // small pause to look natural
            yield return new WaitForSeconds(0.2f);
        }
    }
}
