using Core.AI.Sheep.Config;

using UnityEngine;

public class SheepSoundDriver : MonoBehaviour
{
    #region Const Footsteps

    private const float FOOTSTEPS_VOLUME = 2f;

    private const float MAX_TIME_BETWEEN_FOOTSTEPS = 1f;
    private const float MIN_TIME_BETWEEN_FOOTSTEPS = 0.2f;

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


    [Tooltip("Sound clip for the walking sound.")]
    [SerializeField] private AudioClip _walkingSound;
    
    [Tooltip("Source object for sounds of the sheep.")]
    [SerializeField] public AudioSource AudioSource;
    

    public bool TryPlayWalkSound()
    {
        if (_nextMomentWalkSound > Time.time && _walkingSound == null) return false;

        SheepSoundManager.PlaySoundClip(_walkingSound, AudioSource, FOOTSTEPS_VOLUME, Random.Range(LOWEST_FOOTSTEPS_PITCH, HEIGHEST_FOOTSTEPS_PITCH));

        _nextMomentWalkSound = Time.time + Random.Range(MIN_TIME_BETWEEN_FOOTSTEPS, MAX_TIME_BETWEEN_FOOTSTEPS);
        return true;
    }


    public bool TryPlayBleatSound(SheepArchetype sheepArchetype)
    {
        if (_nextMomentBleatSound > Time.time && sheepArchetype != null) return false;
        AudioClip bleatSound = sheepArchetype.BleatSounds[Random.Range(0, sheepArchetype.BleatSounds.Length)];
        if (bleatSound == null) return false;

        SheepSoundManager.PlaySoundClip(bleatSound, AudioSource, BLEATS_VOLUME, Random.Range(LOWEST_FOOTSTEPS_PITCH, HEIGHEST_FOOTSTEPS_PITCH));

        _nextMomentBleatSound = Time.time + Random.Range(MIN_TIME_BETWEEN_BLEATS, MAX_TIME_BETWEEN_BLEATS);
        return true;
    }

}
