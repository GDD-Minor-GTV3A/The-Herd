using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class NpcPathMovement : MonoBehaviour
{
    [SerializeField] private NpcPath path;
    [SerializeField] private float reachThreshold = 1f;

    private NavMeshAgent _agent;
    private bool _isRunning = false;
    private CancellationTokenSource _cts;

    public bool IsRunning => _isRunning;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        _cts = new CancellationTokenSource();
    }

    private void OnDisable()
    {
        _isRunning = false;

        // Cancel any pending async tasks
        if (_cts != null && !_cts.IsCancellationRequested)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
    }

    public void SetNewPath(NpcPath newPath)
    {
        path = newPath;
    }

    public async void StartPath()
    {
        if (_agent == null || path == null || path.waypoints.Length == 0)
        {
            Debug.LogWarning("NPC_PATH: Missing agent or path data.");
            return;
        }

        if (_isRunning)
        {
            Debug.Log("NPC_PATH: Already running.");
            return;
        }

        _isRunning = true;

        try
        {
            await FollowPath(_cts.Token);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("NPC_PATH: Movement cancelled (scene changed or disabled).");
        }
        finally
        {
            _isRunning = false;
        }
    }

    private async Task FollowPath(CancellationToken token)
    {
        if (path.IsCompleted) return;

        Debug.Log("NPC_PATH_MOVEMENT: Follow Path started");

        for (int i = 0; i < path.waypoints.Length && _isRunning; i++)
        {
            if (token.IsCancellationRequested || _agent == null) break;
            if (!_agent.isActiveAndEnabled) break;

            _agent.SetDestination(path.waypoints[i].position);

            await WaitUntilReached(_agent, reachThreshold, token);

            await Task.Delay(50, token);
        }

        if (path.IsOneShot)
            path.IsCompleted = true;
    }

    private static async Task WaitUntilReached(NavMeshAgent agent, float threshold, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (agent == null || !agent.isActiveAndEnabled) break;

            if (!agent.pathPending && agent.remainingDistance <= threshold)
                break;

            await Task.Yield();
        }
    }

    public async void RevertPath()
    {
        if (path == null) return;

        path.RevertPath();
        _isRunning = true;

        try
        {
            await FollowPath(_cts.Token);
        }
        catch (OperationCanceledException) { }
        finally
        {
            _isRunning = false;
        }
    }
}