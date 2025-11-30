using UnityEngine;
using UnityEngine.AI;

public class SimpleVillagerAI : MonoBehaviour
{
    public enum State { Walking, Idle }

    [Header("References")]
    public NavMeshAgent agent;
    public Collider wanderArea;

    [Header("Settings")]
    public float idleTimeMin = 1.5f;
    public float idleTimeMax = 4f;
    public float pointReachThreshold = 0.5f;
    public float wanderRetryDelay = 0.25f;
    public float sampleRadius = 3f; // radius for NavMesh.SamplePosition
    public float fallbackDistance = 2f; // distance for fallback move

    private State currentState = State.Idle;
    private float idleTimer;

    private void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (!agent.isOnNavMesh)
            Debug.LogWarning("Agent is not on NavMesh: " + gameObject.name);

        EnterIdleState();
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Walking: UpdateWalking(); break;
            case State.Idle: UpdateIdle(); break;
        }
    }

    // -----------------------------
    private void UpdateWalking()
    {
        if (!agent.pathPending && agent.remainingDistance <= pointReachThreshold)
            EnterIdleState();
    }

    private void UpdateIdle()
    {
        idleTimer -= Time.deltaTime;
        if (idleTimer <= 0f)
            TrySetNewDestination();
    }

    private void EnterIdleState()
    {
        currentState = State.Idle;
        idleTimer = Random.Range(idleTimeMin, idleTimeMax);
        agent.ResetPath();
    }

    private void EnterWalkingState()
    {
        currentState = State.Walking;
    }

    // -----------------------------
    private void TrySetNewDestination()
    {
        const int maxTries = 10;

        for (int i = 0; i < maxTries; i++)
        {
            Vector3 randomPoint = GetRandomPointInArea();
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, sampleRadius, agent.areaMask))
            {
                SetDestinationWithinArea(hit.position);
                EnterWalkingState();
                return;
            }
        }

        // Fallback: pick random direction from current position
        Vector3 fallbackDir = Random.insideUnitSphere;
        fallbackDir.y = 0f;
        Vector3 fallbackPoint = transform.position + fallbackDir.normalized * fallbackDistance;
        NavMeshHit fallbackHit;
        if (NavMesh.SamplePosition(fallbackPoint, out fallbackHit, sampleRadius, agent.areaMask))
        {
            SetDestinationWithinArea(fallbackHit.position);
            EnterWalkingState();
            return;
        }

        // If still no valid point, idle briefly
        idleTimer = wanderRetryDelay;
        Debug.Log("Failed to find valid point after fallback: " + gameObject.name);
    }

    // Clamp destination inside the wander area
    private void SetDestinationWithinArea(Vector3 target)
    {
        if (wanderArea == null)
        {
            agent.SetDestination(target);
            return;
        }

        Bounds bounds = wanderArea.bounds;
        Vector3 clamped = target;
        clamped.x = Mathf.Clamp(target.x, bounds.min.x, bounds.max.x);
        clamped.z = Mathf.Clamp(target.z, bounds.min.z, bounds.max.z);
        clamped.y = transform.position.y; // keep same height

        NavMeshHit hit;
        if (NavMesh.SamplePosition(clamped, out hit, sampleRadius, agent.areaMask))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            // fallback to center
            agent.SetDestination(bounds.center);
        }
    }

    private Vector3 GetRandomPointInArea()
    {
        if (wanderArea == null)
        {
            Debug.LogWarning("Wander area missing on " + gameObject.name);
            return transform.position;
        }

        Vector3 basePos = wanderArea.bounds.center;
        Vector3 size = wanderArea.bounds.size * 0.5f;

        float x = Random.Range(-size.x, size.x);
        float z = Random.Range(-size.z, size.z);

        return new Vector3(basePos.x + x, transform.position.y, basePos.z + z);
    }

    // -----------------------------
    private void OnDrawGizmosSelected()
    {
        if (wanderArea != null)
        {
            Gizmos.color = new Color(0, 1, 0, 0.25f);
            Gizmos.DrawCube(wanderArea.bounds.center, wanderArea.bounds.size);
        }
    }
}
