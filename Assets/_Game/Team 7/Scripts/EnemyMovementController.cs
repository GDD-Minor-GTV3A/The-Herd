using Core.Shared;

using UnityEngine;
using UnityEngine.AI;

public class EnemyMovementController : MovementController
{
    public NavMeshAgent Agent { get; private set; }
    private float _moveSpeed = 3.5f;
    
    public void Initialize()
    {
        Agent = GetComponent<NavMeshAgent>();
        if (Agent is null)
        {
            Debug.LogError("EnemyAI: No NavMeshAgent component found.");
            enabled = false;
            return;
        }
        Agent.speed = _moveSpeed;
    }

    public override void MoveTo(Vector3 target)
    {
        SetDestination(target);
    }

    private void SetDestination(Vector3 destination)
    {
        Agent.SetDestination(destination);
    }

    public void LookAt(Vector3 targetPosition)
    {
        Vector3 lookDir = (targetPosition - transform.position).normalized;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
        }
    }
    
    public void SetMovementSpeed(float speed)
    {
        Agent.speed = speed;
    }

    public float GetMovementSpeed()
    {
        return Agent.speed;
    }

    public void ResetAgent()
    {
        ToggleAgent(false);
        Agent.velocity = Vector3.zero;
        Agent.ResetPath();
        ToggleAgent(true);
    }

    public void ToggleAgent(bool toggled)
    {
        Agent.isStopped = !toggled;
    }

    public void Awake()
    {
        Initialize();
    }
}
