using UnityEngine;
using System.Collections;

public enum EnemyState
{
    Idle,
    Stalking
}

public class Monster1AI : MonoBehaviour
{
    public EnemyState currentState = EnemyState.Idle;

    public Transform player;
    public float detectionRange = 15f;
    public float stalkCooldown = 2f;
    public float timeToVanish = 3f;
    public float playerTooFarDistance = 20f;

    private FieldOfView fieldOfView;
    private NEWInCameraDetector cameraDetector;

    [Header("Animator")]
    [SerializeField] private Animator animator;
    private bool isTeleporting;
    private Node[] allNodes;
    private Node currentNode;
    private float stalkTimer = 0f;
    private float visibleTimer = 0f;

   
    private Node nextTeleportTarget;

    void Start()
    {
        animator = GetComponent<Animator>();
        fieldOfView = GetComponent<FieldOfView>();
        cameraDetector = GetComponent<NEWInCameraDetector>();

        allNodes = FindObjectsOfType<Node>();
        currentNode = GetClosestNode(transform.position);
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                IdleBehavior();
                break;
            case EnemyState.Stalking:
                StalkingBehavior();
                break;
        }

    }

    void IdleBehavior()
    {
        if (Vector3.Distance(player.position, transform.position) < detectionRange)
        {
            currentState = EnemyState.Stalking;
        }
    }

    void StalkingBehavior()
    {
        stalkTimer -= Time.deltaTime;

        bool canSee = fieldOfView != null && fieldOfView.canSeePlayer;
        bool isVisible = cameraDetector != null && cameraDetector.IsVisible;

        if (canSee && isVisible)
        {
            visibleTimer += Time.deltaTime;
            if (visibleTimer >= timeToVanish && stalkTimer <= 0f)
            {
                TryTeleport(excludeNode: currentNode);
                visibleTimer = 0f;
                stalkTimer = stalkCooldown;
            }
        }
        else
        {
            visibleTimer = 0f;
        }

        if (Vector3.Distance(player.position, transform.position) > playerTooFarDistance && stalkTimer <= 0f)
        {
            TryTeleport(excludeNode: currentNode);
            stalkTimer = stalkCooldown;
        }
    }


    // Fail safety
    void TryTeleport(Node excludeNode)
    {
        if (isTeleporting) return; // block repeat

        Node targetNode = GetClosestNodeToPlayerExcluding(excludeNode);
        if (targetNode != null)
        {
            nextTeleportTarget = targetNode;
            StartCoroutine(PlayTeleportAnimation());
        }
    }

    private IEnumerator PlayTeleportAnimation()
    {
        isTeleporting = true;

        // play vanish animation
        animator.SetTrigger("Teleport");

        // wait until the animation finishes 
        yield return new WaitForSeconds(0.13f);

        // after vanish ends, re-enable teleporting again
        isTeleporting = false;
    }

   // Brute force anim with event
    public void OnTeleportMoment()
    {
        Debug.Log($"Teleport event fired at {Time.time}");

        if (nextTeleportTarget != null)
        {
            Debug.Log($"Before: {transform.position} → After: {nextTeleportTarget.transform.position}");
            transform.root.position = nextTeleportTarget.transform.position;
            currentNode = nextTeleportTarget;
            Debug.Log($"Now at {transform.position}");
        }
        else
        {
            Debug.LogWarning("TeleportMoment called but no target set!");
        }
    }
    Node GetClosestNode(Vector3 pos)
    {
        Node closest = null;
        float minDist = Mathf.Infinity;
        foreach (Node n in allNodes)
        {
            float dist = Vector3.Distance(pos, n.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = n;
            }
        }
        return closest;
    }

    Node GetClosestNodeToPlayerExcluding(Node excludeNode)
    {
        Node bestNode = null;
        float minDist = Mathf.Infinity;

        foreach (Node n in allNodes)
        {
            if (n == excludeNode) continue;

            float dist = Vector3.Distance(player.position, n.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                bestNode = n;
            }
        }

        return bestNode;
    }
}
