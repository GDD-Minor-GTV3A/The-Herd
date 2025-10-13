using UnityEngine;

public class HomingProjectile : MonoBehaviour
{
    [Header("Homing Settings")]
    public Transform target;             // Set to player at spawn
    [Range(0f, 200f)]
    public float speed = 50f;           // High speed for "near-hitscan" feel
    [Range(0f, 1000f)]
    public float rotateSpeed = 720f;    // How fast it turns toward target
    [Range(0f, 1f)]
    public float homingStrength = 1f;   // 1 = full homing, 0 = straight
    public bool usePredictiveTargeting = true; // Anticipate player movement
    [Range(0f, 2f)]
    public float predictionFactor = 0.5f; // How far ahead to predict

    [Header("Audio Settings")]
    public AudioClip hitSound;           // Sound to play on player hit
    [Range(0f, 1f)]
    public float maxVolume = 1f;        // Max volume when close
    [Range(0f, 100f)]
    public float maxDistance = 50f;     // Distance at which volume is 0

    private Vector3 lastTargetPosition;
    private Rigidbody rb;
    private AudioSource audioSource;
    private Vector3 spawnerPosition;    // Position of spawner for distance calc
    private bool isDestroying;          // Prevent multiple destruction calls

    private void Start()
    {
        // Setup collider as trigger
        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
        else Debug.LogError($"[HomingProjectile] No Collider on {gameObject.name}. Add a trigger Collider.");

        // Cache Rigidbody if present
        rb = GetComponent<Rigidbody>();

        // Setup AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = hitSound;
        audioSource.spatialBlend = 0f; // 2D sound for script-controlled volume
        audioSource.playOnAwake = false;
        audioSource.enabled = true; // Ensure AudioSource is enabled

        // Init predictive targeting
        if (target != null) lastTargetPosition = target.position;
        else Debug.LogWarning($"[HomingProjectile] No target assigned to {gameObject.name}. Projectile will move forward indefinitely.");

        // Validate audio clip
        if (hitSound == null)
            Debug.LogError($"[HomingProjectile] No hit sound assigned to {gameObject.name}. Assign an AudioClip in the Inspector.");
        else
            Debug.Log($"[HomingProjectile] Hit sound assigned: {hitSound.name}, Length: {hitSound.length}s");

        // Log spawn
        Debug.Log($"[HomingProjectile] Spawned {gameObject.name} (ID: {gameObject.GetInstanceID()}) at {Time.time}");

        // Check for other scripts
        CheckForOtherScripts();
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            // Move forward indefinitely if target is lost (infinite lifespan)
            MoveForward();
            return;
        }

        // Calculate target position with prediction
        Vector3 targetPosition = target.position;
        if (usePredictiveTargeting)
        {
            Vector3 targetVelocity = (target.position - lastTargetPosition) / Time.fixedDeltaTime;
            targetPosition += targetVelocity * predictionFactor;
            lastTargetPosition = target.position;
        }

        Vector3 direction = (targetPosition - transform.position).normalized;

        // Rotate toward target
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * homingStrength * Time.fixedDeltaTime);

        // Move forward
        MoveForward();
    }

    private void MoveForward()
    {
        if (rb != null && rb.isKinematic)
        {
            rb.MovePosition(transform.position + transform.forward * speed * Time.fixedDeltaTime);
        }
        else
        {
            transform.position += transform.forward * speed * Time.fixedDeltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isDestroying)
        {
            isDestroying = true;

            // Calculate distance from spawner to player
            float distance = Vector3.Distance(spawnerPosition, other.transform.position);
            float volume = Mathf.Lerp(maxVolume, 0f, distance / maxDistance);
            volume = Mathf.Clamp01(volume);

            // Play sound if available
            if (audioSource != null && hitSound != null && audioSource.enabled)
            {
                audioSource.volume = volume;
                audioSource.Play();
                Debug.Log($"[HomingProjectile] Hit player at {other.transform.position}. Distance from spawner: {distance:F2}m, Volume: {volume:F2}");
            }
            else
            {
                Debug.LogError($"[HomingProjectile] Cannot play sound on {gameObject.name}. AudioSource: {(audioSource != null ? "Present, Enabled: " + audioSource.enabled : "Null")}, HitSound: {hitSound}");
            }

            // Destroy after sound fully plays (or immediately if no sound)
            Destroy(gameObject, hitSound != null ? hitSound.length : 0f);
        }
    }

    public void SetTarget(Transform newTarget, Vector3 spawnerPos)
    {
        target = newTarget;
        spawnerPosition = spawnerPos;
        if (target != null) lastTargetPosition = target.position;
    }

    private void CheckForOtherScripts()
    {
        foreach (var component in GetComponents<MonoBehaviour>())
        {
            if (component != this)
            {
                Debug.LogWarning($"[HomingProjectile] Found other MonoBehaviour on {gameObject.name}: {component.GetType().Name}. Ensure it does not interfere with destruction or audio.");
            }
        }
        ParticleSystem ps = GetComponent<ParticleSystem>();
        if (ps != null)
        {
            Debug.LogWarning($"[HomingProjectile] ParticleSystem found on {gameObject.name}. Duration: {ps.main.duration}s, Stop Action: {ps.main.stopAction}. Ensure Stop Action is not set to Disable or Destroy.");
        }
        if (GetComponent<Animator>() != null)
        {
            Debug.LogWarning($"[HomingProjectile] Animator found on {gameObject.name}. Ensure animations do not interfere with destruction or audio.");
        }
    }
}