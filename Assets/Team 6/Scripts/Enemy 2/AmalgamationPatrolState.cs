using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AmalgamationPatrolState : IAmalgamationState
{
    private readonly AmalgamationStateMachine ctx;
    private readonly NavMeshAgent agent;
    private readonly Transform player;
    private readonly List<Transform> nodes;

    private Transform currentTarget;

    // Timer for periodic re-pathing (optional)
    private float nextChangeTime;

    // Timer for suspicious 180° turn checks
    private float nextSuspiciousTurnTime;

    // Suspicious turn state
    private bool isSuspiciousTurning = false;
    private Quaternion suspiciousStartRot;
    private Quaternion suspiciousTargetRot;
    private float suspiciousTurnTimer = 0f;

    // For nicer debug messages
    private readonly string logPrefix;

    public AmalgamationPatrolState(
        AmalgamationStateMachine ctx,
        NavMeshAgent agent,
        Transform player,
        List<Transform> nodes)
    {
        this.ctx = ctx;
        this.agent = agent;
        this.player = player;
        this.nodes = nodes;

        logPrefix = "[Amalgamation " + ctx.gameObject.name + "] ";
    }

    public void Enter()
    {
        if (nodes == null || nodes.Count == 0)
        {
            DebugLog("NO patrol nodes set on AmalgamationStateMachine.");
            return;
        }

        ctx.anim?.PlayRunImmediate();

        // Let NavMeshAgent handle rotation in patrol
        agent.updateRotation = true;
        agent.isStopped = false;

        ScheduleNextChangeTime();
        ScheduleNextSuspiciousTurnTime();
        DebugLog("Entering PATROL state.");

        // If chase told us to start at a specific node, do that once
        if (ctx.forcedFirstPatrolNode != null)
        {
            currentTarget = ctx.forcedFirstPatrolNode;
            ctx.forcedFirstPatrolNode = null;

            if (agent.SetDestination(currentTarget.position))
            {
                DebugLog($"Forced first patrol node: '{NameOrNull(currentTarget)}' at {currentTarget.position}.");
            }
            else
            {
                DebugLog($"SetDestination FAILED for forced node '{NameOrNull(currentTarget)}'. Falling back to normal PickNextNode().");
                PickNextNode();
            }
        }
        else
        {
            PickNextNode();
        }
    }

    public void Tick()
    {
        if (agent == null || !agent.enabled)
            return;

        // If we are currently performing a slow suspicious turn,
        // handle rotation and skip normal movement logic.
        if (isSuspiciousTurning)
        {
            UpdateSuspiciousTurnRotation();
            return;
        }

        UpdateSpeedBasedOnDistanceToPlayer();

        // If reached current node, pick a new one
        if (!agent.pathPending && agent.remainingDistance <= ctx.nodeReachThreshold)
        {
            DebugLog($"Reached node '{NameOrNull(currentTarget)}' (remainingDistance = {agent.remainingDistance:F2}). Picking next node.");
            PickNextNode();
        }

        // Optionally, periodically change destination even if not reached
        if (ctx.periodicallyChangeTarget && Time.time >= nextChangeTime)
        {
            DebugLog($"Periodic re-path trigger. Current node = '{NameOrNull(currentTarget)}'. Picking next node.");
            PickNextNode();
            ScheduleNextChangeTime();
        }

        // Suspicious 180° turns when player meets conditions
        if (ctx.suspiciousTurnsEnabled && Time.time >= nextSuspiciousTurnTime)
        {
            TrySuspiciousTurn();
            ScheduleNextSuspiciousTurnTime();
        }
    }

    public void Exit()
    {
        DebugLog("Exiting PATROL state.");
        // agent.ResetPath(); // optional
    }

    // ===========================
    //  PATROL / MOVEMENT
    // ===========================

    private void ScheduleNextChangeTime()
    {
        if (!ctx.periodicallyChangeTarget) return;

        float interval = Random.Range(ctx.minChangeInterval, ctx.maxChangeInterval);
        nextChangeTime = Time.time + interval;
        DebugLog($"Next periodic re-path scheduled in {interval:F2} seconds (at t={nextChangeTime:F2}).");
    }

    private void ScheduleNextSuspiciousTurnTime()
    {
        if (!ctx.suspiciousTurnsEnabled) return;

        float interval = Random.Range(ctx.minSuspiciousTurnInterval, ctx.maxSuspiciousTurnInterval);
        nextSuspiciousTurnTime = Time.time + interval;
        DebugLog($"Next suspicious turn check scheduled in {interval:F2} seconds (at t={nextSuspiciousTurnTime:F2}).");
    }

    private void UpdateSpeedBasedOnDistanceToPlayer()
    {
        if (player == null) return;

        float dist = Vector3.Distance(agent.transform.position, player.position);

        float oldSpeed = agent.speed;
        agent.speed = dist > ctx.closeEnoughRadius ? ctx.fastSpeed : ctx.normalSpeed;

        if (ctx.debugLogs && Mathf.Abs(agent.speed - oldSpeed) > 0.01f)
        {
            DebugLog($"Speed changed to {agent.speed:F2} (distance to player = {dist:F2}).");
        }
    }

    private void PickNextNode()
    {
        if (nodes == null || nodes.Count == 0) return;

        bool tryIntercept = Random.value < ctx.interceptChance;
        DebugLog($"PickNextNode called. tryIntercept = {tryIntercept}, interceptChance = {ctx.interceptChance:F2}.");

        Transform next = null;

        if (tryIntercept && player != null)
        {
            next = FindBestInterceptNode(out float bestPathDist);

            if (next != null)
            {
                DebugLog(
                    $"INTERCEPT node selected: '{NameOrNull(next)}'. " +
                    $"Min distance from path to player = {bestPathDist:F2}, desired pathPassRadius = {ctx.pathPassRadius:F2}."
                );
            }
            else
            {
                DebugLog("No valid intercept node found. Falling back to random node.");
            }
        }

        // If no intercept node found or we didn't try intercept, pick any random node
        if (next == null)
        {
            next = nodes[Random.Range(0, nodes.Count)];
            DebugLog($"RANDOM node selected: '{NameOrNull(next)}'.");
        }

        currentTarget = next;
        if (agent.SetDestination(currentTarget.position))
        {
            DebugLog($"SetDestination to node '{NameOrNull(currentTarget)}' at position {currentTarget.position}.");
        }
        else
        {
            DebugLog($"SetDestination FAILED for node '{NameOrNull(currentTarget)}'.");
        }
    }

    private Transform FindBestInterceptNode(out float bestMinPathDistance)
    {
        bestMinPathDistance = float.PositiveInfinity;
        if (player == null || nodes == null || nodes.Count == 0)
            return null;

        Vector3 playerPos = player.position;
        Transform bestNode = null;

        DebugLog("Evaluating nodes for BEST INTERCEPT path...");

        foreach (var node in nodes)
        {
            if (node == null) continue;

            NavMeshPath path = new NavMeshPath();
            bool pathFound = agent.CalculatePath(node.position, path);

            if (!pathFound)
            {
                DebugLog($"  Node '{NameOrNull(node)}': path NOT found.");
                continue;
            }

            if (path.status != NavMeshPathStatus.PathComplete)
            {
                DebugLog($"  Node '{NameOrNull(node)}': path incomplete (status = {path.status}).");
                continue;
            }

            float minDist = MinDistanceFromPathToPoint(path, playerPos);

            DebugLog(
                $"  Node '{NameOrNull(node)}': path COMPLETE, " +
                $"min distance from path to player = {minDist:F2}."
            );

            if (minDist < bestMinPathDistance)
            {
                bestMinPathDistance = minDist;
                bestNode = node;
            }
        }

        if (bestNode != null)
        {
            DebugLog(
                $"BEST intercept node = '{NameOrNull(bestNode)}' " +
                $"with min path distance to player = {bestMinPathDistance:F2}."
            );
        }
        else
        {
            DebugLog("No valid paths to any nodes. Cannot choose intercept node.");
        }

        return bestNode;
    }

    private float MinDistanceFromPathToPoint(NavMeshPath path, Vector3 point)
    {
        var corners = path.corners;
        if (corners == null || corners.Length == 0)
            return float.PositiveInfinity;

        float best = float.PositiveInfinity;

        for (int i = 0; i < corners.Length - 1; i++)
        {
            float d = DistancePointToSegment(point, corners[i], corners[i + 1]);
            if (d < best) best = d;
        }

        return best;
    }

    private float DistancePointToSegment(Vector3 point, Vector3 a, Vector3 b)
    {
        Vector3 ab = b - a;
        float denom = ab.sqrMagnitude;

        if (denom < 0.0001f)
            return Vector3.Distance(point, a);

        float t = Vector3.Dot(point - a, ab) / denom;
        t = Mathf.Clamp01(t);

        Vector3 closest = a + ab * t;
        return Vector3.Distance(point, closest);
    }

    // ===========================
    //  BACK-CONE TURN LOGIC
    // ===========================

    private void TrySuspiciousTurn()
    {
        if (player == null) return;

        float distToPlayer = Vector3.Distance(agent.transform.position, player.position);

        // Distance window (optional)
        if (distToPlayer < ctx.suspiciousMinDistance)
        {
            DebugLog($"Skipping suspicious turn: player too close ({distToPlayer:F1} < {ctx.suspiciousMinDistance:F1}).");
            return;
        }

        if (distToPlayer > ctx.suspiciousMaxDistance)
        {
            DebugLog($"Skipping suspicious turn: player too far ({distToPlayer:F1} > {ctx.suspiciousMaxDistance:F1}).");
            return;
        }

        // If we already see the player in FRONT, no need to spin
        if (ctx.vision != null && ctx.vision.CanSeePlayer)
        {
            DebugLog("Skipping suspicious turn: I already see the player in FRONT.");
            return;
        }

        // Require back cone + LOS from enemy to player
        if (ctx.vision == null || !ctx.vision.useBackVision || !ctx.vision.CanSeePlayerBack)
        {
            DebugLog("Skipping suspicious turn: player not in BACK cone with clear LOS.");
            return;
        }

        DebugLog($"Starting slow BACK-cone suspicious 180° turn. distanceToPlayer = {distToPlayer:F1}");

        isSuspiciousTurning = true;
        suspiciousTurnTimer = 0f;
        suspiciousStartRot = agent.transform.rotation;
        suspiciousTargetRot = suspiciousStartRot * Quaternion.Euler(0f, 180f, 0f);

        agent.isStopped = true;
        agent.ResetPath();
    }

    private void UpdateSuspiciousTurnRotation()
    {
        if (!isSuspiciousTurning) return;

        suspiciousTurnTimer += Time.deltaTime;
        float duration = Mathf.Max(0.01f, ctx.suspiciousTurnDuration);
        float t = Mathf.Clamp01(suspiciousTurnTimer / duration);

        agent.transform.rotation = Quaternion.Slerp(suspiciousStartRot, suspiciousTargetRot, t);

        if (t >= 1f)
        {
            isSuspiciousTurning = false;

            if (currentTarget != null)
            {
                agent.SetDestination(currentTarget.position);
            }

            agent.isStopped = false;
            DebugLog("Finished BACK-cone suspicious 180° turn, resuming patrol.");
        }
    }

    // ===========================
    //  UTILS / DEBUG
    // ===========================

    private string NameOrNull(Transform t)
    {
        return t != null ? t.name : "NULL";
    }

    private void DebugLog(string message)
    {
        if (!ctx.debugLogs) return;
        Debug.Log(logPrefix + message);
    }
}
