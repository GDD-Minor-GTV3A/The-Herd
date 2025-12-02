using System.Collections;

using UnityEngine;
using UnityEngine.AI;

public class AmalgamationShootAttack
{
    private readonly AmalgamationStateMachine ctx;
    private readonly NavMeshAgent agent;
    private readonly Transform player;
    private readonly string logPrefix;

    private bool isRunning;
    private Coroutine routine;

    private AmalgamationLineTelegraph lineTelegraph;

    public AmalgamationShootAttack(
        AmalgamationStateMachine ctx,
        NavMeshAgent agent,
        Transform player,
        string logPrefix)
    {
        this.ctx = ctx;
        this.agent = agent;
        this.player = player;
        this.logPrefix = logPrefix;

        // find telegraph in children
        lineTelegraph = ctx.GetComponentInChildren<AmalgamationLineTelegraph>(true);
    }

    public void Begin()
    {
        if (agent == null || player == null || !agent.enabled)
        {
            DebugLog("ShootLine BEGIN failed: missing agent or player.");
            return;
        }

        if (ctx.lineFirePoint == null || ctx.lineBulletPrefab == null)
        {
            DebugLog("ShootLine BEGIN failed: no firePoint or bullet prefab.");
            return;
        }

        if (isRunning && routine != null)
            ctx.StopCoroutine(routine);

        isRunning = true;
        routine = ctx.StartCoroutine(RunAttack());
    }

    public void Tick()
    {
        // Behaviour lives in coroutine; nothing needed here for now.
    }

    public void Cancel()
    {
        if (!isRunning) return;

        if (routine != null)
        {
            ctx.StopCoroutine(routine);
            routine = null;
        }

        HideTelegraph();
        isRunning = false;
    }

    private IEnumerator RunAttack()
    {
        // stop moving while we do this
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        float telegraphTime = 1.0f;
        float t = 0f;

        // TELEGRAPH PHASE
        while (t < telegraphTime)
        {
            DrawTelegraph();
            t += Time.deltaTime;
            yield return null;
        }

        // FIRE
        FireLine();
        HideTelegraph();

        // small delay, then back to chase
        yield return new WaitForSeconds(0.25f);

        if (agent != null && agent.enabled)
            agent.isStopped = false;

        isRunning = false;
        ctx.SwitchState(ctx.ChaseState);
    }

    private void DrawTelegraph()
    {
        if (player == null)
            return;

        // Take enemy position as base, flatten to ground
        Vector3 enemyPos = ctx.transform.position;

        // Slightly above ground so it doesn't z-fight with the floor
        Vector3 origin = enemyPos;
        origin.y += 0.05f;

        // Target: player's planar position at same Y
        Vector3 target = player.position;
        target.y = origin.y;

        Vector3 dir = target - origin;
        if (dir.sqrMagnitude < 0.001f)
            return;

        dir.Normalize();
        Vector3 end = origin + dir * ctx.lineIndicatorLength;

        if (lineTelegraph != null)
        {
            lineTelegraph.Show(origin, end);
        }
        else
        {
            Debug.DrawRay(origin, dir * ctx.lineIndicatorLength, Color.red);
        }
    }


    private void HideTelegraph()
    {
        if (lineTelegraph != null)
            lineTelegraph.Hide();
    }

    private void FireLine()
    {
        if (ctx.lineFirePoint == null || ctx.lineBulletPrefab == null)
        {
            DebugLog("FireLine failed: missing firePoint or prefab.");
            return;
        }

        Vector3 origin = ctx.lineFirePoint.position;
        Vector3 dir = player.position - origin;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f)
            dir = ctx.transform.forward;

        dir.Normalize();

        Vector3 right = Vector3.Cross(Vector3.up, dir);
        if (right.sqrMagnitude < 0.0001f)
            right = ctx.transform.right;
        right.Normalize();

        int bullets = Mathf.Max(1, ctx.lineBulletsInLine);
        float halfIndex = (bullets - 1) * 0.5f;

        for (int i = 0; i < bullets; i++)
        {
            float offset = (i - halfIndex) * ctx.lineBulletSpacing;
            Vector3 spawnPos = origin + right * offset;

            Object.Instantiate(
                ctx.lineBulletPrefab,
                spawnPos,
                Quaternion.LookRotation(dir, Vector3.up)
            );
        }

        DebugLog($"ShootLine FIRED {bullets} bullets.");
    }

    private void DebugLog(string msg)
    {
        if (!ctx.debugLogs) return;
        Debug.Log($"{logPrefix}{msg}");
    }
}
