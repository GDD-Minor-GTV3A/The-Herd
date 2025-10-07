using System;

using Core.Shared;
using Core.Shared.StateMachine;

using UnityEngine;

public class FleeingState : IState
{
    private DrekavacStateManager _manager;
    private EnemyMovementController _movement;
    
    private float fleeingSpeed = 15f;
    private Vector3 fleeTarget;                       // Escape target position

    public FleeingState(DrekavacStateManager manager, EnemyMovementController movement)
    {
        _manager = manager;
        _movement = movement;
    }
    
    public void OnStart()
    {
        _movement.SetMovementSpeed(fleeingSpeed);
    }

    public void OnUpdate()
    {
        if (_movement.Agent.hasPath)
        {
            _movement.LookAt(_movement.Agent.steeringTarget); // face where running
        }

        // Check if agent really moved and reached destination
        if (!_movement.Agent.pathPending && _movement.Agent.hasPath && _movement.Agent.remainingDistance <= _movement.Agent.stoppingDistance + 0.5f)
        {
            _manager.Despawn();
        }
    }

    public void OnStop()
    {
        //throw new System.NotImplementedException();
    }
}
