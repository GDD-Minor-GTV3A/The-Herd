using Gameplay.Player;      // for Player.TakeDamage
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

    private readonly string logPrefix;

    // Attack selection (never 3 of the same)
    private AttackType currentAttackType;
    private AttackType lastAttackType = AttackType.SlamCone;
    private int sameAttackCount = 0;

    // Separate helpers for each attack
    private readonly AmalgamationSlamAttack slamAttack;
    private readonly AmalgamationSecondAttack secondAttack;
    private readonly AmalgamationShootAttack shootAttack;

    public AmalgamationAttackState(
        AmalgamationStateMachine ctx,
        NavMeshAgent agent,
        Transform player)
    {
        this.ctx = ctx;
        this.agent = agent;
        this.player = player;

        logPrefix = "[Amalgamation " + ctx.gameObject.name + "][ATTACK] ";

        slamAttack   = new AmalgamationSlamAttack(ctx, agent, player, logPrefix);
        secondAttack = new AmalgamationSecondAttack(ctx, agent, player, logPrefix);
        shootAttack = new AmalgamationShootAttack(ctx, agent, player, logPrefix);

    }

    public void Enter()
    {
        if (player == null || agent == null || !agent.enabled)
        {
            DebugLog("Cannot ENTER ATTACK: missing agent or player.");
            return;
        }

        // Pick which attack to use this time (never 3 identical in a row)
        currentAttackType = ChooseNextAttackType();

        DebugLog($"Entering ATTACK state. Chosen attack type = {currentAttackType}.");

        // Generic nav-setup for any attack
        agent.isStopped = false;
        agent.updateRotation = false;
        agent.updatePosition = true;
        agent.stoppingDistance = 0f;
        agent.speed = ctx.attackSpeed; // Slam will keep this, SecondAttack overrides with its own speed

        // Start specific attack behaviour
        switch (currentAttackType)
        {
            case AttackType.SlamCone:
                slamAttack.Begin();
                break;

            case AttackType.SecondAttack:
                secondAttack.Begin();
                break;

            case AttackType.ShootLine:
                shootAttack.Begin();
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
                secondAttack.Tick();
                break;

            case AttackType.ShootLine:
                shootAttack.Tick();
                break;
        }
    }

    public void Exit()
    {
        DebugLog("Exiting ATTACK state.");

        // Let both attacks clean themselves up; only the active one will actually matter
        slamAttack.Cancel();
        secondAttack.Cancel();
        shootAttack.Cancel();

        if (agent != null && agent.enabled)
        {
            // Restore navmesh control so Chase can work normally
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
        // Distance to player at the moment we decide the attack
        float distToPlayer = (player != null)
            ? Vector3.Distance(ctx.transform.position, player.position)
            : Mathf.Infinity;

        bool inShootRange =
            distToPlayer >= ctx.shootLineMinDistance &&
            distToPlayer <= ctx.shootLineMaxDistance;

        AttackType candidate;

        bool mustSwitch = sameAttackCount >= 2; // "no 3 in a row" rule stays

        if (!inShootRange)
        {
            // ---------- CLOSE / MID RANGE ----------
            // Behaviour 100% stays in your teammates' world:
            // only SlamCone / SecondAttack, no ShootLine here.

            if (mustSwitch)
            {
                candidate = (lastAttackType == AttackType.SlamCone)
                    ? AttackType.SecondAttack
                    : AttackType.SlamCone;
            }
            else
            {
                // 50/50 between Slam and Second
                candidate = (Random.value < 0.5f)
                    ? AttackType.SlamCone
                    : AttackType.SecondAttack;
            }
        }
        else
        {
            // ---------- LONG RANGE ----------
            // Now ShootLine is allowed.

            AttackType[] options;

            if (mustSwitch)
            {
                // Can't repeat same attack 3x; build a list that excludes lastAttackType
                if (lastAttackType == AttackType.SlamCone)
                    options = new[] { AttackType.SecondAttack, AttackType.ShootLine };
                else if (lastAttackType == AttackType.SecondAttack)
                    options = new[] { AttackType.SlamCone, AttackType.ShootLine };
                else // lastAttackType == AttackType.ShootLine
                    options = new[] { AttackType.SlamCone, AttackType.SecondAttack };
            }
            else
            {
                // All three are possible
                options = new[]
                {
                AttackType.SlamCone,
                AttackType.SecondAttack,
                AttackType.ShootLine
            };
            }

            candidate = options[Random.Range(0, options.Length)];
        }

        // ---- bookkeeping for "no 3 same in a row" ----
        if (candidate == lastAttackType)
        {
            sameAttackCount++;
        }
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

        return candidate;
    }


    private void DebugLog(string message)
    {
        if (!ctx.debugLogs) return;
        Debug.Log(logPrefix + message);
    }
}
