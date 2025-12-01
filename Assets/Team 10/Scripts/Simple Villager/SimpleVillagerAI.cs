using UnityEngine;
using UnityEngine.AI;

public class SimpleVillagerAI : MonoBehaviour
{
    // Define the NPC states: Walking or Idle
    public enum State { Walking, Idle }

    [Header("References")]
    public NavMeshAgent agent;      // Reference to the NavMeshAgent component
    public Collider wanderArea;     // Collider defining the NPC's allowed movement zone

    [Header("Settings")]
    public float idleTimeMin = 1.5f;    // Minimum idle time in seconds
    public float idleTimeMax = 4f;      // Maximum idle time in seconds
    public float pointReachThreshold = 0.5f;  // Distance threshold to consider destination reached
    public float wanderRetryDelay = 0.25f;    // Time to wait before retrying a failed destination
    public float sampleRadius = 3f;     // Radius for NavMesh.SamplePosition
    public float fallbackDistance = 2f; // Distance for fallback movement if no valid point found

    private State currentState = State.Idle;
    private float idleTimer;

    private void Start()
    {
        // Assign NavMeshAgent if not set in Inspector
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        // Warn if agent is not currently on NavMesh
        if (!agent.isOnNavMesh)
            Debug.LogWarning("Agent is not on NavMesh: " + gameObject.name);

        // Start in Idle state
        EnterIdleState();
    }

    private void Update()
    {
        // Update based on current state
        switch (currentState)
        {
            case State.Walking: UpdateWalking(); break;
            case State.Idle: UpdateIdle(); break;
        }
    }

    // -----------------------------
    // Walking state logic
    private void UpdateWalking()
    {
        // If agent reached its destination, switch to Idle
        if (!agent.pathPending && agent.remainingDistance <= pointReachThreshold)
            EnterIdleState();
    }

    // Idle state logic
    private void UpdateIdle()
    {
        // Countdown idle timer
        idleTimer -= Time.deltaTime;

        // When timer runs out, try to move to a new destination
        if (idleTimer <= 0f)
            TrySetNewDestination();
    }

    // -----------------------------
    // State transitions
    private void EnterIdleState()
    {
        currentState = State.Idle;
        idleTimer = Random.Range(idleTimeMin, idleTimeMax); // Random idle duration
        agent.ResetPath(); // Stop movement
        // TODO: Trigger idle animation here if NPC team provides one
    }

    private void EnterWalkingState()
    {
        currentState = State.Walking;
        // TODO: Trigger walking animation here if NPC team provides one
    }

    // -----------------------------
    // Attempt to find a new destination within the wander area
    private void TrySetNewDestination()
    {
        const int maxTries = 10;

        // Try several random points within wander area
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

        // Fallback: pick a random direction from current position if all else fails
        Vector3 fallbackDir = Random.insideUnitSphere;
        fallbackDir.y = 0f; // Keep flat
        Vector3 fallbackPoint = transform.position + fallbackDir.normalized * fallbackDistance;
        NavMeshHit fallbackHit;
        if (NavMesh.SamplePosition(fallbackPoint, out fallbackHit, sampleRadius, agent.areaMask))
        {
            SetDestinationWithinArea(fallbackHit.position);
            EnterWalkingState();
            return;
        }

        // Still no valid point? Stay idle for a short period before retrying
        idleTimer = wanderRetryDelay;
        //Debug.Log("Failed to find valid point after fallback: " + gameObject.name);
    }

    // -----------------------------
    // Clamp destination inside the wander area so NPC never leaves
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
        clamped.y = transform.position.y; // Keep same height

        NavMeshHit hit;
        if (NavMesh.SamplePosition(clamped, out hit, sampleRadius, agent.areaMask))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            // If no valid point, fallback to center of wander area
            agent.SetDestination(bounds.center);
        }
    }

    // -----------------------------
    // Pick a random point inside the wander area
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
    // Draw the wander area in the editor for visualization
    private void OnDrawGizmosSelected()
    {
        if (wanderArea != null)
        {
            Gizmos.color = new Color(0, 1, 0, 0.25f);
            Gizmos.DrawCube(wanderArea.bounds.center, wanderArea.bounds.size);
        }
    }
}
