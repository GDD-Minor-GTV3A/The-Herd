using UnityEngine;

public class ShamanAnimationHandler : MonoBehaviour
{
    [Header("Animator")]
    public Animator shamanAnimator;

    [Header("Patrol Settings")]
    public float patrolDistance = 5f;
    public float speed = 2f;
    public float turnSpeed = 180f;

    [Header("Audio Settings")]
    public AudioSource shamanAudioSource;   
    public AudioClip walkingClip;           
    public AudioClip talkingClip;           

    private Vector3 startPos;
    private Vector3 endPos;

    private bool movingForward = true;
    private bool isTurning = false;
    private float turnAngle = 0f;

    void Start()
    {
        startPos = transform.position;
        endPos = startPos + transform.forward * patrolDistance;
    }

    void Update()
    {
        bool isTalking = shamanAnimator.GetBool("isTalking");

        if (isTalking)
        {
            Idle();
            PlayAudio(talkingClip, true); 
            return;
        }
        else
        {
            PlayAudio(walkingClip, true); // loop walking clip while moving
        }

        if (isTurning)
            Turn();
        else
            Move();
    }

    // ---------------- MOVEMENT ----------------

    void Move()
    {
        Vector3 target = movingForward ? endPos : startPos;
        Vector3 direction = (target - transform.position);
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                8f * Time.deltaTime
            );
        }

        transform.position += transform.forward * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            isTurning = true;
            turnAngle = 0f;
        }
    }

    void Turn()
    {
        float step = turnSpeed * Time.deltaTime;
        transform.Rotate(0f, step, 0f);
        turnAngle += step;

        if (turnAngle >= 180f)
        {
            isTurning = false;
            movingForward = !movingForward;
        }
    }

    // ---------------- IDLE ----------------

    void Idle()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        Vector3 lookDir = player.transform.position - transform.position;
        lookDir.y = 0f;

        if (lookDir.sqrMagnitude < 0.01f) return;

        Quaternion lookRot = Quaternion.LookRotation(lookDir);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRot,
            5f * Time.deltaTime
        );
    }

    // ---------------- AUDIO ----------------

    void PlayAudio(AudioClip clip, bool loop)
    {
        if (shamanAudioSource == null || clip == null) return;

        if (shamanAudioSource.clip == clip && shamanAudioSource.isPlaying) return;

        shamanAudioSource.Stop();
        shamanAudioSource.clip = clip;
        shamanAudioSource.loop = loop;
        shamanAudioSource.Play();
    }
}
