using System.Collections;
using System.Collections.Generic;

using Core.AI.Sheep.Event;
using Core.AI.Sheep;
using Core.Events;

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
    public float stalkCooldown = 0.1f;
    public float timeToVanish = 0.1f;
    public float playerTooFarDistance = 20f;

    private EnemyState previousState;

    private FieldOfView fieldOfView;
    private NEWInCameraDetector cameraDetector;

    [Header("Animator / Attack")]
    [SerializeField] private DetectSheep sheepDetector;
    [SerializeField] private Animator animator;
    [SerializeField] private string sheepAttackBoolName = "Attack";
    [SerializeField] private string attackStateName = "Attack_Scarecrow"; // <- Animator state name
    [SerializeField] private float sheepLoseDelay = 0.5f;
    [SerializeField] private float _scareAmount = 1f;

    private bool _isAttackingSheep = false;
    private float _sheepLostTimer = 0f;

    [Header("VFX")]
    [SerializeField] private ScarecrowVFX vfx;

    private bool isTeleporting;
    private bool teleportEventConsumed = false;

    private Node[] allNodes;
    private Node currentNode;
    private Node lastNode;

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

    [Header("Idle Teleport")]
    [SerializeField] private float idleTeleportDelay = 10f;
    private float idleTimer = 0f;

    [Header("Target Filters")]
    public bool requireOutOfPlayerView = true;
    public bool requireOutOfMonsterFOV = false;

    [Header("Teleport Cooldown")]
    [SerializeField] private float teleportCooldown = 2f;
    private float teleportCooldownTimer = 0f;

    private void Start()
    {
        if (animator == null)
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

        currentNode = GetClosestNode(transform.position);
        if (currentNode != null)
        {
            transform.position = currentNode.transform.position;
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

        // Only count idle time when not in attack state
        if (!IsInAttackState())
        {
            idleTimer += Time.deltaTime;
        }

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

        // Idle teleport
        if (idleTimer >= idleTeleportDelay && !isTeleporting && !IsInAttackState())
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
                TryTeleport(excludeNode: currentNode, allowDuringAttack: true); // <- key change
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
            TryTeleport(excludeNode: currentNode, allowDuringAttack: false); // blocked while attacking
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

            foreach (Transform t in sheepDetector.visibleTargets)
            {
                if (t.TryGetComponent<SheepStateManager>(out var sheepStateManager))
                {
                    EventManager.Broadcast(new SheepScareEvent(
                        sheepStateManager,
                        _scareAmount,         // You must define this field
                        transform.position    // Scarecrow position
                    ));
                }
            }


            if (!_isAttackingSheep)
            {
                _isAttackingSheep = true;
                animator.SetBool(sheepAttackBoolName, true);

                if (vfx != null)
                    vfx.TriggerVFX();

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
        // Make sure this matches your attack state/clip name
        animator.Play(attackStateName, 0, 1f);
        Debug.Log("Monster forced to final attack frame.");
    }

    // ===== Teleport selection =====

    private void TryTeleport(Node excludeNode, bool allowDuringAttack = false)
    {
        
        if (!allowDuringAttack && IsInAttackState())
            return;

        if (isTeleporting || teleportCooldownTimer > 0f || nextTeleportTarget != null)
            return;

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

        List<Node> candidates = new List<Node>();
        foreach (var n in allNodes)
        {
            if (n == null) continue;
            if (n == excludeNode) continue;
            if (n == lastNode) continue;

            bool ok = true;

            if (requireOutOfPlayerView && cam != null)
            {
                if (IsPointVisibleToCamera(n.transform.position, cam))
                    ok = false;
            }

            if (ok && requireOutOfMonsterFOV && fieldOfView != null)
            {
                Vector3 eyePos = transform.position + Vector3.up * 1.5f;
                Vector3 dir = (n.transform.position - eyePos).normalized;
                float dist = Vector3.Distance(eyePos, n.transform.position);

                LayerMask mask = fieldOfView.obstructionMask;
                if (!Physics.Raycast(eyePos, dir, dist, mask))
                {
                    ok = false;
                }
            }

            if (ok) candidates.Add(n);
        }

        if (candidates.Count == 0)
        {
            foreach (var n in allNodes)
            {
                if (n != null && n != excludeNode)
                    candidates.Add(n);
            }
        }

        if (candidates.Count == 0) return null;

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

    private bool IsPointVisibleToCamera(Vector3 worldPos, Camera cam)
    {
        if (cam == null) return false;

        Vector3 vp = cam.WorldToViewportPoint(worldPos);
        if (vp.z <= 0f) return false;
        if (vp.x < 0f || vp.x > 1f || vp.y < 0f || vp.y > 1f) return false;

        Ray ray = cam.ScreenPointToRay(new Vector3(vp.x * cam.pixelWidth, vp.y * cam.pixelHeight, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            return Vector3.Distance(hit.point, worldPos) < 0.75f;
        }
        return false;
    }

    private IEnumerator PlayTeleportAnimation()
    {
        isTeleporting = true;
        teleportEventConsumed = false;

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

        yield return null;

        idleTimer = 0f;
    }

    public void OnTeleportMoment()
    {
        if (teleportEventConsumed)
            return;

        teleportEventConsumed = true;

        Debug.Log($"[Monster1AI] Teleport event fired at {Time.time}");

        if (nextTeleportTarget != null)
        {
            Vector3 from = transform.position;
            Vector3 to = nextTeleportTarget.transform.position;

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

        isTeleporting = false;
        teleportCooldownTimer = teleportCooldown;
        nextTeleportTarget = null;

        visibleTimer = 0f;
        stalkTimer = stalkCooldown;
    }

    private bool IsInAttackState()
    {
        if (animator == null) return false;

        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        return info.IsName(attackStateName);
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

    private void PlayStateSound(AudioClip clip, bool loop)
    {
        if (audioSource == null || clip == null)
            return;

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.volume = sfxVolume;
        audioSource.Play();
    }

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
