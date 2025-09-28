using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float circleRadius = 8f;       // Distance to keep while circling the player
    public float moveSpeed = 3.5f;        // Normal movement speed
    public float sprintSpeed = 7f;        // Speed when chasing a sheep
    public float dragSpeed = 1.2f;        // Speed when dragging a sheep
    public float circleSpeed = 2f;        // Speed of circling rotation

    [Header("Behavior Settings")]
    public float directionSwitchInterval = 5f;         // Time between direction switches while circling
    public Vector2 stalkDurationRange = new Vector2(10f, 20f); // Randomized stalking duration
    public Transform grabPoint;                        // Point on enemy where sheep is held

    [Header("Grab Settings")]
    public float dragAwayDistance = 20f;              // Distance to retreat while dragging
    public float despawnDistance = 30f;              // Distance from player to despawn enemy while dragging

    [Header("Animation")]
    public Animator animator;                         // Animator for controlling animations

    private NavMeshAgent agent;                       // Reference to NavMeshAgent component
    private Transform player;                         // Reference to player transform
    private float angleOffset;                        // Angle used for circling calculations
    private int circleDirection;                      // 1 or -1 for clockwise/counterclockwise
    private float nextSwitchTime;                     // Next time to switch circling direction
    private bool isSettled = false;                  // True when enemy reached circle radius
    private float stalkEndTime;                       // When to stop stalking and start hunting

    private enum EnemyState { Stalking, Hunting, Dragging }
    private EnemyState currentState = EnemyState.Stalking;

    private GameObject grabbedSheep;                 // Currently held sheep
    private Rigidbody grabbedSheepRb;                // Rigidbody of grabbed sheep
    private bool grabbedSheepOriginalKinematic;      // Original kinematic state to restore

    void Start()
    {
        // Get NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("EnemyAI: No NavMeshAgent component found.");
            enabled = false;
            return;
        }

        agent.speed = moveSpeed;

        // Find player by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
        {
            Debug.LogError("EnemyAI: No object with tag 'Player' found!");
            enabled = false;
            return;
        }

        // Create a grab point if not assigned
        if (grabPoint == null)
        {
            GameObject gp = new GameObject(name + "_GrabPoint");
            gp.transform.SetParent(transform);
            gp.transform.localPosition = new Vector3(0f, 0.5f, 0.6f);
            grabPoint = gp.transform;
        }

        // Position enemy at nearest point on the circle radius
        Vector3 toEnemy = (transform.position - player.position).normalized;
        Vector3 nearestPoint = player.position + toEnemy * circleRadius;

        if (NavMesh.SamplePosition(nearestPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);

        angleOffset = Mathf.Atan2(toEnemy.z, toEnemy.x) * Mathf.Rad2Deg;
        circleDirection = Random.value > 0.5f ? 1 : -1;  // Random initial circling direction

        ScheduleNextDirectionSwitch();
        stalkEndTime = Time.time + Random.Range(stalkDurationRange.x, stalkDurationRange.y);
    }

    void Update()
    {
        if (player == null) return;

        // Despawn only if dragging a sheep and too far from player
        if (grabbedSheep != null && Vector3.Distance(transform.position, player.position) > despawnDistance)
        {
            ReleaseGrabbedSheep();
            // IMPLEMENTATION TEAM ADD REPLACING SHEEP WITH DEAD ONE OR KILLING SHEEP CODE HERE
            Destroy(gameObject);
            return;
        }

        // Handle behavior states
        switch (currentState)
        {
            case EnemyState.Stalking:
                UpdateStalking();
                break;
            case EnemyState.Hunting:
                UpdateHunting();
                break;
            case EnemyState.Dragging:
                UpdateDragging();
                break;
        }
    }

    // Enemy circles the player while stalking
    private void UpdateStalking()
    {
        if (!isSettled)
        {
            float distToPlayer = Vector3.Distance(transform.position, player.position);
            if (Mathf.Abs(distToPlayer - circleRadius) < 0.5f)
                isSettled = true; // Enemy reached the circle radius
            else
            {
                LookAt(player.position);  // Face player while moving to radius
                return;
            }
        }

        // Switch circling direction at intervals
        if (Time.time >= nextSwitchTime)
        {
            circleDirection *= -1;
            ScheduleNextDirectionSwitch();
        }

        // Update circling angle
        angleOffset += circleSpeed * circleDirection * Time.deltaTime;

        // Calculate target position on circle
        float radians = angleOffset * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(radians), 0f, Mathf.Sin(radians)) * circleRadius;
        Vector3 targetPos = player.position + offset;

        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);

        LookAt(player.position);
        animator.SetBool("Stalking", true); // Play stalking animation

        if (Time.time >= stalkEndTime)
        {
            currentState = EnemyState.Hunting;
            animator.SetBool("Stalking", false);
            agent.speed = sprintSpeed;
        }
    }

    // Enemy hunts for nearest sheep
    private void UpdateHunting()
    {
        if (grabbedSheep != null) return; // Skip if already holding a sheep

        GameObject[] sheepObjects = GameObject.FindGameObjectsWithTag("Sheep");
        if (sheepObjects.Length == 0) return;

        GameObject closestSheep = null;
        float closestDist = Mathf.Infinity;

        foreach (GameObject sheep in sheepObjects)
        {
            float dist = Vector3.Distance(transform.position, sheep.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestSheep = sheep;
            }
        }

        if (closestSheep != null)
        {
            agent.SetDestination(closestSheep.transform.position);
            LookAt(closestSheep.transform.position);
        }
    }

    // Handles grabbing a sheep and positioning it at the edge of colliders
    private void GrabSheep(GameObject sheep)
    {
        if (sheep == null) return;

        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.ResetPath();

        grabbedSheep = sheep;
        grabbedSheepRb = grabbedSheep.GetComponent<Rigidbody>();
        if (grabbedSheepRb != null)
        {
            grabbedSheepRb.linearVelocity = Vector3.zero;
            grabbedSheepRb.angularVelocity = Vector3.zero;
            grabbedSheepOriginalKinematic = grabbedSheepRb.isKinematic;
            grabbedSheepRb.isKinematic = true;
        }

        // Position sheep at the edge of colliders
        Collider enemyCollider = GetComponent<Collider>();
        Collider sheepCollider = grabbedSheep.GetComponent<Collider>();
        if (enemyCollider != null && sheepCollider != null)
        {
            Vector3 direction = (grabbedSheep.transform.position - transform.position).normalized;

            float enemyEdge = GetColliderExtentAlongDirection(enemyCollider, direction);
            float sheepEdge = GetColliderExtentAlongDirection(sheepCollider, -direction);

            // Place sheep so edges touch
            grabbedSheep.transform.position = transform.position + direction * (enemyEdge + sheepEdge);
        }
        else
        {
            // Fallback: attach to grab point
            grabbedSheep.transform.position = grabPoint.position;
        }

        // Parent sheep for smooth movement
        grabbedSheep.transform.SetParent(grabPoint, true);

        currentState = EnemyState.Dragging;
        agent.speed = dragSpeed;
        agent.isStopped = false;
    }

    // Returns half-size of collider along a direction
    private float GetColliderExtentAlongDirection(Collider col, Vector3 dir)
    {
        dir = col.transform.InverseTransformDirection(dir.normalized);
        Bounds b = col.bounds;
        Vector3 extents = b.extents;

        return Mathf.Abs(dir.x * extents.x) + Mathf.Abs(dir.y * extents.y) + Mathf.Abs(dir.z * extents.z);
    }

    // Enemy drags sheep away from player while facing the player
    private void UpdateDragging()
    {
        if (grabbedSheep == null)
        {
            currentState = EnemyState.Hunting;
            agent.speed = sprintSpeed;
            return;
        }

        Vector3 awayDir = (transform.position - player.position).normalized;
        Vector3 escapeTarget = transform.position + awayDir * dragAwayDistance;

        if (NavMesh.SamplePosition(escapeTarget, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);

        animator.SetBool("Retreating", true); // Play retreating animation
        LookAt(player.position);              // Face player while dragging
    }

    // Releases the sheep and restores Rigidbody state
    public void ReleaseGrabbedSheep()
    {
        if (grabbedSheep == null) return;

        grabbedSheep.transform.SetParent(null, true);

        if (grabbedSheepRb != null)
            grabbedSheepRb.isKinematic = grabbedSheepOriginalKinematic;

        grabbedSheep = null;
        grabbedSheepRb = null;
    }

    // Smoothly rotate enemy to face target
    private void LookAt(Vector3 targetPos)
    {
        Vector3 lookDir = (targetPos - transform.position).normalized;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
        }
    }

    // Randomize next circling direction switch time
    private void ScheduleNextDirectionSwitch()
    {
        float randomFactor = Random.Range(0.5f, 1.5f);
        nextSwitchTime = Time.time + directionSwitchInterval * randomFactor;
    }

    // Detect collisions with sheep and grab if hunting
    private void OnCollisionEnter(Collision collision)
    {
        if (currentState == EnemyState.Hunting && collision.gameObject.CompareTag("Sheep"))
        {
            GrabSheep(collision.gameObject);
        }
    }
}
