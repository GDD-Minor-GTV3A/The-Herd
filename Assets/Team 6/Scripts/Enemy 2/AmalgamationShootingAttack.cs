using System.Collections;
using Core.AI.Sheep;
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

    private Transform target; // sheep target (preferred), fallback player

    public bool IsFinished { get; private set; }

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
        if (agent == null || !agent.enabled)
        {
            DebugLog("ShootLine BEGIN failed: missing agent.");
            IsFinished = true;
            return;
        }

        if (ctx.lineFirePoint == null || ctx.lineBulletPrefab == null)
        {
            DebugLog("ShootLine BEGIN failed: no firePoint or bullet prefab.");
            IsFinished = true;
            return;
        }

        // Pick a new sheep every time this attack begins
        target = FindRandomAliveSheep();
        if (target == null)
            target = player; // fallback
        
        if (target == null)
        {
            DebugLog("ShootLine BEGIN failed: no sheep and no player fallback.");
            IsFinished = true;
            return;
        }
        DebugLog($"ShootLine BEGIN: picked target={(target ? target.name : "NULL")} (playerFallback={(target==player)})");

        if (isRunning && routine != null)
            ctx.StopCoroutine(routine);

        IsFinished = false;
        isRunning = true;
        DebugLog($"ShootLine BEGIN: starting coroutine. isRunning was {isRunning}");

        routine = ctx.StartCoroutine(RunAttack());

        DebugLog($"ShootLine BEGIN: targeting '{target.name}'.");
    }

    public void Tick()
    {
        // coroutine driven
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
        IsFinished = true;
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
            // If the sheep died/despawned during telegraph, retarget once
            if (target == null || (target.CompareTag("Sheep") && IsSheepDead(target)))
            {
                target = FindRandomAliveSheep();
                if (target == null)
                    target = player;
            }

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
        IsFinished = true;
        ctx.SwitchState(ctx.ChaseState);
    }

    private void DrawTelegraph()
    {
        if (target == null)
            return;

        Vector3 enemyPos = ctx.transform.position;

        Vector3 origin = enemyPos;
        origin.y += 0.05f;

        Vector3 tgt = target.position;
        tgt.y = origin.y;

        Vector3 dir = tgt - origin;
        if (dir.sqrMagnitude < 0.001f)
            return;

        dir.Normalize();
        Vector3 end = origin + dir * ctx.lineIndicatorLength;

        if (lineTelegraph != null)
            lineTelegraph.Show(origin, end);
        else
            Debug.DrawRay(origin, dir * ctx.lineIndicatorLength, Color.red);
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

        if (target == null)
        {
            DebugLog("FireLine failed: no target.");
            return;
        }

        Vector3 origin = ctx.lineFirePoint.position;

        Vector3 dir = target.position - origin;
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

        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);

        for (int i = 0; i < bullets; i++)
        {
            float offset = (i - halfIndex) * ctx.lineBulletSpacing;
            Vector3 spawnPos = origin + right * offset;

            Object.Instantiate(ctx.lineBulletPrefab, spawnPos, rot);
        }

        DebugLog($"ShootLine FIRED {bullets} bullets toward '{target.name}'.");
    }

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

    private bool IsSheepDead(Transform sheepTransform)
    {
        if (sheepTransform == null) return true;

        var hp =
            sheepTransform.GetComponent<SheepHealth>() ??
            sheepTransform.GetComponentInParent<SheepHealth>() ??
            sheepTransform.GetComponentInChildren<SheepHealth>();

        return (hp != null && hp.IsDead);
    }

    private void DebugLog(string msg)
    {
        if (!ctx.debugLogs) return;
        Debug.Log($"{logPrefix}{msg}");
    }
}
