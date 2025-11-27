using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AmalgamationChaseState : IAmalgamationState
{
    private readonly AmalgamationStateMachine ctx;
    private readonly NavMeshAgent agent;
    private readonly Transform player;
    private readonly List<Transform> nodes;

    private readonly string logPrefix;
    private float lastTimeHadLOS;
    private bool hasReachedPreferredRingOnce;

    // Timer for periodic attacks WHILE in chase
    private float nextAttackTime;

    public AmalgamationChaseState(
        AmalgamationStateMachine ctx,
        NavMeshAgent agent,
        Transform player,
        List<Transform> nodes)
    {
        this.ctx = ctx;
        this.agent = agent;
        this.player = player;
        this.nodes = nodes;

        logPrefix = "[Amalgamation " + ctx.gameObject.name + "][CHASE] ";
    }

    public void Enter()
    {
        if (player == null || agent == null || !agent.enabled)
        {
            DebugLog("Cannot ENTER chase: missing agent or player.");
            return;
        }

        ctx.anim?.PlayRunImmediate();

        DebugLog("Entering CHASE state.");

        agent.isStopped = false;
        agent.updateRotation = false; // we'll rotate manually to always look at the player

        lastTimeHadLOS = Time.time; // assume we had LOS when entering

        // IMPORTANT: we do NOT reset hasReachedPreferredRingOnce here.
        // It only resets when we lose aggro and go back to PATROL,
        // so the "fastSpeed rush" only happens once per aggro.

        ScheduleNextAttackTime();
        UpdateSpeed();
        UpdateChaseDestination();
    }

    public void Tick()
    {
        if (player == null || agent == null || !agent.enabled)
            return;

        // 1) Drop aggro if player is simply too far away
        float distToPlayer = Vector3.Distance(agent.transform.position, player.position);
        if (distToPlayer > ctx.chaseMaxDistance)
        {
            //DebugLog($"Player too far (dist={distToPlayer:F1} > chaseMaxDistance={ctx.chaseMaxDistance:F1}) -> losing aggro.");
            LoseAggroAndReturnToPatrol();
            return;
        }

        // 2) Check LOS for aggro maintenance
        bool hasLOS = HasDirectLineOfSightToPlayer();

        if (hasLOS)
        {
            lastTimeHadLOS = Time.time;
        }
        else
        {
            float lostFor = Time.time - lastTimeHadLOS;
            if (lostFor > ctx.chaseLoseSightDelay)
            {
                //DebugLog($"Lost LOS to player for {lostFor:F2}s -> losing aggro and returning to PATROL.");
                LoseAggroAndReturnToPatrol();
                return;
            }
        }

        UpdateSpeed();
        UpdateChaseDestination();
        RotateTowardsPlayer();

        // Periodic attack trigger (no attack duration here)
        MaybeTriggerAttack();
    }

    public void Exit()
    {
        DebugLog("Exiting CHASE state.");
        // Patrol.Enter() will re-enable agent.updateRotation.
    }

    // ===========================
    //  CHASE MOVEMENT LOGIC
    // ===========================

    private void UpdateSpeed()
    {
        float dist = Vector3.Distance(agent.transform.position, player.position);
        float oldSpeed = agent.speed;

        // How close do we have to be to count as "arrived" on the ring?
        float arriveDist = ctx.chasePreferredDistance + ctx.chasePreferredArriveTolerance;

        // FIRST time only: once we're inside that radius, switch to sustain speed
        if (!hasReachedPreferredRingOnce && dist <= arriveDist)
        {
            hasReachedPreferredRingOnce = true;

            if (ctx.debugLogs)
            {
                DebugLog($"Reached chase ring (dist={dist:F2} <= {arriveDist:F2}). " +
                         "Switching from catch-up speed to sustain speed.");
            }
        }

        float targetSpeed = !hasReachedPreferredRingOnce
            ? ctx.fastSpeed          // initial rush the first time we ever reach the ring this aggro
            : ctx.chaseSustainSpeed; // sustained speed for the rest of the chase

        agent.speed = targetSpeed;

        if (ctx.debugLogs && Mathf.Abs(agent.speed - oldSpeed) > 0.01f)
        {
            DebugLog($"Speed changed to {agent.speed:F2} (distance to player = {dist:F2}).");
        }
    }

    /// <summary>
    /// Compute a point on a ring around the player at chasePreferredDistance
    /// and ask NavMeshAgent to move there. NavMesh will handle walls/corners.
    /// </summary>
    private void UpdateChaseDestination()
    {
        Vector3 enemyPos = agent.transform.position;
        Vector3 playerPos = player.position;

        Vector3 fromPlayerToEnemy = enemyPos - playerPos;
        fromPlayerToEnemy.y = 0f;

        if (fromPlayerToEnemy.sqrMagnitude < 0.01f)
        {
            // If we're on top of the player somehow, pick an arbitrary direction
            fromPlayerToEnemy = -player.forward;
        }

        fromPlayerToEnemy.Normalize();

        float desiredDist = Mathf.Max(ctx.chasePreferredDistance, ctx.chaseMinDistance);
        Vector3 desiredPos = playerPos + fromPlayerToEnemy * desiredDist;

        // Try to snap this position onto the NavMesh
        if (NavMesh.SamplePosition(desiredPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            if (!agent.pathPending)
            {
                agent.SetDestination(hit.position);
            }
        }
        else
        {
            // Fallback: just chase directly to the player's position if sampling fails
            if (!agent.pathPending)
            {
                agent.SetDestination(playerPos);
            }
        }
    }

    /// <summary>
    /// Smoothly rotate the Amalgamation to always look at the player.
    /// </summary>
    private void RotateTowardsPlayer()
    {
        Vector3 dir = player.position - agent.transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.0001f)
            return;

        Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
        agent.transform.rotation = Quaternion.Slerp(
            agent.transform.rotation,
            targetRot,
            ctx.chaseRotationSpeed * Time.deltaTime
        );
    }

    /// <summary>
    /// Raycast enemyEye -> playerEye with NO range limit (only blocked by obstacles).
    /// Uses chasePlayerMask / chaseObstacleMask if set; otherwise falls back to vision masks,
    /// then to default layers. We IGNORE TRIGGERS and we ALSO IGNORE SHEEP (by tag),
    /// so sheep never break LOS.
    /// </summary>
    private bool HasDirectLineOfSightToPlayer()
    {
        if (player == null)
            return false;

        Vector3 enemyPos = agent.transform.position;
        Vector3 playerPos = player.position;

        float enemyEyeHeight = 1.6f;
        float playerEyeHeight = 1.6f;

        // ================== BUILD MASK ==================
        int mask;

        bool haveChaseMasks = (ctx.chaseObstacleMask.value != 0) || (ctx.chasePlayerMask.value != 0);
        if (haveChaseMasks)
        {
            mask = ctx.chaseObstacleMask | ctx.chasePlayerMask;
        }
        else if (ctx.vision != null)
        {
            enemyEyeHeight = ctx.vision.enemyEyeHeight;
            playerEyeHeight = ctx.vision.playerEyeHeight;

            mask = ctx.vision.obstacleMask | ctx.vision.playerMask;
            if (mask == 0)
            {
                mask = Physics.DefaultRaycastLayers;
                //DebugLog("WARNING: vision obstacleMask | playerMask == 0, using DefaultRaycastLayers for chase LOS.");
            }
        }
        else
        {
            mask = Physics.DefaultRaycastLayers;
        }

        // Heights from vision if available
        if (ctx.vision != null)
        {
            enemyEyeHeight = ctx.vision.enemyEyeHeight;
            playerEyeHeight = ctx.vision.playerEyeHeight;
        }

        Vector3 enemyEye = enemyPos + Vector3.up * enemyEyeHeight;
        Vector3 playerEye = playerPos + Vector3.up * playerEyeHeight;

        Vector3 dir = playerEye - enemyEye;
        float dist = dir.magnitude;
        if (dist <= 0.001f) return true;

        dir /= dist;

        // Debug line so you can always SEE the chase LOS check in Scene view
        Debug.DrawLine(enemyEye, playerEye, Color.blue, 0.05f);

        // ====== NEW: RaycastAll so we can skip sheep ======
        RaycastHit[] hits = Physics.RaycastAll(
            enemyEye,
            dir,
            dist,
            mask,
            QueryTriggerInteraction.Ignore
        );

        if (hits.Length == 0)
        {
            if (ctx.debugLogs)
            {
                DebugLog("CHASE LOS: raycast hit NOTHING between enemy and player.");
            }
            // Keep old semantics: treat this as no LOS (usually mask misconfig)
            return false;
        }

        // Sort by distance so we process closest first
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (var hit in hits)
        {
            // ====== 1) Ignore anything that is (or is under) a Sheep ======
            Transform t = hit.transform;
            bool isSheep = false;
            while (t != null)
            {
                if (t.CompareTag("Sheep"))
                {
                    isSheep = true;
                    break;
                }
                t = t.parent;
            }

            if (isSheep)
            {
                if (ctx.debugLogs)
                {
                    DebugLog($"CHASE LOS: ignoring '{hit.collider.name}' because it's a Sheep.");
                }
                continue; // do NOT block LOS, just skip
            }

            // ====== 2) Decide if this hit is actually the PLAYER ======
            bool layerMatchesPlayerMask = false;
            if (ctx.chasePlayerMask.value != 0)
            {
                layerMatchesPlayerMask =
                    ((1 << hit.collider.gameObject.layer) & ctx.chasePlayerMask.value) != 0;
            }
            else if (ctx.vision != null && ctx.vision.playerMask.value != 0)
            {
                layerMatchesPlayerMask =
                    ((1 << hit.collider.gameObject.layer) & ctx.vision.playerMask.value) != 0;
            }
            else
            {
                // no explicit player mask – allow any layer for the player hierarchy
                layerMatchesPlayerMask = true;
            }

            bool sameTransform   = hit.transform == player;
            bool isChildOfPlayer = hit.transform.IsChildOf(player);
            bool isParentOfPlayer = player.IsChildOf(hit.transform);

            bool transformIsPlayer = sameTransform || isChildOfPlayer || isParentOfPlayer;
            bool hitIsPlayer = layerMatchesPlayerMask && transformIsPlayer;

            if (hitIsPlayer)
            {
                if (ctx.debugLogs)
                {
                    //DebugLog($"CHASE LOS: saw PLAYER first (hit '{hit.collider.name}').");
                }
                return true;
            }

            // ====== 3) Anything else that we didn't ignore counts as a blocker ======
            if (ctx.debugLogs)
            {
                //DebugLog($"CHASE LOS BLOCKED by '{hit.collider.name}' (layer {hit.collider.gameObject.layer}).");
            }
            return false;
        }

        // We got hits, but all of them were ignored (e.g. only sheep)
        if (ctx.debugLogs)
        {
           // DebugLog("CHASE LOS: only ignored colliders (e.g. Sheep) between enemy and player, treating as CLEAR.");
        }
        return true;
    }

    /// <summary>
    /// Picks a patrol node near (but not too close to) the player, then switches to Patrol.
    /// </summary>
    private void LoseAggroAndReturnToPatrol()
    {
        // record the time so re-aggro cooldown works
        ctx.lastAggroLostTime = Time.time;

        // reset the "first rush" flag so the next aggro can use fastSpeed again
        hasReachedPreferredRingOnce = false;

        if (nodes != null && nodes.Count > 0 && player != null)
        {
            float minDist = ctx.chaseReengageMinNodeDistance;
            float maxDist = ctx.chaseReengageMaxNodeDistance;

            List<Transform> candidates = new List<Transform>();
            float bestFallbackDist = float.PositiveInfinity;
            Transform bestFallback = null;

            Vector3 playerPos = player.position;

            foreach (var node in nodes)
            {
                if (node == null) continue;

                float d = Vector3.Distance(playerPos, node.position);

                // Candidate for the nice band [min, max]
                if (d >= minDist && d <= maxDist)
                {
                    candidates.Add(node);
                }

                // Track closest as fallback
                if (d < bestFallbackDist)
                {
                    bestFallbackDist = d;
                    bestFallback = node;
                }
            }

            Transform chosen = null;
            if (candidates.Count > 0)
            {
                chosen = candidates[Random.Range(0, candidates.Count)];
            }
            else
            {
                chosen = bestFallback; // if no in range, just pick the nearest
            }

            ctx.forcedFirstPatrolNode = chosen;

            if (ctx.debugLogs)
            {
                DebugLog($"LoseAggro: chosen patrol node '{(chosen != null ? chosen.name : "NULL")}' near player.");
            }
        }

        ctx.SwitchState(ctx.PatrolState);
    }

    // ===========================
    //  ATTACK TIMER LOGIC
    // ===========================

    private void ScheduleNextAttackTime()
    {
        if (!ctx.attackEnabled) return;

        float interval = Random.Range(ctx.attackIntervalMin, ctx.attackIntervalMax);
        nextAttackTime = Time.time + interval;

        if (ctx.debugLogs)
        {
            //DebugLog($"Next ATTACK in {interval:F2}s (at t={nextAttackTime:F2}).");
        }
    }

    private void MaybeTriggerAttack()
    {
        if (!ctx.attackEnabled) return;
        if (Time.time < nextAttackTime) return;

        if (ctx.debugLogs)
        {
            DebugLog("Attack timer elapsed -> switching to ATTACK.");
        }

        ctx.SwitchState(ctx.AttackState);
        // NOTE: the next attack timer will be scheduled when we come back to CHASE (Enter()).
    }

    // ===========================
    //  UTILS / DEBUG
    // ===========================

    private void DebugLog(string message)
    {
        if (!ctx.debugLogs) return;
        Debug.Log(logPrefix + message);
    }
}
