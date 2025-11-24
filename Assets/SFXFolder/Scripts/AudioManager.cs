using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource sfxSource2;

    [Header("3D Sound Boxes")]
    public AudioSource[] soundBoxes; 

    [Header("Audio Clips")]
    public AudioClip[] sfxClips;
    public AudioClip windClip;
    public AudioClip IntroMusic;
    public AudioClip Candle;

    [Header("Settings")]
    [Range(0f, 1f)]
    public float windVolume = 0.5f;
    public float IntroVolume = 0.5f;
    public float CandleVolume = 2f;

    public GameObject EndTrigger;

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
        if (windClip != null && sfxSource != null)
        {
            sfxSource.clip = windClip;
            sfxSource.volume = windVolume;
            sfxSource.loop = true;
            sfxSource.Play();
        }

        if (IntroMusic != null && musicSource != null)
        {
            musicSource.clip = IntroMusic;
            musicSource.volume = IntroVolume;
            musicSource.loop = false;
            musicSource.Play();
        }

        if (Candle != null && sfxSource2 != null)
        {
            sfxSource2.clip = Candle;
            sfxSource2.volume = CandleVolume;
            sfxSource2.loop = true;
            sfxSource2.Play();
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

    public void PlayFadeOut()
    {
        StartCoroutine(FadeOutMusic(5f));
    }

    public void PlayCandleBlowOut()
    {
        StartCoroutine(PlayCandle(1f));
    }

    public IEnumerator PlayCandle(float duration)
    {
        float startVolume = sfxSource2.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            sfxSource2.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        sfxSource2.volume = 0f;
        sfxSource2.Stop();
    }

    public IEnumerator FadeOutMusic(float duration)
    {
        float startVolume = musicSource.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
            Debug.Log("Fade out begins");
        }

        musicSource.volume = 0f;
        musicSource.Stop();
    }
}