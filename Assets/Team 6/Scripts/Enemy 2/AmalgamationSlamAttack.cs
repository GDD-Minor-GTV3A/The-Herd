using Core.AI.Sheep;
using Gameplay.Player;      // kept for fallback if no sheep exist
using UnityEngine;
using UnityEngine.AI;

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
    private readonly Transform player;      // fallback target if no sheep
    private readonly string logPrefix;

    // NEW: sheep target (preferred)
    private Transform target;

    private Phase currentPhase = Phase.Idle;
    private float phaseTimer;

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

    // NEW overload: Begin with sheep target
    public void Begin(Transform sheepTarget)
    {
        target = sheepTarget != null ? sheepTarget : player; // fallback
        Begin();
    }

    // Original begin still exists so your old code style still works
    public void Begin()
    {
        if (target == null) target = player;

        currentPhase = Phase.Approach;
        phaseTimer = 0f;

        if (ctx.debugLogs)
            DebugLog("Slam BEGIN -> Approach");
    }

    public void Tick()
    {
        if (target == null || agent == null || !agent.enabled)
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

    public void Cancel()
    {
        currentPhase = Phase.Idle;

        if (ctx.slamTelegraph != null)
            ctx.slamTelegraph.Hide();
    }

    private void UpdateApproach()
    {
        phaseTimer += Time.deltaTime;

        SetAttackDestination();
        RotateTowardsTarget();

        float maxAllowedDist = ctx.attackApproachDistance + ctx.attackArriveThreshold;

        float distToTarget = Vector3.Distance(agent.transform.position, target.position);
        bool closeByDistance = distToTarget <= maxAllowedDist;

        bool closeByPath =
            !agent.pathPending &&
            agent.hasPath &&
            agent.remainingDistance <= ctx.attackArriveThreshold;

        if (closeByDistance || closeByPath)
        {
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
                    $"Slam APPROACH complete. distToTarget={distToTarget:F2}, " +
                    $"remainingDist={agent.remainingDistance:F2}. Locking pose and telegraphing."
                );
            }

            agent.isStopped = true;
            agent.ResetPath();
            agent.velocity = Vector3.zero;
            agent.updatePosition = false;

            agent.transform.position = attackOrigin;
            agent.transform.rotation = attackRotation;

            currentPhase = Phase.Telegraph;
            phaseTimer = 0f;

            float outerRadius = ctx.slamRange;
            float innerRadius = outerRadius * ctx.slamInnerRadiusFactor;

            if (ctx.slamTelegraph != null)
            {
                ctx.slamTelegraph.transform.position = attackOrigin;
                ctx.slamTelegraph.transform.rotation = attackRotation;
                ctx.slamTelegraph.Show(outerRadius, innerRadius, ctx.slamAngle);
            }

            if (ctx.anim != null)
                ctx.anim.TriggerSlam();
        }
    }

    private void UpdateTelegraph()
    {
        phaseTimer += Time.deltaTime;

        // keep facing target during telegraph
        RotateTowardsTarget();

        if (phaseTimer >= ctx.slamTelegraphTime)
        {
            DoDamage();

            if (ctx.slamTelegraph != null)
                ctx.slamTelegraph.Hide();

            agent.updatePosition = true;
            agent.isStopped = false;

            currentPhase = Phase.Idle;

            // Slam finishes by returning to chase (your project’s pattern)
            ctx.SwitchState(ctx.ChaseState);
        }
    }

    private void DoDamage()
    {
        Vector3 originPlanar = attackOrigin;
        Vector3 forwardPlanar = attackForward;

        originPlanar.y = 0f;
        forwardPlanar.y = 0f;
        if (forwardPlanar.sqrMagnitude < 0.0001f)
            forwardPlanar = Vector3.forward;
        forwardPlanar.Normalize();

        float maxRadius = ctx.slamRange;
        float innerRadius = maxRadius * ctx.slamInnerRadiusFactor;
        float halfAngle = ctx.slamAngle * 0.5f;

        // Only detect sheep layer (as requested)
        int mask = (ctx.sheepMask.value != 0) ? ctx.sheepMask.value : ~0;

        Collider[] hits = Physics.OverlapSphere(
            attackOrigin,
            maxRadius,
            mask,
            QueryTriggerInteraction.Collide
        );

        if (ctx.debugLogs)
        {
            Debug.Log(
                $"{logPrefix} Slam checking {hits.Length} colliders for SHEEP damage " +
                $"(innerRadius={innerRadius:F2}, maxRadius={maxRadius:F2}, halfAngle={halfAngle:F1})."
            );
        }

        var damaged = new System.Collections.Generic.HashSet<SheepStateManager>();

        foreach (Collider col in hits)
        {
            // Must be on a Sheep root tagged "Sheep"
            Transform t = col.transform;
            SheepStateManager sheepRoot = null;

            while (t != null)
            {
                if (t.CompareTag("Sheep"))
                {
                    sheepRoot = t.GetComponent<SheepStateManager>();
                    if (sheepRoot == null) sheepRoot = t.GetComponentInParent<SheepStateManager>();
                    break;
                }
                t = t.parent;
            }

            if (sheepRoot == null) continue;
            if (damaged.Contains(sheepRoot)) continue;

            SheepHealth hp = sheepRoot.GetComponent<SheepHealth>();
            if (hp == null || hp.IsDead) continue;

            Vector3 sheepPlanar = sheepRoot.transform.position;
            sheepPlanar.y = 0f;

            Vector3 toSheep = sheepPlanar - originPlanar;
            float distSheep = toSheep.magnitude;

            if (distSheep <= 0.001f || distSheep > maxRadius)
                continue;

            Vector3 dirSheep = toSheep / distSheep;
            float angleSheep = Vector3.Angle(forwardPlanar, dirSheep);

            if (angleSheep > halfAngle)
                continue;

            // Inner zone = stronger damage
            float damage = (distSheep <= innerRadius) ? ctx.slamInnerDamage : ctx.slamOuterDamage;

            hp.ApplyDamage(damage);
            damaged.Add(sheepRoot);

            if (ctx.debugLogs)
            {
                Debug.Log(
                    $"{logPrefix} Slam HIT SHEEP '{sheepRoot.name}': dist={distSheep:F2}, angle={angleSheep:F1}, dmg={damage}."
                );
            }
        }
    }

    private void SetAttackDestination()
    {
        Vector3 enemyPos = agent.transform.position;
        Vector3 targetPos = target.position;

        Vector3 fromTargetToEnemy = enemyPos - targetPos;
        fromTargetToEnemy.y = 0f;

        if (fromTargetToEnemy.sqrMagnitude < 0.01f)
        {
            fromTargetToEnemy = agent.transform.forward;
            fromTargetToEnemy.y = 0f;
        }

        fromTargetToEnemy.Normalize();

        float desiredDist = Mathf.Max(0f, ctx.attackApproachDistance);
        Vector3 desiredPos = targetPos + fromTargetToEnemy * desiredDist;

        if (NavMesh.SamplePosition(desiredPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            if (!agent.pathPending)
                agent.SetDestination(hit.position);
        }
        else if (!agent.pathPending)
        {
            agent.SetDestination(targetPos);
        }
    }

    private void RotateTowardsTarget()
    {
        if (target == null) return;

        Vector3 dir = target.position - agent.transform.position;
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
