using UnityEngine;
using UnityEngine.AI;

public class AmalgamationSpawnIntroState : IAmalgamationState
{
    private readonly AmalgamationStateMachine sm;
    private readonly NavMeshAgent agent;

    private bool waiting;          // are we currently in the "wait at box" phase?
    private float waitRemaining;   // countdown timer

    public AmalgamationSpawnIntroState(AmalgamationStateMachine stateMachine, NavMeshAgent navAgent)
    {
        sm = stateMachine;
        agent = navAgent;
    }

    public void Enter()
    {
        // Reset the trigger flag so this intro works cleanly every time
        sm.spawnIntroTriggerHit = false;

        waiting = false;
        waitRemaining = sm.spawnWaitAtBoxTime;

        // Configure movement for intro-follow
        agent.isStopped = false;
        agent.speed = sm.spawnIntroSpeed;
        agent.updateRotation = true;

        DebugLog("Entering SPAWN INTRO state. Following player until trigger is entered.");
    }

    public void Exit()
    {
        // Nothing required, but we can stop spamming movement updates if you want
        // agent.ResetPath();
    }

    public void Tick()
    {
        if (sm.player == null)
            return;

        // Phase 1: follow player until the intro trigger is hit
        if (!sm.spawnIntroTriggerHit && !waiting)
        {
            FollowPlayer();
            return;
        }

        // Phase 2: trigger hit -> wait once -> then switch to patrol
        if (!waiting)
        {
            waiting = true;
            waitRemaining = sm.spawnWaitAtBoxTime;

            // Stop moving while waiting at the box/trigger area
            agent.isStopped = true;
            agent.ResetPath();

            DebugLog($"Trigger entered. Waiting for {sm.spawnWaitAtBoxTime:0.00} seconds before switching to PATROL.");
        }

        // Count down (NOTE: if Time.timeScale == 0, this will never finish)
        waitRemaining -= Time.deltaTime;

        if (waitRemaining <= 0f)
        {
            DebugLog("Wait finished. Switching to PATROL.");
            sm.SwitchState(sm.PatrolState);
        }
    }

    private void FollowPlayer()
    {
        // Keep pushing destination toward player
        // (If your player is moving, this makes the enemy keep tracking.)
        if (!agent.isOnNavMesh)
            return;

        agent.isStopped = false;

        // Optional: you can stop within some radius instead of face-hugging
        // float stopRadius = 1.5f;
        // if (Vector3.Distance(agent.transform.position, sm.player.position) <= stopRadius) { agent.isStopped = true; return; }

        agent.SetDestination(sm.player.position);
    }

    public void OnTriggerEnter(Collider other)
    {
        // Not used in your current architecture because the StateMachine handles OnTriggerEnter,
        // but included to satisfy interface consistency.
    }

    public void OnTriggerExit(Collider other)
    {
        // Not needed.
    }

    private void DebugLog(string msg)
    {
        if (!sm.debugLogs) return;
        Debug.Log($"[Amalgamation {sm.gameObject.name}][SPAWN] {msg}");
    }
}
