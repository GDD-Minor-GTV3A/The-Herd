using Gameplay.Player;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Second attack:
///  - 2 zones (inner + outer) using same cone telegraph
///  - Inner ring: always damage player + always kill sheep
///  - Outer ring: 50/50 (configurable) for player + sheep
///  - Moves toward the player while charging, then fires and pauses.
/// </summary>
public class AmalgamationSecondAttack
{
    private enum Phase
    {
        Idle,
        Charge,
        FirePause
    }

    private readonly AmalgamationStateMachine ctx;
    private readonly NavMeshAgent agent;
    private readonly Transform player;
    private readonly string logPrefix;

    private Phase currentPhase = Phase.Idle;
    private float phaseTimer;

    // Cached pose for when the attack actually fires
    private Vector3 attackOrigin;
    private Quaternion attackRotation;
    private Vector3 attackForward;

    public AmalgamationSecondAttack(
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
    /// Called by AttackState.Enter when SecondAttack is chosen.
    /// </summary>
    public void Begin()
    {
        if (agent == null || player == null || !agent.enabled)
        {
            DebugLog("Cannot BEGIN SecondAttack: missing agent or player.");
            currentPhase = Phase.Idle;
            return;
        }

        currentPhase = Phase.Charge;
        phaseTimer = 0f;

        // Move towards player with separate speed
        agent.isStopped = false;
        agent.updatePosition = true;
        agent.speed = ctx.secondAttackChargeSpeed;

        // Show telegraph as a cone with two radii (inner + outer)
        if (ctx.slamTelegraph != null)
        {
            // Same world position / rotation style as slam, so y matches your cones
            ctx.slamTelegraph.transform.position = agent.transform.position;
            ctx.slamTelegraph.transform.rotation = agent.transform.rotation;

            ctx.slamTelegraph.Show(
                ctx.secondAttackOuterRadius,
                ctx.secondAttackInnerRadius,
                ctx.secondAttackAngle
            );
        }

        DebugLog("SecondAttack BEGIN: charging towards player.");
    }

    /// <summary>
    /// Called from AttackState.Tick while this attack is active.
    /// </summary>
    public void Tick()
    {
        switch (currentPhase)
        {
            case Phase.Charge:
                UpdateCharge();
                break;

            case Phase.FirePause:
                UpdateFirePause();
                break;
        }
    }

    /// <summary>
    /// Called from AttackState.Exit (or when something cancels the attack).
    /// </summary>
    public void Cancel()
    {
        if (currentPhase == Phase.Idle)
            return;

        currentPhase = Phase.Idle;
        phaseTimer = 0f;

        if (ctx.slamTelegraph != null)
            ctx.slamTelegraph.Hide();
    }

    // =========================================
    //              PHASES
    // =========================================

    private void UpdateCharge()
    {
        if (agent == null || player == null || !agent.enabled)
            return;

        phaseTimer += Time.deltaTime;

        // Move towards the player while charging
        Vector3 playerPos = player.position;
        if (!agent.pathPending)
        {
            agent.SetDestination(playerPos);
        }

        RotateTowardsPlayer();

        if (phaseTimer >= ctx.secondAttackChargeTime)
        {
            FireAttack();
        }
    }

    private void UpdateFirePause()
    {
        if (agent == null || !agent.enabled)
            return;

        phaseTimer += Time.deltaTime;

        // Stay frozen in place for the pause duration
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.transform.position = attackOrigin;
        agent.transform.rotation = attackRotation;

        if (phaseTimer >= ctx.secondAttackFirePauseTime)
        {
            DebugLog("SecondAttack fire pause finished -> back to CHASE.");
            currentPhase = Phase.Idle;
            ctx.SwitchState(ctx.ChaseState);
        }
    }

    private void FireAttack()
    {
        if (agent == null || player == null || !agent.enabled)
            return;

        // Lock pose at fire moment
        attackOrigin = agent.transform.position;
        attackRotation = agent.transform.rotation;
        attackForward = attackRotation * Vector3.forward;
        attackForward.y = 0f;
        if (attackForward.sqrMagnitude < 0.0001f)
            attackForward = Vector3.forward;
        attackForward.Normalize();

        DebugLog("SecondAttack FIRE: applying damage in 2 rings.");

        // Do damage
        DoDamage();

        // Hide telegraph
        if (ctx.slamTelegraph != null)
            ctx.slamTelegraph.Hide();

        // Freeze briefly after firing
        currentPhase = Phase.FirePause;
        phaseTimer = 0f;

        agent.isStopped = true;
        agent.ResetPath();
        agent.velocity = Vector3.zero;
    }

    // =========================================
    //              DAMAGE
    // =========================================

    private void DoDamage()
    {
        Vector3 originPlanar  = attackOrigin;
        Vector3 forwardPlanar = attackForward;

        originPlanar.y  = 0f;
        forwardPlanar.y = 0f;
        if (forwardPlanar.sqrMagnitude < 0.0001f)
            forwardPlanar = Vector3.forward;
        forwardPlanar.Normalize();

        float innerRadius = ctx.secondAttackInnerRadius;
        float outerRadius = ctx.secondAttackOuterRadius;
        float halfAngle   = ctx.secondAttackAngle * 0.5f;

        // --------------------------
        // PLAYER DAMAGE
        // --------------------------
        if (ctx.player != null)
        {
            Transform playerTransform = ctx.player;

            Vector3 playerPlanar = playerTransform.position;
            playerPlanar.y = 0f;

            Vector3 toPlayer = playerPlanar - originPlanar;
            float   dist     = toPlayer.magnitude;

            if (dist > 0.001f && dist <= outerRadius)
            {
                Vector3 dir   = toPlayer / dist;
                float   angle = Vector3.Angle(forwardPlanar, dir);

                if (angle <= halfAngle)
                {
                    // inner zone = guaranteed hit
                    bool isInner = dist <= innerRadius;
                    bool hitOuter = false;

                    if (!isInner)
                    {
                        float roll = Random.value;
                        hitOuter = (roll < ctx.secondAttackOuterPlayerHitChance);

                        if (ctx.debugLogs)
                        {
                            DebugLog($"SecondAttack PLAYER in outer ring: dist={dist:F2}, roll={roll:F2}, " +
                                     $"hitChance={ctx.secondAttackOuterPlayerHitChance:F2}, hit={hitOuter}.");
                        }
                    }

                    bool shouldHit = isInner || hitOuter;

                    if (shouldHit)
                    {
                        Player playerComp =
                            playerTransform.GetComponent<Player>() ??
                            playerTransform.GetComponentInParent<Player>() ??
                            playerTransform.GetComponentInChildren<Player>();

                        if (playerComp != null)
                        {
                            float dmg = isInner
                                ? ctx.secondAttackInnerDamage
                                : ctx.secondAttackOuterDamage;

                            playerComp.TakeDamage(dmg);

                            if (ctx.debugLogs)
                            {
                                Debug.Log(
                                    $"{logPrefix} SecondAttack HIT PLAYER: dist={dist:F2}, angle={angle:F1}, " +
                                    $"zone={(isInner ? "INNER" : "OUTER")}, dmg={dmg}."
                                );
                            }
                        }
                        else if (ctx.debugLogs)
                        {
                            Debug.LogWarning(
                                $"{logPrefix} SecondAttack tried to hit player but no Player component was found on '{playerTransform.name}'."
                            );
                        }
                    }
                    else if (ctx.debugLogs)
                    {
                        Debug.Log(
                            $"{logPrefix} SecondAttack PLAYER in OUTER ring but RNG said NO DAMAGE (dist={dist:F2})."
                        );
                    }
                }
                else if (ctx.debugLogs)
                {
                    Debug.Log(
                        $"{logPrefix} SecondAttack player OUT OF ANGLE: dist={dist:F2}, angle={angle:F1}, halfAngle={halfAngle:F1}."
                    );
                }
            }
        }

        // --------------------------
        // SHEEP DAMAGE
        // --------------------------

        // Overlap up to OUTER radius
        Collider[] hits = Physics.OverlapSphere(
            attackOrigin,
            outerRadius,
            ~0,
            QueryTriggerInteraction.Collide
        );

        if (ctx.debugLogs)
        {
            Debug.Log($"{logPrefix} SecondAttack checking {hits.Length} colliders for SHEEP in both rings " +
                      $"(innerRadius={innerRadius:F2}, outerRadius={outerRadius:F2}, halfAngle={halfAngle:F1}).");
        }

        foreach (Collider col in hits)
        {
            Transform t         = col.transform;
            Transform sheepRoot = null;

            // Walk up to find a "Sheep" tag
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
                    Debug.Log($"{logPrefix} SecondAttack overlap hit '{col.name}' but no ancestor with tag 'Sheep' was found.");
                }
                continue;
            }

            Vector3 sheepPlanar = sheepRoot.position;
            sheepPlanar.y = 0f;

            Vector3 toSheep   = sheepPlanar - originPlanar;
            float   distSheep = toSheep.magnitude;

            if (distSheep <= 0.001f || distSheep > outerRadius)
                continue;

            Vector3 dirSheep   = toSheep / distSheep;
            float   angleSheep = Vector3.Angle(forwardPlanar, dirSheep);

            if (angleSheep > halfAngle)
                continue;

            bool isInner = distSheep <= innerRadius;
            bool killOuter = false;

            if (!isInner)
            {
                float roll = Random.value;
                killOuter = (roll < ctx.secondAttackOuterSheepKillChance);

                if (ctx.debugLogs)
                {
                    Debug.Log(
                        $"{logPrefix} SecondAttack SHEEP '{sheepRoot.name}' in OUTER ring: " +
                        $"dist={distSheep:F2}, roll={roll:F2}, killChance={ctx.secondAttackOuterSheepKillChance:F2}, kill={killOuter}."
                    );
                }
            }

            bool shouldKill = isInner || killOuter;

            if (shouldKill)
            {
                Object.Destroy(sheepRoot.gameObject);

                if (ctx.debugLogs)
                {
                    Debug.Log(
                        $"{logPrefix} SecondAttack KILLED SHEEP '{sheepRoot.name}' " +
                        $"zone={(isInner ? "INNER" : "OUTER")}, dist={distSheep:F2}, angle={angleSheep:F1}."
                    );
                }
            }
            else if (ctx.debugLogs)
            {
                Debug.Log(
                    $"{logPrefix} SecondAttack SHEEP '{sheepRoot.name}' in OUTER ring BUT RNG spared it " +
                    $"(dist={distSheep:F2}, angle={angleSheep:F1})."
                );
            }
        }
    }

    // =========================================
    //              HELPERS
    // =========================================

    private void RotateTowardsPlayer()
    {
        if (agent == null || player == null) return;

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
