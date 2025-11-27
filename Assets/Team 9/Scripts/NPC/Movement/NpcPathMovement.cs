using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class NpcPathMovement : MonoBehaviour
{
    [SerializeField] private NpcPath path;
    [SerializeField] private float reachThreshold = 1f;
    
    
    private NavMeshAgent _agent;
    private int _currentIndex = 0;

    private bool _isRunning = false;

    public bool IsRunning => _isRunning;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        if (_agent == null) return;
        if (path.waypoints.Length == 0) return;

        //_isRunning = true;
        //await FollowPath();
    }

    private async Task FollowPath()
    {
        if (path.IsCompleted)
        {
            _isRunning = false;
            return;
        }
        
        for (int i = 0; i < path.waypoints.Length && _isRunning; i++)
        {
            _agent.SetDestination(path.waypoints[i].position);

            await WaitUntilReached(_agent, reachThreshold);

            await Task.Delay(50);
        }

        _isRunning = false;
        if (path.IsOneShot)
            path.IsCompleted = true;
    }

    private static async Task WaitUntilReached(NavMeshAgent agent, float threshold)
    {
        while (agent.pathPending || agent.remainingDistance > threshold)
        {
            await Task.Yield();
        }
    }

    public void SetNewPath(NpcPath newPath)
    {
        path = newPath;
    }

    private async void RevertPath()
    {
        path.RevertPath();
        _isRunning = true;
        await FollowPath();

    }

    public async void StartPath()
    {
        _isRunning = true;
        await FollowPath();

        Debug.Log("NPC_PATH: Starting new Path");
        
    }



    private void OnDisable() => _isRunning = false;


    // Update is called once per frame
    void Update()
    {
 
    }
}
