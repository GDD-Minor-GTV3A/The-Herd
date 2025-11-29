using UnityEngine;
using UnityEngine.AI;

// Put this on the SAME object that has the Animator (Armature).
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class AmalgamationFootsteps : MonoBehaviour
{
    [Header("Clips")]
    public AudioClip[] footstepClips;

    [Header("Distance / Volume")]
    [Tooltip("Player starts hearing steps when enemy is this close.")]
    public float startAudibleDistance = 80f;
    [Tooltip("At or closer than this distance, steps are at max volume.")]
    public float fullVolumeDistance = 3f;
    public float maxVolume = 1f;

    [Header("Speed Influence")]
    [Tooltip("Agent speed at which footsteps are considered 'full speed'.")]
    public float maxSpeedForFullVolume = 8f;
    public Vector2 pitchRandomRange = new Vector2(0.9f, 1.1f);

    private AudioSource audioSource;
    private NavMeshAgent agent;
    private Transform player;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 1f;   // 3D sound

        // Get NavMeshAgent & player from parent (root enemy)
        agent = GetComponentInParent<NavMeshAgent>();

        var sm = GetComponentInParent<AmalgamationStateMachine>();
        if (sm != null)
            player = sm.player;
    }

    // Called from Animation Event on footstep frames
    public void Footstep()
    {
        if (footstepClips == null || footstepClips.Length == 0) return;
        if (audioSource == null) return;

        // If we're basically not moving, don't play a step
        if (agent != null && agent.velocity.sqrMagnitude < 0.01f)
            return;

        // Distance-based audibility
        float distance = Mathf.Infinity;
        if (player != null)
            distance = Vector3.Distance(transform.position, player.position);

        // Too far away: no sound at all
        if (distance > startAudibleDistance)
            return;

        // dist01 = 0 when far edge, 1 when at fullVolumeDistance or closer
        float dist01 = Mathf.InverseLerp(startAudibleDistance, fullVolumeDistance, distance);

        // Base volume grows as we get closer
        float baseVolume = dist01;

        // Speed influence (faster = a bit louder / heavier)
        float speed01 = 0f;
        if (agent != null && maxSpeedForFullVolume > 0.01f)
        {
            float currentSpeed = agent.velocity.magnitude;
            speed01 = Mathf.Clamp01(currentSpeed / maxSpeedForFullVolume);
        }

        // Blend between quieter walking and loud sprinting
        float volume = baseVolume * Mathf.Lerp(0.6f, 1f, speed01);
        volume = Mathf.Clamp(volume, 0f, maxVolume);

        // Random clip + pitch so it doesn't sound robotic
        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
        audioSource.pitch = Random.Range(pitchRandomRange.x, pitchRandomRange.y);
        audioSource.PlayOneShot(clip, volume);
    }
}
