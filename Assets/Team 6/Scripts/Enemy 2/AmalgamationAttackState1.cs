using Core.AI.Sheep;
using Gameplay.Player;      // still used by other attacks
using UnityEngine;
using UnityEngine.AI;

public class AmalgamationAttackState : IAmalgamationState
{
    private enum AttackType
    {
        SlamCone = 0,
        SecondAttack = 1,
        ShootLine = 2
    }

    private readonly AmalgamationStateMachine ctx;
    private readonly NavMeshAgent agent;
    private readonly Transform player;

    private readonly AmalgamationSlamAttack slamAttack;
    private readonly AmalgamationSecondAttack secondAttack;
    private readonly AmalgamationShootAttack shootAttack;

    private readonly string logPrefix;

    private AttackType currentAttackType;

    private static AttackType lastAttackType = AttackType.SlamCone;
    private static int sameAttackCount = 0;

    private Transform currentSheepTarget;

    public AmalgamationAttackState(
        AmalgamationStateMachine ctx,
        NavMeshAgent agent,
        Transform player)
    {
        this.ctx = ctx;
        this.agent = agent;
        this.player = player;

        logPrefix = "[Amalgamation " + ctx.gameObject.name + "][ATTACK] ";

        slamAttack = new AmalgamationSlamAttack(ctx, agent, player, logPrefix);
        secondAttack = new AmalgamationSecondAttack(ctx, agent, player, logPrefix);
        shootAttack = new AmalgamationShootAttack(ctx, agent, player, logPrefix);
    }

    public void Enter()
    {
        if (player == null || agent == null || !agent.enabled)
            return;

        // Pick a random sheep when we enter attack state (used by SLAM)
        currentSheepTarget = FindRandomAliveSheep();

        currentAttackType = ChooseNextAttackType();

        DebugLog($"Entering ATTACK state. Chosen attack type = {currentAttackType}.");

        // Generic nav-setup for any attack
        agent.isStopped = false;
        agent.updateRotation = false;
        agent.updatePosition = true;
        agent.stoppingDistance = 0f;
        agent.speed = ctx.attackSpeed;

        Debug.Log(
            $"[ATTACK DEBUG] {ctx.name} chose ATTACK = {currentAttackType}"
        );

        // Start specific attack behaviour
        switch (currentAttackType)
        {
            case AttackType.SlamCone:
                // Slam now prefers sheep; if none exist, it can fallback inside SlamAttack
                slamAttack.Begin(currentSheepTarget);
                break;

            case AttackType.SecondAttack:
                secondAttack.Begin();
                break;

            case AttackType.ShootLine:
                shootAttack.Begin();
                DebugLog($"Attack Enter: sheepTarget={(currentSheepTarget ? currentSheepTarget.name : "NULL")} player={(player ? player.name : "NULL")}");
                break;
        }
    }

   public void Tick()
    {
        if (player == null || agent == null || !agent.enabled)
            return;

        switch (currentAttackType)
        {
            case AttackType.SlamCone:
                slamAttack.Tick();
                break;

            case AttackType.SecondAttack:
                secondAttack.Tick();   // ✅ ONLY Tick here
                break;

            case AttackType.ShootLine:
                shootAttack.Tick();
                break;
        }
    }       


    public void Exit()
    {
        DebugLog("Exiting ATTACK state.");

        slamAttack.Cancel();
        secondAttack.Cancel();
        shootAttack.Cancel();

        if (agent != null && agent.enabled)
        {
            agent.updatePosition = true;
            agent.isStopped = false;
            agent.ResetPath();
            agent.velocity = Vector3.zero;
        }

        if (ctx.slamTelegraph != null)
        {
            ctx.slamTelegraph.Hide();
        }
    }

    // =========================================
    //   ATTACK SELECTION (no 3 same in a row)
    // =========================================

    private AttackType ChooseNextAttackType()
    {
        float distToPlayer = (player != null)
            ? Vector3.Distance(ctx.transform.position, player.position)
            : Mathf.Infinity;

        bool inShootRange =
            distToPlayer >= ctx.shootLineMinDistance &&
            distToPlayer <= ctx.shootLineMaxDistance;

        bool mustSwitch = (sameAttackCount >= 2);

        AttackType candidate;

        if (!inShootRange)
        {
            // close range: slam or second only
            if (mustSwitch)
            {
                candidate = (lastAttackType == AttackType.SlamCone)
                    ? AttackType.SecondAttack
                    : AttackType.SlamCone;
            }
            else
            {
                candidate = (Random.value < 0.5f) ? AttackType.SlamCone : AttackType.SecondAttack;
            }
        }
        else
        {
            // long range: slam / second / shoot
            AttackType[] options;

            if (mustSwitch)
            {
                if (lastAttackType == AttackType.SlamCone)
                    options = new[] { AttackType.SecondAttack, AttackType.ShootLine };
                else if (lastAttackType == AttackType.SecondAttack)
                    options = new[] { AttackType.SlamCone, AttackType.ShootLine };
                else
                    options = new[] { AttackType.SlamCone, AttackType.SecondAttack };
            }
            else
            {
                options = new[] { AttackType.SlamCone, AttackType.SecondAttack, AttackType.ShootLine };
            }

            candidate = options[Random.Range(0, options.Length)];
        }

        if (candidate == lastAttackType)
            sameAttackCount++;
        else
        {
            lastAttackType = candidate;
            sameAttackCount = 1;
        }

        if (ctx.debugLogs)
        {
            DebugLog(
                $"ChooseNextAttackType -> {candidate}, dist={distToPlayer:F1}, " +
                $"inShootRange={inShootRange}, last={lastAttackType}, sameCount={sameAttackCount}"
            );
        }

        DebugLog(
            $"ATTACK PICK: candidate={candidate} dist={distToPlayer:F2} " +
            $"shootRange=[{ctx.shootLineMinDistance},{ctx.shootLineMaxDistance}] inShootRange={inShootRange} " +
            $"mustSwitch={mustSwitch} lastAttackType={lastAttackType} sameAttackCount={sameAttackCount}"
        );

        return candidate;
    }

    // =========================================
    //   SHEEP TARGETING
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

        var pick = candidates[Random.Range(0, candidates.Count)];
        return pick != null ? pick.transform : null;
    }

    private void DebugLog(string message)
    {
        if (!ctx.debugLogs) return;
        Debug.Log(logPrefix + message);
    }
}
