using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public enum EnemyState
{
    Idle,
    Stalking
}

public class Monster1AI : MonoBehaviour
{
    public EnemyState currentState = EnemyState.Idle;

    [Header("Player Reference")]
    public Transform player;

    [Header("Detection Settings")]
    public float detectionRange = 15f;
    public float stalkCooldown = 1f;
    public float timeToVanish = 0.3f;
    public float playerTooFarDistance = 20f;

    private EnemyState previousState;

    private FieldOfView fieldOfView;
    private NEWInCameraDetector cameraDetector;

    [Header("Animator")]
    [SerializeField] private DetectSheep sheepDetector;
    private bool _isAttackingSheep = false;
    [SerializeField] private string sheepAttackBoolName = "Attack";
    [SerializeField] private float sheepLoseDelay = 0.5f;
    private float _sheepLostTimer = 0f;
    [SerializeField] private Animator animator;
   
    private bool isTeleporting;

    // Double-event guard (animation may fire twice)
    private bool teleportEventConsumed = false;

    private Node[] allNodes;
    private Node currentNode;
    private Node lastNode; // prevent immediate repeats

    private float stalkTimer = 0f;
    private float visibleTimer = 0f;

    [Header("Audio (Optional)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip idleSound;
    [SerializeField] private AudioClip stalkingSound;
    [SerializeField] private AudioClip teleportSound;
    [SerializeField] private AudioClip reappearSound;
    [Range(0f, 1f)][SerializeField] private float sfxVolume = 0.8f;

    private Node nextTeleportTarget;

    // Idle teleport safety
    private float idleTimer = 0f;
    private float idleTeleportDelay = 10f; // seconds before auto teleport if idle too long

    // Optional: also require the target not to be currently in monster FOV
    [Header("Target Filters")]
    public bool requireOutOfPlayerView = true;
    public bool requireOutOfMonsterFOV = false;

    [Header("Teleport Cooldown")]
    [SerializeField] private float teleportCooldown = 2f;
    private float teleportCooldownTimer = 0f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        fieldOfView = GetComponent<FieldOfView>();
        cameraDetector = GetComponent<NEWInCameraDetector>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogWarning("[Monster1AI] No AudioSource found! Sounds will be skipped safely.");
            }
        }

        allNodes = FindObjectsOfType<Node>();
        if (allNodes.Length == 0)
        {
            Debug.LogError("[Monster1AI] No Node objects found in the scene!");
            enabled = false;
            return;
        }

        // Start on closest node
        currentNode = GetClosestNode(transform.position);
        if (currentNode != null)
        {
            transform.position = currentNode.transform.position; // IMPORTANT: not transform.root!
            lastNode = null;
            Debug.Log($"[Monster1AI] Starting on node: {currentNode.name} @ {transform.position}");
        }

        PlayStateSound(idleSound, loop: true);
        previousState = currentState;
    }

    private void Update()
    {
        if (previousState != currentState)
        {
            OnStateChanged(previousState, currentState);
            previousState = currentState;
        }

        idleTimer += Time.deltaTime;

        if (teleportCooldownTimer > 0f)
        {
            teleportCooldownTimer -= Time.deltaTime;
        }

        switch (currentState)
        {
            case EnemyState.Idle:
                IdleBehavior();
                break;
            case EnemyState.Stalking:
                StalkingBehavior();
                break;
        }

       
        if (idleTimer >= idleTeleportDelay && !isTeleporting)
        {
            Debug.Log("[Monster1AI] Idle teleport triggered (timeout).");
            TryTeleport(excludeNode: currentNode);
            idleTimer = 0f;
        }
        HandleSheepAttack();
    }

    private void IdleBehavior()
    {
        if (player != null && Vector3.Distance(player.position, transform.position) < detectionRange)
        {
            currentState = EnemyState.Stalking;
        }
    }

    private void StalkingBehavior()
    {
        stalkTimer -= Time.deltaTime;

        bool canSee = fieldOfView != null && fieldOfView.canSeePlayer;
        bool isVisible = cameraDetector != null && cameraDetector.IsVisible;

        if (canSee && isVisible)
        {
            visibleTimer += Time.deltaTime;
            if (visibleTimer >= timeToVanish && stalkTimer <= 0f)
            {
                Debug.Log("[Monster1AI] Player stared too long -> teleporting!");
                TryTeleport(excludeNode: currentNode);
                visibleTimer = 0f;
                stalkTimer = stalkCooldown;
            }
        }
        else
        {
            visibleTimer = 0f;
        }

        if (player != null && Vector3.Distance(player.position, transform.position) > playerTooFarDistance && stalkTimer <= 0f)
        {
            Debug.Log("[Monster1AI] Player is too far -> teleporting closer.");
            TryTeleport(excludeNode: currentNode);
            stalkTimer = stalkCooldown;
        }
    }
    private void HandleSheepAttack()
    {
        if (sheepDetector == null)
            return;

        bool sheepInRange = sheepDetector.visibleTargets.Count > 0;

        if (sheepInRange)
        {
            _sheepLostTimer = 0f;

            if (!_isAttackingSheep)
            {
                _isAttackingSheep = true;
                animator.SetBool(sheepAttackBoolName, true);
                Debug.Log("[Monster1AI] Sheep detected -> start attack.");
            }
        }
        else
        {
            if (_isAttackingSheep)
            {
                _sheepLostTimer += Time.deltaTime;

                if (_sheepLostTimer >= sheepLoseDelay)
                {
                    _isAttackingSheep = false;
                    animator.SetBool(sheepAttackBoolName, false);
                    Debug.Log("[Monster1AI] Sheep gone -> stop attack.");
                }
            }
            else
            {
                _sheepLostTimer = 0f;
            }
        }
    }

    public void FreezePose()
    {
       
        animator.Play("Attack_Scarecrow", 0, 1f);
        Debug.Log("Monster forced to final attack frame.");
    }
    // ===== Teleport selection =====

    private void TryTeleport(Node excludeNode)
    {
        // hard guards – no double / spam teleports
        if (isTeleporting || teleportCooldownTimer > 0f || nextTeleportTarget != null)
        {
            return;
        }

        if (allNodes == null || allNodes.Length == 0)
        {
            Debug.LogWarning("[Monster1AI] No nodes found! Cannot teleport.");
            return;
        }

        Node targetNode = PickBestNode(excludeNode);

        if (targetNode != null)
        {
            nextTeleportTarget = targetNode;
            Debug.Log($"[Monster1AI] Preparing to teleport from {currentNode?.name} → {nextTeleportTarget.name}");
            StartCoroutine(PlayTeleportAnimation());
        }
        else
        {
            Debug.LogWarning("[Monster1AI] Could not determine any node to teleport to.");
        }
    }

    private Node PickBestNode(Node excludeNode)
    {
        Camera cam = (cameraDetector != null) ? cameraDetector.cam : Camera.main;

        // Build candidate list
        List<Node> candidates = new List<Node>();
        foreach (var n in allNodes)
        {
            if (n == null) continue;
            if (n == excludeNode) continue;     // not the current node
            if (n == lastNode) continue;        // not the immediately previous node (prevents repeats)

            bool ok = true;

            if (requireOutOfPlayerView && cam != null)
            {
                if (IsPointVisibleToCamera(n.transform.position, cam))
                    ok = false;
            }

            if (ok && requireOutOfMonsterFOV && fieldOfView != null)
            {
                // Simple LOS from monster "eyes" to node
                Vector3 eyePos = transform.position + Vector3.up * 1.5f;
                Vector3 dir = (n.transform.position - eyePos).normalized;
                float dist = Vector3.Distance(eyePos, n.transform.position);

                // Reuse obstruction mask from FOV if set; otherwise nothing blocks
                LayerMask mask = fieldOfView.obstructionMask;
                if (!Physics.Raycast(eyePos, dir, dist, mask))
                {
                    // Node is directly visible -> reject if we require "out of monster FOV"
                    ok = false;
                }
            }

            if (ok) candidates.Add(n);
        }

        // If too strict and nothing left, relax to "anything except current"
        if (candidates.Count == 0)
        {
            foreach (var n in allNodes)
            {
                if (n != null && n != excludeNode)
                    candidates.Add(n);
            }
        }

        if (candidates.Count == 0) return null;

        // Choose the candidate closest to the player (feels smart)…
        Node best = candidates[0];
        float bestDist = (player != null)
            ? Vector3.Distance(player.position, best.transform.position)
            : Vector3.Distance(transform.position, best.transform.position);

        for (int i = 1; i < candidates.Count; i++)
        {
            float d = (player != null)
                ? Vector3.Distance(player.position, candidates[i].transform.position)
                : Vector3.Distance(transform.position, candidates[i].transform.position);

            if (d < bestDist)
            {
                best = candidates[i];
                bestDist = d;
            }
        }

        return best;
    }

    // Camera visibility check for a world point
    private bool IsPointVisibleToCamera(Vector3 worldPos, Camera cam)
    {
        if (cam == null) return false;

        Vector3 vp = cam.WorldToViewportPoint(worldPos);
        if (vp.z <= 0f) return false; // behind camera
        if (vp.x < 0f || vp.x > 1f || vp.y < 0f || vp.y > 1f) return false; // off screen

        // Optional: confirm unobstructed line of sight from camera to the point
        Ray ray = cam.ScreenPointToRay(new Vector3(vp.x * cam.pixelWidth, vp.y * cam.pixelHeight, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            // If the first thing hit is very close to the node position, we consider it "visible"
            return Vector3.Distance(hit.point, worldPos) < 0.75f;
        }
        return false;
    }

    private IEnumerator PlayTeleportAnimation()
    {
        isTeleporting = true;
        teleportEventConsumed = false; // allow exactly one event to act

        animator.SetTrigger("Teleport");

        if (audioSource != null && teleportSound != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(teleportSound, sfxVolume);
        }
        else
        {
            Debug.Log("[Monster1AI] Skipping teleport sound (no AudioSource or clip).");
        }

        Debug.Log("[Monster1AI] Teleport animation triggered.");

        // No isTeleporting reset here – we wait for OnTeleportMoment
        yield return null;

        idleTimer = 0f;
    }

    // Called by animation event (ensure there is only ONE event on the Teleport/Vanish clip)
    public void OnTeleportMoment()
    {
        if (teleportEventConsumed)
        {
            // Ignore duplicate events
            return;
        }
        teleportEventConsumed = true;

        Debug.Log($"[Monster1AI] Teleport event fired at {Time.time}");

        if (nextTeleportTarget != null)
        {
            Vector3 from = transform.position;
            Vector3 to = nextTeleportTarget.transform.position;

            // IMPORTANT: move only THIS transform, not root
            transform.position = to;

            lastNode = currentNode;
            currentNode = nextTeleportTarget;

            Debug.Log($"[Monster1AI] Moving from {from} → {to}");
            Debug.Log($"[Monster1AI] Now at node: {currentNode.name}");
        }
        else
        {
            Debug.LogWarning("[Monster1AI] TeleportMoment called but no target set!");
        }

        // teleport finished – unlock future teleports and start cooldown
        isTeleporting = false;
        teleportCooldownTimer = teleportCooldown;
        nextTeleportTarget = null;

        // reset stalking/visible timers so logic doesn't instantly re-request teleport
        visibleTimer = 0f;
        stalkTimer = stalkCooldown;
    }

    private void OnStateChanged(EnemyState oldState, EnemyState newState)
    {
        switch (newState)
        {
            case EnemyState.Idle:
                PlayStateSound(idleSound, loop: true);
                break;
            case EnemyState.Stalking:
                PlayStateSound(stalkingSound, loop: true);
                break;
        }
    }

    // === Sound Handling (safe) ===
    private void PlayStateSound(AudioClip clip, bool loop)
    {
        if (audioSource == null || clip == null)
        {
            // Silent mode OK
            return;
        }

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.volume = sfxVolume;
        audioSource.Play();
    }

    // === Node Helpers ===
    private Node GetClosestNode(Vector3 pos)
    {
        Node closest = null;
        float minDist = Mathf.Infinity;
        foreach (Node n in allNodes)
        {
            if (n == null) continue;
            float dist = Vector3.Distance(pos, n.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = n;
            }
        }
        return closest;
    }
}
