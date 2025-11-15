using System.Collections;

using UnityEngine;

namespace Project.Audio
{
    /// <summary>
    /// Handles background music with wind ambience, including fades between tracks
    /// and pauses.
    /// </summary>
    public class BackgroundMusicHandler : MonoBehaviour
    {
        [Header("Audio Sources")]
        [Tooltip("AudioSource used for music playback.")]
        [SerializeField] private AudioSource musicSource;

        [Tooltip("AudioSource used for wind ambience.")]
        [SerializeField] private AudioSource windSource;

        [Header("Music Clips")]
        [Tooltip("Playlist of music clips to play.")]
        [SerializeField] private AudioClip[] musicClips;

        [Header("Timing")]
        [Tooltip("Seconds used for fading volumes in/out.")]
        [SerializeField] private float fadeSeconds = 1.5f;

        [Tooltip("Seconds to wait with wind ambience between music tracks.")]
        [SerializeField] private float windPauseSeconds = 5f;

        [Header("Volumes")]
        [Tooltip("Music volume while a track is playing.")]
        [Range(0f, 1f)]
        [SerializeField] private float musicVolume = 1.0f;

        [Tooltip("Wind volume while music is playing.")]
        [Range(0f, 1f)]
        [SerializeField] private float windUnderMusicVolume = 0.2f;

        [Tooltip("Wind volume during the pause between tracks.")]
        [Range(0f, 1f)]
        [SerializeField] private float windDuringPauseVolume = 0.8f;

        private int currentIndex = 0;

        private void Awake()
        {
            if (!musicSource || !windSource || musicClips == null || musicClips.Length == 0)
            {
                Debug.LogError("BackgroundMusicHandler: Please assign Music Source, Wind Source, and at least one Music Clip.");
                enabled = false;
                return;
            }

            musicSource.playOnAwake = false;
            musicSource.loop = false;
            musicSource.spatialBlend = 0f;
            musicSource.volume = 0f;

            windSource.loop = true;
            windSource.spatialBlend = 0f;

            if (!windSource.isPlaying && windSource.clip)
            {
                windSource.Play();
            }

            windSource.volume = 0f;
        }

        private void Start()
        {
            StartCoroutine(MusicLoopRoutine());
        }

        private IEnumerator MusicLoopRoutine()
        {
            yield return FadeVolume(windSource, windUnderMusicVolume, fadeSeconds);

            while (true)
            {
                var _clip = musicClips[currentIndex];
                if (!_clip)
                {
                    currentIndex = (currentIndex + 1) % musicClips.Length;
                    continue;
                }

                musicSource.clip = _clip;
                musicSource.time = 0f;
                musicSource.volume = 0f;
                musicSource.Play();

                StartCoroutine(FadeVolume(windSource, windUnderMusicVolume, fadeSeconds));
                yield return FadeVolume(musicSource, musicVolume, fadeSeconds);

                bool _isFadingOut = false;

                while (musicSource.clip == _clip)
                {
                    float _length = Mathf.Max(_clip.length, fadeSeconds + 0.05f);
                    float _remaining = Mathf.Max(0f, _length - musicSource.time);

                    if (!_isFadingOut && _remaining <= fadeSeconds + 0.02f)
                    {
                        _isFadingOut = true;
                        StartCoroutine(FadeVolume(musicSource, 0f, fadeSeconds));
                        StartCoroutine(FadeVolume(windSource, windDuringPauseVolume, fadeSeconds));
                    }

                    if (_isFadingOut && (musicSource.volume <= 0.001f || !musicSource.isPlaying))
                    {
                        break;
                    }

                    yield return null;
                }

                musicSource.Stop();
                musicSource.volume = 0f;

                if (windPauseSeconds > 0f)
                {
                    yield return new WaitForSecondsRealtime(windPauseSeconds);
                }

                yield return FadeVolume(windSource, windUnderMusicVolume, fadeSeconds);

                currentIndex = (currentIndex + 1) % musicClips.Length;
            }
        }

        private IEnumerator FadeVolume(AudioSource source, float targetVolume, float durationSeconds)
        {
            if (!source || durationSeconds <= 0f)
            {
                if (source)
                {
                    source.volume = targetVolume;
                }

                yield break;
            }

            float _startVolume = source.volume;
            float _t = 0f;

            while (_t < durationSeconds)
            {
                _t += Time.unscaledDeltaTime;
                float _k = Mathf.Clamp01(_t / durationSeconds);
                source.volume = Mathf.Lerp(_startVolume, targetVolume, _k);
                yield return null;
            }

            source.volume = targetVolume;
        }
    }
}
