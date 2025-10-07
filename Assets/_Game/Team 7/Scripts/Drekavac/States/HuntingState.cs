using Core.Shared;
using Core.Shared.StateMachine;

using UnityEngine;

public class HuntingState : IState
{
    private DrekavacStateManager _manager;
    private EnemyMovementController _movement;
    private float _huntingSpeed = 15f;
    
    public HuntingState(DrekavacStateManager manager, EnemyMovementController movement)
    {
        _manager = manager;
        _movement = movement;
    }

    public void OnStart()
    {
        _movement.ToggleAgent(true);
        _movement.SetMovementSpeed(_huntingSpeed);
    }

    public void OnUpdate()
    {
        if (_manager.grabbedObject is not null)
            return;

        GameObject[] sheepObjects = GameObject.FindGameObjectsWithTag("Sheep");
        if (sheepObjects.Length == 0) return;

        GameObject closestSheep = null;
        float closestDist = Mathf.Infinity;

        foreach (GameObject sheep in sheepObjects)
        {
            float dist = Vector3.Distance(_manager.transform.position, sheep.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestSheep = sheep;
            }
        }

        if (closestSheep is not null)
        {
            var closestPosition = closestSheep.transform.position;
            _movement.MoveTo(closestPosition);
            _movement.LookAt(closestPosition);
        }
    }

    public void OnStop()
    {
        //throw new System.NotImplementedException();
    }
}
