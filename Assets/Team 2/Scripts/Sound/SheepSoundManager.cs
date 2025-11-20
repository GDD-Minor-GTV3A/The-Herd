using UnityEngine;

public class SheepSoundManager : MonoBehaviour
{
    public static SheepSoundManager Instance;
    [Tooltip("Source object for sounds of sheep.")]
    [SerializeField] private AudioSource _audioSource;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void PlaySoundClip(AudioClip clip, Transform soundTransform)
    {
        PlaySoundClip(clip, soundTransform, Random.Range(0.02f, 0.2f), Random.Range(0.8f, 1.2f));
    }

    public void PlaySoundClip(AudioClip clip, Transform soundTransform, float volume, float pitch)
    {
        // Why is this a thing??
        //_audioSource = Instantiate(_audioSource, soundTransform.position, Quaternion.identity);

        _audioSource.clip = clip;
        _audioSource.volume = volume;
        _audioSource.pitch = pitch;
        _audioSource.Play();
    }
}
