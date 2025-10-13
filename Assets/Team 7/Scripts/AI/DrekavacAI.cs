using Core.AI.Sheep;

using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.AI;

public class DrekavacAI : MonoBehaviour
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
    public float dragAwayDistance = 20f;               // Distance to retreat while dragging
    public float despawnDistance = 30f;                // Distance from player to despawn enemy while dragging

    [Header("Abort Settings")]
    public float abortDistance = 40f;                  // How far to run before despawning
    public float abortSpeed = 8f;                      // Speed while aborting
    public float playerCloseAbortDistance = 5f;        // Distance that triggers abort if player gets too close

    [Header("Animation")]
    public Animator animator;                          // Animator for controlling animations
    public AudioSource audioSource;                    // The AudioSource component
    public AudioClip Screech;
    public AudioClip Chomp;
    public AudioClip Snarl;

    private NavMeshAgent agent;                        // Reference to NavMeshAgent component
    private Transform player;                          // Reference to player transform
    private float angleOffset;                         // Angle used for circling calculations
    private int circleDirection;                       // 1 or -1 for clockwise/counterclockwise
    private float nextSwitchTime;                      // Next time to switch circling direction
    private bool isSettled = false;                    // True when enemy reached circle radius
    private float stalkEndTime;                        // When to stop stalking and start hunting

    private Vector3 circleCenter;                      // Fixed center of circling
    private Vector3 abortTarget;                       // Escape target position

    private enum EnemyState { Stalking, Hunting, Dragging, Aborting }
    private EnemyState currentState = EnemyState.Stalking;

    private GameObject grabbedSheep;                   // Currently held sheep
    private Rigidbody grabbedSheepRb;                  // Rigidbody of grabbed sheep
    private bool grabbedSheepOriginalKinematic;        // Original kinematic state to restore

    void Start()
    {
        // Play spawn screech
        audioSource.clip = Screech;
        audioSource.Play();

        // Get NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("EnemyAI: No NavMeshAgent component found.");
            enabled = false;
            return;
        }

        agent.speed = moveSpeed;

        // Find player by tag (still needed for dragging/abort)
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

        // --- Compute initial circle center from sheep ---
        GameObject[] sheepObjects = GameObject.FindGameObjectsWithTag("Sheep");
        if (sheepObjects.Length > 0)
        {
            Vector3 avgPos = Vector3.zero;
            foreach (GameObject sheep in sheepObjects)
                avgPos += sheep.transform.position;
            avgPos /= sheepObjects.Length;
            circleCenter = avgPos;
        }
        else
        {
            // Fallback to player if no sheep exist
            circleCenter = player.position;
        }

        // Position enemy at nearest point on the circle radius around the center
        Vector3 toEnemy = (transform.position - circleCenter).normalized;
        Vector3 nearestPoint = circleCenter + toEnemy * circleRadius;

        if (NavMesh.SamplePosition(nearestPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);

        // Compute initial angleOffset based on enemy relative to circle center
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
            Destroy(grabbedSheep); // IMP01 - Chris
            ReleaseGrabbedSheep();
            Destroy(gameObject);
            return;
        }

        // Check abort condition (player too close) if stalking or dragging
        if ((currentState == EnemyState.Stalking || currentState == EnemyState.Dragging) &&
            Vector3.Distance(transform.position, player.position) <= playerCloseAbortDistance)
        {
            Abort();
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
            case EnemyState.Aborting:
                UpdateAborting();
                break;
        }
    }

    private void UpdateStalking()
    {
        // Compute the average position of all sheep to use as circle center
        GameObject[] sheepObjects = GameObject.FindGameObjectsWithTag("Sheep");
        if (sheepObjects.Length > 0)
        {
            Vector3 avgPos = Vector3.zero;
            foreach (GameObject sheep in sheepObjects)
                avgPos += sheep.transform.position;
            avgPos /= sheepObjects.Length;
            circleCenter = avgPos; // Update circle center each frame
        }

        if (!isSettled)
        {
            // Always recalc nearest point dynamically in case sheep have moved
            Vector3 toEnemy = (transform.position - circleCenter).normalized;
            Vector3 nearestPoint = circleCenter + toEnemy * circleRadius;

            if (NavMesh.SamplePosition(nearestPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                agent.SetDestination(hit.position);

            float distToCenter = Vector3.Distance(transform.position, circleCenter);
            if (Mathf.Abs(distToCenter - circleRadius) < 0.5f)
            {
                isSettled = true; // Enemy reached the circle radius
                audioSource.clip = Snarl;
                audioSource.Play();
            }
            else
            {
                // Face the herd center while moving to radius
                LookAt(circleCenter);
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
        Vector3 targetPos = circleCenter + offset;

        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit2, 2f, NavMesh.AllAreas))
            agent.SetDestination(hit2.position);

        // Face the herd center while circling
        LookAt(circleCenter);

        animator.SetBool("Stalking", true);  // Play stalking animation

        if (Time.time >= stalkEndTime)
        {
            currentState = EnemyState.Hunting;
            animator.SetBool("Stalking", false);
            agent.speed = sprintSpeed;
        }
    }



    private void UpdateHunting()
    {
        if (grabbedSheep != null) return;

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

    private void GrabSheep(GameObject sheep)
    {
        if (sheep == null) return;

        // IMP02 Disable sheep's AI - Chris
        sheep.GetComponent <SheepStateManager>().enabled = false;
        sheep.GetComponent<NavMeshAgent>().enabled = false;
        // 

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

        Collider enemyCollider = GetComponent<Collider>();
        Collider sheepCollider = grabbedSheep.GetComponent<Collider>();
        if (enemyCollider != null && sheepCollider != null)
        {
            Vector3 direction = (grabbedSheep.transform.position - transform.position).normalized;

            float enemyEdge = GetColliderExtentAlongDirection(enemyCollider, direction);
            float sheepEdge = GetColliderExtentAlongDirection(sheepCollider, -direction);

            grabbedSheep.transform.position = transform.position + direction * (enemyEdge + sheepEdge);
        }
        else
        {
            grabbedSheep.transform.position = grabPoint.position;
        }

        grabbedSheep.transform.SetParent(grabPoint, true);

        currentState = EnemyState.Dragging;
        audioSource.clip = Chomp;
        audioSource.Play();
        agent.speed = dragSpeed;
        agent.isStopped = false;
    }

    private float GetColliderExtentAlongDirection(Collider col, Vector3 dir)
    {
        dir = col.transform.InverseTransformDirection(dir.normalized);
        Bounds b = col.bounds;
        Vector3 extents = b.extents;

        return Mathf.Abs(dir.x * extents.x) + Mathf.Abs(dir.y * extents.y) + Mathf.Abs(dir.z * extents.z);
    }

    private void UpdateDragging()
    {
        if (grabbedSheep == null)
        {
            currentState = EnemyState.Hunting;
            agent.speed = sprintSpeed;
            return;
        }

        // Compute average position of remaining sheep (excluding grabbed sheep)
        GameObject[] sheepObjects = GameObject.FindGameObjectsWithTag("Sheep");
        Vector3 sheepCenter = Vector3.zero;
        int count = 0;
        foreach (GameObject sheep in sheepObjects)
        {
            if (sheep != grabbedSheep)
            {
                sheepCenter += sheep.transform.position;
                count++;
            }
        }
        if (count > 0)
            sheepCenter /= count;
        else
            sheepCenter = circleCenter; // fallback if no other sheep

        // Run in opposite direction of sheep herd
        Vector3 awayDir = (transform.position - sheepCenter).normalized;
        Vector3 escapeTarget = transform.position + awayDir * dragAwayDistance;

        if (NavMesh.SamplePosition(escapeTarget, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);

        // Face backwards toward the herd (look at herd, move away)
        LookAt(sheepCenter);

        animator.SetBool("Retreating", true); // Play retreating animation
    }


    private void Abort()
    {
        // Drop sheep if dragging
        if (grabbedSheep != null)
        {
            ReleaseGrabbedSheep();
        }

        currentState = EnemyState.Aborting;
        agent.speed = abortSpeed;
        animator.SetBool("Stalking", false);
        animator.SetBool("Retreating", false);

        Vector3 awayDir = (transform.position - player.position).normalized;
        Vector3 rawTarget = transform.position + awayDir * Mathf.Max(abortDistance, circleRadius * 2f);

        // Find a valid NavMesh point in the intended direction
        if (NavMesh.SamplePosition(rawTarget, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            abortTarget = hit.position;
            agent.isStopped = false;
            agent.ResetPath();
            agent.updateRotation = false; // let LookAt handle facing
            agent.SetDestination(abortTarget);
        }
        else
        {
            Debug.LogWarning("DrekavacAI: Could not find valid abort position.");
            // If no path, fallback to despawn directly
            Destroy(gameObject);
        }
    }

    private void UpdateAborting()
    {
        if (agent.hasPath)
        {
            LookAt(agent.steeringTarget); // face where running
        }

        // Check if agent really moved and reached destination
        if (!agent.pathPending && agent.hasPath && agent.remainingDistance <= agent.stoppingDistance + 0.5f)
        {
            Destroy(gameObject);
        }
    }





    public void ReleaseGrabbedSheep()
    {
        if (grabbedSheep == null) return;

        grabbedSheep.transform.SetParent(null, true);

        if (grabbedSheepRb != null)
            grabbedSheepRb.isKinematic = grabbedSheepOriginalKinematic;

        // IMP03 Return Sheep's AI - Chris
        grabbedSheep.GetComponent<SheepStateManager>().enabled = true;
        grabbedSheep.GetComponent<NavMeshAgent>().enabled = true;
        //

        grabbedSheep = null;
        grabbedSheepRb = null;
    }

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

    private void ScheduleNextDirectionSwitch()
    {
        float randomFactor = Random.Range(0.5f, 1.5f);
        nextSwitchTime = Time.time + directionSwitchInterval * randomFactor;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (currentState == EnemyState.Hunting && collision.gameObject.CompareTag("Sheep"))
        {
            GrabSheep(collision.gameObject);
        }
    }
}