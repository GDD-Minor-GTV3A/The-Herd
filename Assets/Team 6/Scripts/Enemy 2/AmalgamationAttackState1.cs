using Gameplay.Player;      // for Player.TakeDamage
using UnityEngine;
using UnityEngine.AI;

public class AmalgamationAttackState : IAmalgamationState
{
    private enum AttackType
    {
        SlamCone = 0,
        SecondAttack = 1
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
        }
    }

    public void Exit()
    {
        DebugLog("Exiting ATTACK state.");

        // Let both attacks clean themselves up; only the active one will actually matter
        slamAttack.Cancel();
        secondAttack.Cancel();

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
        AttackType candidate;

        bool mustSwitch = sameAttackCount >= 2; // <-- keeps "no 3 in a row"
        if (mustSwitch)
        {
            candidate = (lastAttackType == AttackType.SlamCone)
                ? AttackType.SecondAttack
                : AttackType.SlamCone;
        }
        else
        {
            candidate = (Random.value < 0.5f)
                ? AttackType.SlamCone
                : AttackType.SecondAttack;
        }

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
            DebugLog($"ChooseNextAttackType -> {candidate}, last={lastAttackType}, sameCount={sameAttackCount}");
        }

        return candidate;
    }

    private void DebugLog(string message)
    {
        if (!ctx.debugLogs) return;
        Debug.Log(logPrefix + message);
    }
}
