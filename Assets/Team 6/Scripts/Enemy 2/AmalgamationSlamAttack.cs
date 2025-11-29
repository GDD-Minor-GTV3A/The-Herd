using Gameplay.Player;      // for Player.TakeDamage
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Handles the slam cone attack: approach, telegraph, and damage.
/// The AttackState just calls Begin/Tick/Cancel on this.
/// </summary>
public class AmalgamationSlamAttack
{
    private enum Phase
    {
        Idle,
        Approach,
        Telegraph
    }

    private readonly AmalgamationStateMachine ctx;
    private readonly NavMeshAgent agent;
    private readonly Transform player;
    private readonly string logPrefix;

    // State
    private Phase currentPhase = Phase.Idle;
    private float phaseTimer;

    // Cached pose for slam (where/rotation when attack fires)
    private Vector3 attackOrigin;
    private Quaternion attackRotation;
    private Vector3 attackForward;

    public AmalgamationSlamAttack(
        AmalgamationStateMachine ctx,
        NavMeshAgent agent,
        Transform player,
        string logPrefix)
    {
        this.ctx = ctx;
        this.agent = agent;
        this.player = player;
        this.logPrefix = logPrefix;
    }

    /// <summary>
    /// Start the slam-attack behaviour (called from AttackState.Enter when Slam is chosen).
    /// </summary>
    public void Begin()
    {
        currentPhase = Phase.Approach;
        phaseTimer = 0f;
    }

    /// <summary>
    /// Per-frame update for the slam (called from AttackState.Tick).
    /// </summary>
    public void Tick()
    {
        if (currentPhase == Phase.Idle)
            return;

        if (player == null || agent == null || !agent.enabled)
            return;

        switch (currentPhase)
        {
            case Phase.Approach:
                UpdateApproach();
                break;

            case Phase.Telegraph:
                UpdateTelegraph();
                break;
        }
    }

    /// <summary>
    /// Called from AttackState.Exit to clean up visuals if we leave the state early.
    /// </summary>
    public void Cancel()
    {
        currentPhase = Phase.Idle;
        phaseTimer = 0f;

        if (ctx.slamTelegraph != null)
            ctx.slamTelegraph.Hide();
    }

    // =========================================
    //              PHASES
    // =========================================

    private void UpdateApproach()
    {
        // Move + rotate while closing in to the desired distance
        SetAttackDestination();
        RotateTowardsPlayer();

        float distToPlayer = Vector3.Distance(agent.transform.position, player.position);

        float maxAllowedDist = ctx.attackApproachDistance + ctx.attackArriveThreshold;
        bool closeByDistance = distToPlayer <= maxAllowedDist;

        bool closeByPath =
            !agent.pathPending &&
            agent.remainingDistance <= ctx.attackArriveThreshold;

        if (closeByDistance || closeByPath)
        {
            // Lock in the pose for the slam
            attackOrigin = agent.transform.position;
            attackRotation = agent.transform.rotation;
            attackForward = attackRotation * Vector3.forward;
            attackForward.y = 0f;
            if (attackForward.sqrMagnitude < 0.0001f)
                attackForward = Vector3.forward;
            attackForward.Normalize();

            if (ctx.debugLogs)
            {
                DebugLog(
                    $"Slam APPROACH complete. distToPlayer={distToPlayer:F2}, " +
                    $"remainingDist={agent.remainingDistance:F2}. Locking pose and telegraphing."
                );
            }

            // Freeze navmesh movement
            agent.isStopped = true;
            agent.ResetPath();
            agent.velocity = Vector3.zero;
            agent.updatePosition = false;

            // Ensure we stay exactly at the cached pose
            agent.transform.position = attackOrigin;
            agent.transform.rotation = attackRotation;

            StartTelegraph();

            currentPhase = Phase.Telegraph;
            phaseTimer = 0f;
        }
    }

    private void UpdateTelegraph()
    {
        // Completely frozen: no movement, no rotation
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.transform.position = attackOrigin;
        agent.transform.rotation = attackRotation;

        phaseTimer += Time.deltaTime;
        if (phaseTimer >= ctx.slamTelegraphTime)
        {
            // Actually deal damage now from the cached pose
            DoDamage();

            if (ctx.slamTelegraph != null)
                ctx.slamTelegraph.Hide();

            currentPhase = Phase.Idle;

            // Immediately go back to CHASE; chase will resume ring logic
            ctx.SwitchState(ctx.ChaseState);
        }
    }

    // =========================================
    //              VISUALS
    // =========================================

    private void StartTelegraph()
    {
        if (ctx.slamTelegraph == null)
        {
            DebugLog("No ConeTelegraph assigned on state machine for slam.");
            return;
        }

        float outerRadius = ctx.slamRange;
        float innerRadius = outerRadius * ctx.slamInnerRadiusFactor;

        // Place telegraph at locked pose
        ctx.slamTelegraph.transform.position = attackOrigin;
        ctx.slamTelegraph.transform.rotation = attackRotation;

        ctx.slamTelegraph.Show(outerRadius, innerRadius, ctx.slamAngle);
    }

    // =========================================
    //              DAMAGE
    // =========================================

    private void DoDamage()
    {
        // Planar origin/forward for distance & angle math
        Vector3 originPlanar  = attackOrigin;
        Vector3 forwardPlanar = attackForward;

        originPlanar.y  = 0f;
        forwardPlanar.y = 0f;
        if (forwardPlanar.sqrMagnitude < 0.0001f)
            forwardPlanar = Vector3.forward;
        forwardPlanar.Normalize();

        float maxRadius   = ctx.slamRange;
        float innerRadius = maxRadius * ctx.slamInnerRadiusFactor;
        float halfAngle   = ctx.slamAngle * 0.5f;

        // --------------------------
        // PLAYER DAMAGE  (same logic as before)
        // --------------------------
        if (ctx.player != null)
        {
            Transform playerTransform = ctx.player;

            Vector3 playerPlanar = playerTransform.position;
            playerPlanar.y = 0f;

            Vector3 toPlayer = playerPlanar - originPlanar;
            float   dist     = toPlayer.magnitude;

            if (dist > 0.001f && dist <= maxRadius)
            {
                Vector3 dir   = toPlayer / dist;
                float   angle = Vector3.Angle(forwardPlanar, dir);

                if (angle <= halfAngle)
                {
                    // Search on root, parents, and children – this is what worked before
                    Player playerComp =
                        playerTransform.GetComponent<Player>() ??
                        playerTransform.GetComponentInParent<Player>() ??
                        playerTransform.GetComponentInChildren<Player>();

                    if (playerComp != null)
                    {
                        float damage =
                            (dist <= innerRadius)
                            ? ctx.slamInnerDamage      // inner cone
                            : ctx.slamOuterDamage;     // outer cone

                        playerComp.TakeDamage(damage);

                        if (ctx.debugLogs)
                        {
                            Debug.Log(
                                $"{logPrefix} Slam HIT PLAYER: dist={dist:F2}, angle={angle:F1}, dmg={damage}."
                            );
                        }
                    }
                    else if (ctx.debugLogs)
                    {
                        Debug.LogWarning(
                            $"{logPrefix} Slam tried to hit player but no Player component was found on '{playerTransform.name}'."
                        );
                    }
                }
                else if (ctx.debugLogs)
                {
                    Debug.Log(
                        $"{logPrefix} Slam player OUT OF ANGLE: dist={dist:F2}, angle={angle:F1}."
                    );
                }
            }
            else if (ctx.debugLogs)
            {
                Debug.Log(
                    $"{logPrefix} Slam player OUT OF RANGE: dist={dist:F2}, maxRadius={maxRadius:F2}."
                );
            }
        }

        // --------------------------
        // SHEEP INSTA-KILL (INNER CONE)
        // --------------------------

        // Use REAL attackOrigin height and INCLUDE triggers, then filter by inner cone
        Collider[] hits = Physics.OverlapSphere(
            attackOrigin,
            maxRadius,                      // full cone radius
            ~0,
            QueryTriggerInteraction.Collide // includes trigger colliders
        );

        if (ctx.debugLogs)
        {
            Debug.Log($"{logPrefix} Checking {hits.Length} colliders for SHEEP kills " +
                      $"(innerRadius={innerRadius:F2}, halfAngle={halfAngle:F1}).");
        }

        foreach (Collider col in hits)
        {
            Transform t         = col.transform;
            Transform sheepRoot = null;

            // Walk up the hierarchy until we find something tagged "Sheep"
            while (t != null)
            {
                if (t.CompareTag("Sheep"))
                {
                    sheepRoot = t;
                    break;
                }

                t = t.parent;
            }

            if (sheepRoot == null)
            {
                if (ctx.debugLogs)
                {
                    Debug.Log($"{logPrefix} Overlap hit '{col.name}' but no ancestor with tag 'Sheep' was found.");
                }
                continue;
            }

            Vector3 sheepPlanar = sheepRoot.position;
            sheepPlanar.y = 0f;

            Vector3 toSheep   = sheepPlanar - originPlanar;
            float   distSheep = toSheep.magnitude;

            // Must be in INNER radius
            if (distSheep <= 0.001f || distSheep > innerRadius)
            {
                if (ctx.debugLogs)
                {
                    Debug.Log(
                        $"{logPrefix} Sheep '{sheepRoot.name}' OUT OF INNER RADIUS " +
                        $"(dist={distSheep:F2}, innerRadius={innerRadius:F2})."
                    );
                }
                continue;
            }

            Vector3 dirSheep   = toSheep / distSheep;
            float   angleSheep = Vector3.Angle(forwardPlanar, dirSheep);

            if (angleSheep <= halfAngle)
            {
                Object.Destroy(sheepRoot.gameObject);

                if (ctx.debugLogs)
                {
                    Debug.Log(
                        $"{logPrefix} Slam KILLED SHEEP '{sheepRoot.name}': " +
                        $"dist={distSheep:F2}, angle={angleSheep:F1}."
                    );
                }
            }
            else if (ctx.debugLogs)
            {
                Debug.Log(
                    $"{logPrefix} Sheep '{sheepRoot.name}' OUT OF ANGLE " +
                    $"(dist={distSheep:F2}, angle={angleSheep:F1}, halfAngle={halfAngle:F1})."
                );
            }
        }
    }

    // =========================================
    //              HELPERS
    // =========================================

    private void SetAttackDestination()
    {
        Vector3 enemyPos  = agent.transform.position;
        Vector3 playerPos = player.position;

        Vector3 fromPlayerToEnemy = enemyPos - playerPos;
        fromPlayerToEnemy.y = 0f;

        if (fromPlayerToEnemy.sqrMagnitude < 0.01f)
        {
            // on top of the player, just pick some direction
            fromPlayerToEnemy = -player.forward;
        }

        fromPlayerToEnemy.Normalize();

        // like chase ring, but using attackApproachDistance
        float desiredDist = Mathf.Max(0f, ctx.attackApproachDistance);
        Vector3 desiredPos = playerPos + fromPlayerToEnemy * desiredDist;

        if (NavMesh.SamplePosition(desiredPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            if (!agent.pathPending)
                agent.SetDestination(hit.position);
        }
        else if (!agent.pathPending)
        {
            // fallback: just run directly to the player
            agent.SetDestination(playerPos);
        }
    }

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

    private void DebugLog(string message)
    {
        if (!ctx.debugLogs) return;
        Debug.Log(logPrefix + message);
    }
}
