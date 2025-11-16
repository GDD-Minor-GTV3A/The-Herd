using UnityEngine;

public class DogSoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip bark;

    [Space]

    [SerializeField] private AudioClip[] ambientClips;
    [Header("Timing")]
    [SerializeField] private float minDelay = 4f;   // minimum seconds between sounds
    [SerializeField] private float maxDelay = 10f;  // maximum seconds between sounds
    private float _nextSoundTime;

    private void Start()
    {
        ScheduleNextSound();
    }

    private void Update()
    {
        if (Time.time >= _nextSoundTime)
        {
            PlayRandomAmbientSound();
            ScheduleNextSound();
        }
    }

    private void PlayRandomAmbientSound()
    {
        if (ambientClips.Length == 0) return;

        AudioClip clip = ambientClips[Random.Range(0, ambientClips.Length)];
        audioSource.PlayOneShot(clip);
    }

    private void ScheduleNextSound()
    {
        _nextSoundTime = Time.time + Random.Range(minDelay, maxDelay);
    }
    public void PlayBarkSound()
    {
        if (bark == null)
        {
            Debug.LogWarning("DamageSoundPlayer: No damage sounds assigned!");
            return;
        }
        AudioClip clip = bark;
        audioSource.PlayOneShot(clip);
    }
}
