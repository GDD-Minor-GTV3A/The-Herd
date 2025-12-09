using Core.AI.Sheep.Config;
using UnityEngine;
using UnityEngine.Serialization;

public class SheepSoundDriver : MonoBehaviour
{
    #region Const Footsteps

    private const float FOOTSTEPS_VOLUME = 0.7f;

    private const float MAX_TIME_BETWEEN_FOOTSTEPS = 1f;
    private const float MIN_TIME_BETWEEN_FOOTSTEPS = 0.5f;

    private const float HEIGHEST_FOOTSTEPS_PITCH = 1.2f;
    private const float LOWEST_FOOTSTEPS_PITCH = 0.8f;

    #endregion
    #region Const Bleats

    private const float BLEATS_VOLUME = 2f;

    private const float MAX_TIME_BETWEEN_BLEATS = 10f;
    private const float MIN_TIME_BETWEEN_BLEATS = 5f;

    #endregion
    private float _nextMomentBleatSound;
    private float _nextMomentWalkSound;
    
    [Header("Audio Sources")]
    [SerializeField] private AudioSource _walkingAudioSource;
    [SerializeField] private AudioSource _bleatingAudioSource;
    [FormerlySerializedAs("_miscaudioSource")] [SerializeField] private AudioSource _miscAudioSource;

    [Tooltip("Sound clip for the walking sound.")]
    [SerializeField] private AudioClip _walkingSound;

    [Header("Audio Volumes")]
    [Tooltip("Volume of the walking sound.")]
    [SerializeField] private float _walkingSoundVolume = FOOTSTEPS_VOLUME;
    [Tooltip("Volume of the bleat sound.")]
    [SerializeField] private float _bleatSoundVolume = BLEATS_VOLUME;
    
    
    public bool TryPlayWalkSound()
    {
        if (_nextMomentWalkSound > Time.time) return false;

        PlaySoundClipInternal(_walkingSound, _walkingAudioSource, FOOTSTEPS_VOLUME, Random.Range(LOWEST_FOOTSTEPS_PITCH, HEIGHEST_FOOTSTEPS_PITCH));

        _nextMomentWalkSound = Time.time + _walkingSound.length + Random.Range(MIN_TIME_BETWEEN_FOOTSTEPS, MAX_TIME_BETWEEN_FOOTSTEPS);
        return true;
    }


    public bool TryPlayBleatSound(SheepArchetype sheepArchetype)
    {
        if (_nextMomentBleatSound > Time.time) return false;
        
        AudioClip bleatSound = sheepArchetype.BleatSounds[Random.Range(0, sheepArchetype.BleatSounds.Length)];
        if (bleatSound == null) return false;

        PlaySoundClipInternal(bleatSound, _bleatingAudioSource, BLEATS_VOLUME, Random.Range(LOWEST_FOOTSTEPS_PITCH, HEIGHEST_FOOTSTEPS_PITCH));

        _nextMomentBleatSound = Time.time + bleatSound.length + Random.Range(MIN_TIME_BETWEEN_BLEATS, MAX_TIME_BETWEEN_BLEATS);
        return true;
    }

    public void PlayMiscSound(AudioClip clip, float volume = 1.0f, float pitch = 1.0f)
    {
        PlaySoundClipInternal(clip, _miscAudioSource, volume, pitch);
    }

    private static void PlaySoundClipInternal(AudioClip clip, AudioSource audioSource, float volume, float pitch)
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
