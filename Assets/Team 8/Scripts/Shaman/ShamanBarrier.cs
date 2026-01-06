using UnityEngine;
using Gameplay.Player;

[RequireComponent(typeof(BoxCollider))]
public class ShamanBarrier : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SheepCountChecker sheepCountChecker;

    [Header("Settings")]
    [SerializeField] private float maxCrossDistance = 3.5f;
    [SerializeField] private float pushbackDistance = 2f;
    [SerializeField] private float pushbackSpeed = 8f;
    [SerializeField] private float dialogueCooldown = 2f;

    private Transform playerTransform;
    private CharacterController playerController;
    private Animator playerAnimator;
    private Vector3 pushbackTarget;
    private float cooldownTimer;
    private bool playerInZone;
    private bool isPushing;
    private bool dialogueTriggered;

    private static readonly int WalkingSpeed = Animator.StringToHash("WalkingSpeed");

    private void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;
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
        if (other.CompareTag("Player"))
            playerInZone = false;
    }

    private void Update()
    {
        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;

        if (isPushing && playerController != null)
            PushBack();

        if (!playerInZone || playerTransform == null) return;

        float crossDistance = Vector3.Dot(playerTransform.position - transform.position, transform.forward);
        if (crossDistance > maxCrossDistance)
            TriggerBarrier();
    }

    private void TriggerBarrier()
    {
        bool hasEnough = sheepCountChecker != null && sheepCountChecker.HasEnough();

        if (!dialogueTriggered && cooldownTimer <= 0)
        {
            dialogueTriggered = true;
            cooldownTimer = dialogueCooldown;
            PlayerInputHandler.DisableAllPlayerActions();
            sheepCountChecker?.StartDialogue();
        }

        if (!hasEnough)
        {
            PlayerInputHandler.DisableAllPlayerActions();
            var pushDirection = -transform.forward;
            pushDirection.y = 0;
            pushbackTarget = playerTransform.position + pushDirection * pushbackDistance;
            pushbackTarget.y = playerTransform.position.y;
            isPushing = true;
        }
    }

    private void PushBack()
    {
        var direction = pushbackTarget - playerTransform.position;
        direction.y = 0;

        if (direction.magnitude < 0.5f)
        {
            isPushing = false;
            if (playerAnimator != null)
                playerAnimator.SetFloat(WalkingSpeed, 0f);
            PlayerInputHandler.EnableAllPlayerActions();
            return;
        }

        if (playerAnimator != null)
            playerAnimator.SetFloat(WalkingSpeed, 0.5f);

        var lookDirection = -transform.forward;
        lookDirection.y = 0;
        if (lookDirection != Vector3.zero)
            playerTransform.rotation = Quaternion.LookRotation(lookDirection);

        playerController.Move(direction.normalized * pushbackSpeed * Time.deltaTime);
    }

    public void Disable() => gameObject.SetActive(false);
}
