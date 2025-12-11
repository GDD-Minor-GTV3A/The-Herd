using UnityEngine;
using Gameplay.Player;

/// <summary>
/// Creates an invisible barrier that prevents the player from progressing
/// until they have gathered enough sheep. If the player crosses the barrier
/// by more than the allowed distance, triggers dialogue and pushes them back.
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class ShamanBarrier : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SheepCountChecker sheepCountChecker;

    [Header("Barrier Settings")]
    [SerializeField] private float maxCrossDistance = 3.5f;
    [SerializeField] private float pushbackDistance = 2f;
    [SerializeField] private float pushbackSpeed = 8f;
    [SerializeField] private float dialogueCooldown = 2f;

    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;

    private BoxCollider barrierCollider;
    private Transform playerTransform;
    private CharacterController playerController;
    private Animator playerAnimator;
    private bool playerInZone;
    private bool isPushingBack;
    private Vector3 pushbackTarget;
    private bool dialogueTriggered;
    private float cooldownTimer;

    private static readonly int WalkingSpeedParam = Animator.StringToHash("WalkingSpeed");

    private void Awake()
    {
        barrierCollider = GetComponent<BoxCollider>();
        barrierCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerTransform = other.transform;
        playerController = other.GetComponent<CharacterController>();
        playerAnimator = other.GetComponentInChildren<Animator>();
        playerInZone = true;
        dialogueTriggered = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInZone = false;
    }

    private void Update()
    {
        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;

        if (isPushingBack && playerController != null)
            PushPlayerBack();

        if (!playerInZone || playerTransform == null) return;

        float crossDistance = CalculateCrossDistance();

        if (crossDistance > maxCrossDistance)
        {
            bool hasEnoughSheep = sheepCountChecker != null && sheepCountChecker.HasEnoughSheep();
            TriggerBarrier(hasEnoughSheep);
        }
    }

    /// <summary>
    /// Calculates how far past the barrier the player has crossed.
    /// </summary>
    private float CalculateCrossDistance()
    {
        Vector3 toPlayer = playerTransform.position - transform.position;
        return Vector3.Dot(toPlayer, transform.forward);
    }

    /// <summary>
    /// Triggers the barrier dialogue and initiates pushback if player doesn't have enough sheep.
    /// </summary>
    private void TriggerBarrier(bool hasEnoughSheep)
    {
        if (!dialogueTriggered && cooldownTimer <= 0)
        {
            dialogueTriggered = true;
            cooldownTimer = dialogueCooldown;

            if (sheepCountChecker != null)
                sheepCountChecker.StartDialogueBasedOnSheepCount();
        }

        if (!hasEnoughSheep)
        {
            PlayerInputHandler.DisableAllPlayerActions();

            Vector3 pushDirection = -transform.forward;
            pushDirection.y = 0;
            pushbackTarget = playerTransform.position + pushDirection * pushbackDistance;
            pushbackTarget.y = playerTransform.position.y;

            isPushingBack = true;
        }
    }

    /// <summary>
    /// Smoothly pushes the player back behind the barrier with walk animation.
    /// </summary>
    private void PushPlayerBack()
    {
        if (playerController == null) return;

        Vector3 direction = pushbackTarget - playerTransform.position;
        direction.y = 0;

        if (direction.magnitude < 0.5f)
        {
            isPushingBack = false;

            if (playerAnimator != null)
                playerAnimator.SetFloat(WalkingSpeedParam, 0f);

            PlayerInputHandler.EnableAllPlayerActions();
            return;
        }

        if (playerAnimator != null)
            playerAnimator.SetFloat(WalkingSpeedParam, 0.5f);

        Vector3 lookDirection = -transform.forward;
        lookDirection.y = 0;
        if (lookDirection != Vector3.zero)
            playerTransform.rotation = Quaternion.LookRotation(lookDirection);

        Vector3 movement = direction.normalized * pushbackSpeed * Time.deltaTime;
        playerController.Move(movement);
    }

    /// <summary>
    /// Returns whether the barrier is currently blocking the player.
    /// </summary>
    public bool IsBlocking()
    {
        return sheepCountChecker == null || !sheepCountChecker.HasEnoughSheep();
    }

    /// <summary>
    /// Disables the barrier GameObject.
    /// </summary>
    public void DisableBarrier()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Draws visual debug lines for the barrier in the Scene view.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;

        Vector3 center = transform.position;
        Vector3 right = transform.right;

        BoxCollider col = GetComponent<BoxCollider>();
        float width = col != null ? col.size.x * transform.lossyScale.x : 10f;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(center - right * width / 2, center + right * width / 2);

        Gizmos.color = Color.yellow;
        Vector3 maxCrossPos = center + transform.forward * maxCrossDistance;
        Gizmos.DrawLine(maxCrossPos - right * width / 2, maxCrossPos + right * width / 2);

        Gizmos.color = Color.green;
        Vector3 pushbackPos = center - transform.forward * pushbackDistance;
        Gizmos.DrawLine(pushbackPos - right * width / 2, pushbackPos + right * width / 2);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(center, transform.forward * 2f);
    }
}
