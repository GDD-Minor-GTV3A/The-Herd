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

    public void PlaySoundClip(AudioClip clip, AudioSource source)
    {
        PlaySoundClip(clip, source, Random.Range(0.02f, 0.2f), Random.Range(0.8f, 1.2f));
    }

    public void PlaySoundClip(AudioClip clip, AudioSource source, float volume, float pitch)
    {
        // Why is this a thing??
        //_audioSource = Instantiate(_audioSource, soundTransform.position, Quaternion.identity);
        if (source == null || clip == null) return;
        
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.Play();
    }
}
