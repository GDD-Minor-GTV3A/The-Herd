using System;

using Core.Shared;
using Core.Shared.StateMachine;

using UnityEngine;
using UnityEngine.AI;

public class DraggingState : IState
{
    private DrekavacStateManager _manager;
    private DrekavacAnimatorController _animator;
    private EnemyMovementController _movement;
    private float _draggingSpeed = 1.2f;
    private float _dragAwayDistance = 20f;
    float _despawnDistance = 30f; 
    
    public DraggingState(DrekavacStateManager manager, EnemyMovementController movement)
    {
        _manager = manager;
        _movement = movement;
        _animator = _manager.drekavacAnimatorController ?? throw new ArgumentNullException(nameof(manager.drekavacAnimatorController));
    }
    
    public void OnStart()
    {
        _movement.ToggleAgent(true);
        _animator.SetDragging(true);
        _movement.SetMovementSpeed(_draggingSpeed);
        _manager.AudioController.PlayChomp();
    }

    public void OnUpdate()
    {
        if (_manager.grabbedObject is null)
        {
            _manager.SetState<HuntingState>();
            return;
        }

        if (Vector3.Distance(_manager.transform.position, _manager.playerLocation.position) > _despawnDistance)
        {
            _manager.Despawn();
        }

        // Compute average position of remaining sheep (excluding grabbed sheep)
        GameObject[] sheepObjects = GameObject.FindGameObjectsWithTag("Sheep");
        Vector3 sheepCenter = Vector3.zero;
        int count = 0;
        foreach (GameObject sheep in sheepObjects)
        {
            if (sheep != _manager.grabbedObject)
            {
                sheepCenter += sheep.transform.position;
                count++;
            }
        }
        sheepCenter = count > 0 ? sheepCenter / count : _manager.playerLocation.position;

        var position = _manager.transform.position;
        
        // Run in opposite direction of sheep herd
        Vector3 awayDir = (position - sheepCenter).normalized;
        Vector3 escapeTarget = position + awayDir * _dragAwayDistance;

        if (NavMesh.SamplePosition(escapeTarget, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            _movement.MoveTo(hit.position);

        // Face backwards toward the herd (look at herd, move away)
        _movement.LookAt(sheepCenter);
    }

    public void OnStop()
    {
        _animator.SetDragging(false);
    }
}
