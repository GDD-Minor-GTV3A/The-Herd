using Core.AI.Sheep;
using UnityEngine;
using UnityEngine.AI;

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
    private readonly Transform player;   // fallback if no sheep
    private readonly string logPrefix;

    private Phase currentPhase = Phase.Idle;
    private float phaseTimer;

    private Transform target;            // current sheep target (preferred)

    // Cached pose for when the attack fires
    private Vector3 attackOrigin;
    private Quaternion attackRotation;
    private Vector3 attackForward;

    public bool IsFinished { get; private set; } = false;

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
    /// Called once by AttackState.Enter.
    /// Picks its own sheep target.
    /// </summary>
    public void Begin()
    {
        if (agent == null || !agent.enabled)
        {
            DebugLog("Cannot BEGIN SecondAttack: missing agent.");
            currentPhase = Phase.Idle;
            IsFinished = true;
            return;
        }

        IsFinished = false;

        // Pick a sheep for this attack
        target = FindRandomAliveSheep();

        // Fallback if none exist
        if (target == null)
            target = player;

        if (target == null)
        {
            DebugLog("SecondAttack: no sheep and no player fallback -> finish immediately.");
            currentPhase = Phase.Idle;
            IsFinished = true;
            return;
        }

        currentPhase = Phase.Charge;
        phaseTimer = 0f;

        // Ensure nav is enabled for movement
        agent.ResetPath();
        agent.isStopped = false;
        agent.updatePosition = true;
        agent.speed = ctx.secondAttackChargeSpeed;

        // Show telegraph (using slamTelegraph as in your original)
        if (ctx.slamTelegraph != null)
        {
            ctx.slamTelegraph.transform.position = agent.transform.position;
            ctx.slamTelegraph.transform.rotation = agent.transform.rotation;

            ctx.slamTelegraph.Show(
                ctx.secondAttackOuterRadius,
                ctx.secondAttackInnerRadius,
                ctx.secondAttackAngle
            );
        }

        DebugLog($"SecondAttack BEGIN: charging towards target '{target.name}'.");
    }

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

    public void Cancel()
    {
        if (currentPhase == Phase.Idle)
            return;

        currentPhase = Phase.Idle;
        phaseTimer = 0f;
        IsFinished = true;

        if (ctx.slamTelegraph != null)
            ctx.slamTelegraph.Hide();
    }

    // =========================================
    //              PHASES
    // =========================================

    private void UpdateCharge()
    {
        if (agent == null || !agent.enabled)
            return;

        // If sheep died/despawned mid-charge, retarget once
        if (target == null)
            target = FindRandomAliveSheep();

        // If still none, fallback player; if none, bail
        if (target == null)
            target = player;

        if (target == null)
        {
            DebugLog("SecondAttack target lost and no fallback -> finish.");
            FinishToChase();
            return;
        }

        phaseTimer += Time.deltaTime;

        // Move towards target (sample onto navmesh so it actually moves)
        Vector3 desired = target.position;
        if (NavMesh.SamplePosition(desired, out NavMeshHit hit, 3f, NavMesh.AllAreas))
        {
            if (!agent.pathPending)
                agent.SetDestination(hit.position);
        }
        else
        {
            // still try raw position
            if (!agent.pathPending)
                agent.SetDestination(desired);
        }

        RotateTowardsTarget();

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

        // Freeze pose during pause
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.transform.position = attackOrigin;
        agent.transform.rotation = attackRotation;

        if (phaseTimer >= ctx.secondAttackFirePauseTime)
        {
            DebugLog("SecondAttack fire pause finished -> back to CHASE.");
            FinishToChase();
        }
    }

    private void FireAttack()
    {
        if (agent == null || !agent.enabled)
            return;

        // Lock pose at fire moment
        attackOrigin = agent.transform.position;
        attackRotation = agent.transform.rotation;

        attackForward = attackRotation * Vector3.forward;
        attackForward.y = 0f;
        if (attackForward.sqrMagnitude < 0.0001f)
            attackForward = Vector3.forward;
        attackForward.Normalize();

        DebugLog("SecondAttack FIRE: applying SHEEP damage in 2 rings.");

        DoDamage();

        if (ctx.slamTelegraph != null)
            ctx.slamTelegraph.Hide();

        // Freeze briefly after firing
        currentPhase = Phase.FirePause;
        phaseTimer = 0f;

        agent.isStopped = true;
        agent.ResetPath();
        agent.velocity = Vector3.zero;
    }

    private void FinishToChase()
    {
        currentPhase = Phase.Idle;
        IsFinished = true;
        ctx.SwitchState(ctx.ChaseState);
    }

    // =========================================
    //              DAMAGE (SHEEP ONLY)
    // =========================================

    private void DoDamage()
    {
        Vector3 originPlanar = attackOrigin;
        Vector3 forwardPlanar = attackForward;
        originPlanar.y = 0f;
        forwardPlanar.y = 0f;

        if (forwardPlanar.sqrMagnitude < 0.0001f)
            forwardPlanar = Vector3.forward;
        forwardPlanar.Normalize();

        float innerRadius = ctx.secondAttackInnerRadius;
        float outerRadius = ctx.secondAttackOuterRadius;
        float halfAngle = ctx.secondAttackAngle * 0.5f;

        int mask = (ctx.sheepMask.value != 0) ? ctx.sheepMask.value : ~0;

        Collider[] hits = Physics.OverlapSphere(
            attackOrigin,
            outerRadius,
            mask,
            QueryTriggerInteraction.Collide
        );

        var damaged = new System.Collections.Generic.HashSet<SheepStateManager>();

        foreach (Collider col in hits)
        {
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
            float dist = toSheep.magnitude;

            if (dist <= 0.001f || dist > outerRadius)
                continue;

            float angle = Vector3.Angle(forwardPlanar, toSheep / dist);
            if (angle > halfAngle)
                continue;

            bool isInner = dist <= innerRadius;

            bool shouldHit = isInner;
            if (!isInner)
            {
                float roll = Random.value;
                shouldHit = roll < ctx.secondAttackOuterSheepKillChance; // reused as hit chance
            }

            if (!shouldHit)
                continue;

            float dmg = isInner ? ctx.secondAttackInnerDamage : ctx.secondAttackOuterDamage;

            hp.ApplyDamage(dmg);
            damaged.Add(sheepRoot);

            if (ctx.debugLogs)
            {
                Debug.Log($"{logPrefix} SecondAttack HIT SHEEP '{sheepRoot.name}': " +
                          $"zone={(isInner ? "INNER" : "OUTER")}, dist={dist:F2}, angle={angle:F1}, dmg={dmg}.");
            }
        }
    }

    // =========================================
    //              TARGETING
    // =========================================

    private Transform FindRandomAliveSheep()
    {
        var all = SheepStateManager.AllSheep;
        if (all == null || all.Count == 0)
            return null;

        var candidates = new System.Collections.Generic.List<SheepStateManager>();

        for (int i = 0; i < all.Count; i++)
        {
            var s = all[i];
            if (!s) continue;
            if (!s.isActiveAndEnabled) continue;

            var hp = s.GetComponent<SheepHealth>();
            if (hp != null && hp.IsDead) continue;

            candidates.Add(s);
        }

        if (candidates.Count == 0)
            return null;

        return candidates[Random.Range(0, candidates.Count)].transform;
    }

    private void RotateTowardsTarget()
    {
        if (agent == null || target == null) return;

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
