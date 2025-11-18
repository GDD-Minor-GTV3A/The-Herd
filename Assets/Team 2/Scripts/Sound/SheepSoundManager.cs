using UnityEngine;

public class SheepSoundManager : MonoBehaviour
{
    private static SheepSoundManager _instance;

    private void Awake()
    {
        _instance = this;
    }

    public static void PlaySoundClip(AudioClip clip, AudioSource audioSource)
    {
        _instance?.PlaySoundClipInternal(clip, audioSource, Random.Range(0.02f, 0.2f), Random.Range(0.8f, 1.2f));
    }

    public static void PlaySoundClip(AudioClip clip, AudioSource audioSource, float volume, float pitch)
    {
        _instance?.PlaySoundClipInternal(clip, audioSource, volume, pitch);
    }

    public static void PlaySoundClip(AudioClip clip, SheepSoundDriver soundDriver)
    {
        _instance?.PlaySoundClipInternal(clip, soundDriver.AudioSource, Random.Range(0.02f, 0.2f), Random.Range(0.8f, 1.2f));
    }

    public static void PlaySoundClip(AudioClip clip, SheepSoundDriver soundDriver, float volume, float pitch)
    {
        _instance?.PlaySoundClipInternal(clip, soundDriver.AudioSource, volume, pitch);
    }

    private void PlaySoundClipInternal(AudioClip clip, AudioSource audioSource, float volume, float pitch)
    {
        if (audioSource == null)
        {
            Debug.LogWarning("[SheepSoundManager] No AudioSource found."); 
            return;
        }

        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.pitch = pitch;

        audioSource.Play();
    }
}
