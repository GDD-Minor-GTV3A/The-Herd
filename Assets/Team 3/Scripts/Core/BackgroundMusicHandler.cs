using System.Collections;
using UnityEngine;

public class BackgroundMusicHandler : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioSource windSource;    
    public AudioClip[] musicClips;    

    public float fadeSeconds = 1.5f;

    public float windPauseSeconds = 5f;

    [Range(0f, 1f)] public float musicVolume = 1.0f;
    [Range(0f, 1f)] public float windUnderMusicVolume = 0.2f;
    [Range(0f, 1f)] public float windDuringPauseVolume = 0.8f;

    private int _index = 0;

    private void Awake()
    {
        if (!musicSource || !windSource || musicClips == null || musicClips.Length == 0)
        {
            Debug.LogError("Make sure all fields are filled");
            enabled = false; return;
        }

        musicSource.playOnAwake = false;
        musicSource.loop = false;
        musicSource.spatialBlend = 0f;
        musicSource.volume = 0f;

        windSource.loop = true;
        windSource.spatialBlend = 0f;
        if (!windSource.isPlaying && windSource.clip) windSource.Play();
        windSource.volume = 0f;
    }

    private void Start()
    {
        StartCoroutine(LoopRoutine());
    }

    private IEnumerator LoopRoutine()
    {
        yield return FadeVolume(windSource, windUnderMusicVolume, fadeSeconds);

        while (true)
        {
            var clip = musicClips[_index];
            if (!clip) { _index = (_index + 1) % musicClips.Length; continue; }

            musicSource.clip = clip;
            musicSource.time = 0f;
            musicSource.volume = 0f;
            musicSource.Play();

            StartCoroutine(FadeVolume(windSource, windUnderMusicVolume, fadeSeconds));

            yield return FadeVolume(musicSource, musicVolume, fadeSeconds);

            bool fadingOut = false;
            while (musicSource.clip == clip)
            {
                float length = Mathf.Max(clip.length, fadeSeconds + 0.05f);
                float remaining = Mathf.Max(0f, length - musicSource.time);

                if (!fadingOut && remaining <= fadeSeconds + 0.02f)
                {
                    fadingOut = true;
                    StartCoroutine(FadeVolume(musicSource, 0f, fadeSeconds));
                    StartCoroutine(FadeVolume(windSource, windDuringPauseVolume, fadeSeconds));
                }

                if (fadingOut && (musicSource.volume <= 0.001f || !musicSource.isPlaying))
                {
                    break;
                }

                yield return null;
            }

            musicSource.Stop();
            musicSource.volume = 0f;

            if (windPauseSeconds > 0f)
                yield return new WaitForSecondsRealtime(windPauseSeconds);

            yield return FadeVolume(windSource, windUnderMusicVolume, fadeSeconds);

            _index = (_index + 1) % musicClips.Length;
        }
    }

    private IEnumerator FadeVolume(AudioSource src, float target, float duration)
    {
        if (!src || duration <= 0f)
        {
            if (src) src.volume = target;
            yield break;
        }

        float start = src.volume;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / duration);
            src.volume = Mathf.Lerp(start, target, k);
            yield return null;
        }
        src.volume = target;
    }
}
