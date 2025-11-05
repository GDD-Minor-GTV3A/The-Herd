using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("3D Sound Boxes")]
    public AudioSource[] soundBoxes; // Assign these in Inspector, place them in the scene

    [Header("Audio Clips")]
    public AudioClip[] sfxClips;
    public AudioClip windClip;

    [Header("Settings")]
    [Range(0f, 1f)]
    public float windVolume = 0.5f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (windClip != null && musicSource != null)
        {
            musicSource.clip = windClip;
            musicSource.volume = windVolume;
            musicSource.loop = true;
            musicSource.Play();
        }

        // Ensure sound boxes are properly set for 3D
        foreach (var box in soundBoxes)
        {
            if (box != null)
            {
                box.spatialBlend = 1f;   // Full 3D
                box.rolloffMode = AudioRolloffMode.Logarithmic;
                box.minDistance = 2f;   // tweak for how close before full volume
                box.maxDistance = 20f;  // tweak for how far until inaudible
            }
        }
    }

    // Play 2D SFX (UI clicks, global sounds, etc.)
    public void PlaySFX(int index, float volume = 1f)
    {
        if (sfxClips == null || index < 0 || index >= sfxClips.Length) return;
        if (sfxClips[index] == null || sfxSource == null) return;

        sfxSource.PlayOneShot(sfxClips[index], volume);
    }

    // Play 3D SFX from a sound box
    public void PlaySFXAtBox(int clipIndex, int boxIndex, float volume = 1f)
    {
        if (sfxClips == null || clipIndex < 0 || clipIndex >= sfxClips.Length) return;
        if (soundBoxes == null || boxIndex < 0 || boxIndex >= soundBoxes.Length) return;

        AudioSource box = soundBoxes[boxIndex];
        if (box == null) return;

        box.PlayOneShot(sfxClips[clipIndex], volume);
    }
}