using UnityEngine;
using UnityEngine.AI;

public class AmalgamationSpawnIntroState : IAmalgamationState
{
    private readonly AmalgamationStateMachine ctx;
    private readonly NavMeshAgent agent;

    private enum Phase
    {
        FollowPlayer,   // follow the player
        MoveToBox,      // then go straight to the box collider object
        WaitAtBox       // stop for a few seconds, then go to PATROL
    }

    private Phase phase;
    private float waitTimer;

    private readonly string logPrefix;

    public AmalgamationSpawnIntroState(AmalgamationStateMachine ctx, NavMeshAgent agent)
    {
        this.ctx = ctx;
        this.agent = agent;

        logPrefix = "[Amalgamation " + ctx.gameObject.name + "][SPAWN] ";
    }

    public void Enter()
    {
        if (agent == null || !agent.enabled)
        {
            DebugLog("Cannot ENTER spawn intro: missing or disabled NavMeshAgent.");
            return;
        }

        if (ctx.player == null)
        {
            DebugLog("No player reference; falling back to PATROL.");
            ctx.SwitchState(ctx.PatrolState);
            return;
        }

        if (ctx.spawnBoxTarget == null)
        {
            DebugLog("No spawnBoxTarget assigned; falling back to PATROL.");
            ctx.SwitchState(ctx.PatrolState);
            return;
        }

        ctx.anim?.PlayRunImmediate();

        agent.isStopped       = false;
        agent.updateRotation  = true;
        agent.speed           = ctx.spawnIntroSpeed;

        phase     = Phase.FollowPlayer;
        waitTimer = 0f;

        DebugLog("Entering SPAWN INTRO state. Phase = FollowPlayer.");
        SetDestinationToPlayer();
    }

    public void Tick()
    {
        if (agent == null || !agent.enabled)
            return;

        switch (phase)
        {
            case Phase.FollowPlayer:
                TickFollowPlayer();
                break;
            case Phase.MoveToBox:
                TickMoveToBox();
                break;
            case Phase.WaitAtBox:
                TickWaitAtBox();
                break;
        }
    }

    public void Exit()
    {
        DebugLog("Exiting SPAWN INTRO state.");
        // Patrol/Chase states will reconfigure agent.isStopped / updateRotation as needed.
    }

    // ---------------------------------------------------------
    //  PHASE 1 – FOLLOW THE PLAYER
    // ---------------------------------------------------------

    private void TickFollowPlayer()
    {
        if (ctx.player == null || ctx.spawnBoxTarget == null)
        {
            DebugLog("Lost references in FollowPlayer; switching to PATROL.");
            ctx.SwitchState(ctx.PatrolState);
            return;
        }

        agent.speed = ctx.spawnIntroSpeed;

        SetDestinationToPlayer();
        RotateTowards(ctx.player.position);

        // Distance from enemy to the box collider object
        float distToBox = Vector3.Distance(agent.transform.position, ctx.spawnBoxTarget.position);

        if (distToBox <= ctx.spawnBoxTriggerRadius)
        {
            phase = Phase.MoveToBox;

            DebugLog($"Switching to MoveToBox phase (dist to box = {distToBox:F2}, " +
                     $"triggerRadius = {ctx.spawnBoxTriggerRadius:F2}).");

            SetDestinationToBox();
        }
    }

    // ---------------------------------------------------------
    //  PHASE 2 – MOVE TO BOX COLLIDER OBJECT
    // ---------------------------------------------------------

    private void TickMoveToBox()
    {
        if (ctx.spawnBoxTarget == null)
        {
            DebugLog("No spawnBoxTarget in MoveToBox; switching to PATROL.");
            ctx.SwitchState(ctx.PatrolState);
            return;
        }

        agent.speed = ctx.spawnIntroSpeed;

        SetDestinationToBox();
        RotateTowards(ctx.spawnBoxTarget.position);

        // When we’re close enough, stop and wait
        if (!agent.pathPending && agent.remainingDistance <= ctx.spawnBoxArriveDistance)
        {
            phase      = Phase.WaitAtBox;
            waitTimer  = ctx.spawnWaitAtBoxTime;

            agent.isStopped = true;
            agent.ResetPath();

            ctx.anim?.PlayIdleImmediate();

            DebugLog($"Reached box target. Waiting for {waitTimer:F2} seconds before switching to PATROL.");
        }
    }

    // ---------------------------------------------------------
    //  PHASE 3 – WAIT, THEN HAND OVER TO PATROL
    // ---------------------------------------------------------

    private void TickWaitAtBox()
    {
        waitTimer -= Time.deltaTime;
        if (waitTimer <= 0f)
        {
            DebugLog("Wait time finished. Switching to PATROL.");
            ctx.SwitchState(ctx.PatrolState);
        }
    }

    // ---------------------------------------------------------
    //  HELPERS
    // ---------------------------------------------------------

    private void SetDestinationToPlayer()
    {
        if (ctx.player == null) return;
        if (!agent.pathPending)
        {
            agent.SetDestination(ctx.player.position);
        }
    }

    private void SetDestinationToBox()
    {
        if (ctx.spawnBoxTarget == null) return;
        if (!agent.pathPending)
        {
            agent.SetDestination(ctx.spawnBoxTarget.position);
        }
    }

    private void RotateTowards(Vector3 worldTarget)
    {
        Vector3 dir = worldTarget - agent.transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.0001f)
            return;

        // Re-use chaseRotationSpeed for nice turning speed
        Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
        agent.transform.rotation = Quaternion.Slerp(
            agent.transform.rotation,
            targetRot,
            ctx.chaseRotationSpeed * Time.deltaTime
        );
    }

    private void DebugLog(string msg)
    {
        if (!ctx.debugLogs) return;
        Debug.Log(logPrefix + msg);
    }
}
