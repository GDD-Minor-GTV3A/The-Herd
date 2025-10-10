using System.Collections.Generic;

using UnityEngine;
using Core.Events;


namespace Core.Audio
{
    /// <summary>
    /// Centralized audio manager that handles sound and music playback using object pooling.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        [SerializeField][Tooltip("Configuration data for audio clips.")]
        private List<AudioClipData> _audioClipDataList = new List<AudioClipData>();

        [SerializeField][Tooltip("Master volume for SFX (0-1).")]
        [Range(0f, 1f)]
        private float _sfxVolume = 1f;

        [SerializeField][Tooltip("Master volume for music (0-1).")]
        [Range(0f, 1f)]
        private float _musicVolume = 1f;


        private static AudioManager _instance;
        private Dictionary<string, AudioClipData> _audioClipDataMap;
        private Queue<AudioSource> _sfxAudioSourcePool;
        private Queue<AudioSource> _musicAudioSourcePool;
        private AudioSource _currentMusicSource;
        private const int SFX_POOL_SIZE = 10;
        private const int MUSIC_POOL_SIZE = 2;


        /// <summary>
        /// Singleton instance of the AudioManager.
        /// </summary>
        public static AudioManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<AudioManager>();
                    if (_instance == null)
                    {
                        GameObject audioManagerObject = new GameObject("AudioManager");
                        _instance = audioManagerObject.AddComponent<AudioManager>();
                        DontDestroyOnLoad(audioManagerObject);
                    }
                }
                return _instance;
            }
        }


        /// <summary>
        /// Initializes the audio manager and sets up audio source pools.
        /// </summary>
        public void Initialize()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }

            SetupAudioClipDataMap();
            SetupAudioSourcePools();
            SubscribeToEvents();
        }


        /// <summary>
        /// Plays a sound effect by its ID.
        /// </summary>
        /// <param name="soundId">The ID of the sound to play.</param>
        public void PlaySound(string soundId)
        {
            if (!_audioClipDataMap.TryGetValue(soundId, out AudioClipData audioData))
            {
                Debug.LogWarning($"Audio clip data not found for sound ID: {soundId}");
                return;
            }

            if (audioData.Loop)
            {
                Debug.LogWarning($"Sound '{soundId}' is configured to loop. Use PlayMusic() for looping audio.");
                return;
            }

            AudioSource audioSource = GetAvailableSFXAudioSource();
            if (audioSource == null)
            {
                Debug.LogWarning("No available SFX AudioSource in pool.");
                return;
            }

            PlayAudioClip(audioSource, audioData, _sfxVolume);
        }


        /// <summary>
        /// Plays background music by its ID.
        /// </summary>
        /// <param name="musicId">The ID of the music to play.</param>
        public void PlayMusic(string musicId)
        {
            if (!_audioClipDataMap.TryGetValue(musicId, out AudioClipData audioData))
            {
                Debug.LogWarning($"Audio clip data not found for music ID: {musicId}");
                return;
            }

            StopMusic();

            AudioSource audioSource = GetAvailableMusicAudioSource();
            if (audioSource == null)
            {
                Debug.LogWarning("No available Music AudioSource in pool.");
                return;
            }

            _currentMusicSource = audioSource;
            PlayAudioClip(audioSource, audioData, _musicVolume);
        }


        /// <summary>
        /// Stops the currently playing music.
        /// </summary>
        public void StopMusic()
        {
            if (_currentMusicSource != null && _currentMusicSource.isPlaying)
            {
                _currentMusicSource.Stop();
                ReturnMusicAudioSource(_currentMusicSource);
                _currentMusicSource = null;
            }
        }


        /// <summary>
        /// Sets the master volume for SFX.
        /// </summary>
        /// <param name="volume">Volume level (0-1).</param>
        public void SetSFXVolume(float volume)
        {
            _sfxVolume = Mathf.Clamp01(volume);
        }


        /// <summary>
        /// Sets the master volume for music.
        /// </summary>
        /// <param name="volume">Volume level (0-1).</param>
        public void SetMusicVolume(float volume)
        {
            _musicVolume = Mathf.Clamp01(volume);
            if (_currentMusicSource != null)
            {
                _currentMusicSource.volume = _musicVolume * _audioClipDataMap[_currentMusicSource.clip.name].Volume;
            }
        }


        private void SetupAudioClipDataMap()
        {
            _audioClipDataMap = new Dictionary<string, AudioClipData>();
            foreach (AudioClipData audioData in _audioClipDataList)
            {
                if (audioData != null && audioData.AudioClip != null)
                {
                    _audioClipDataMap[audioData.AudioClip.name] = audioData;
                }
            }
        }


        private void SetupAudioSourcePools()
        {
            _sfxAudioSourcePool = new Queue<AudioSource>();
            _musicAudioSourcePool = new Queue<AudioSource>();

            for (int i = 0; i < SFX_POOL_SIZE; i++)
            {
                AudioSource audioSource = CreateAudioSource("SFX_" + i);
                audioSource.loop = false;
                _sfxAudioSourcePool.Enqueue(audioSource);
            }

            for (int i = 0; i < MUSIC_POOL_SIZE; i++)
            {
                AudioSource audioSource = CreateAudioSource("Music_" + i);
                audioSource.loop = true;
                _musicAudioSourcePool.Enqueue(audioSource);
            }
        }


        private AudioSource CreateAudioSource(string name)
        {
            GameObject audioObject = new GameObject(name);
            audioObject.transform.SetParent(transform);
            AudioSource audioSource = audioObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            return audioSource;
        }


        private AudioSource GetAvailableSFXAudioSource()
        {
            if (_sfxAudioSourcePool.Count > 0)
            {
                return _sfxAudioSourcePool.Dequeue();
            }
            return null;
        }


        private AudioSource GetAvailableMusicAudioSource()
        {
            if (_musicAudioSourcePool.Count > 0)
            {
                return _musicAudioSourcePool.Dequeue();
            }
            return null;
        }


        private void ReturnSFXAudioSource(AudioSource audioSource)
        {
            if (audioSource != null)
            {
                audioSource.Stop();
                audioSource.clip = null;
                _sfxAudioSourcePool.Enqueue(audioSource);
            }
        }


        private void ReturnMusicAudioSource(AudioSource audioSource)
        {
            if (audioSource != null)
            {
                audioSource.Stop();
                audioSource.clip = null;
                _musicAudioSourcePool.Enqueue(audioSource);
            }
        }


        private void PlayAudioClip(AudioSource audioSource, AudioClipData audioData, float masterVolume)
        {
            audioSource.clip = audioData.AudioClip;
            audioSource.volume = masterVolume * audioData.Volume;
            audioSource.pitch = Random.Range(audioData.MinPitch, audioData.MaxPitch);
            audioSource.loop = audioData.Loop;
            audioSource.Play();

            if (!audioData.Loop)
            {
                StartCoroutine(ReturnSFXAudioSourceWhenFinished(audioSource));
            }
        }


        private System.Collections.IEnumerator ReturnSFXAudioSourceWhenFinished(AudioSource audioSource)
        {
            yield return new WaitForSeconds(audioSource.clip.length);
            ReturnSFXAudioSource(audioSource);
        }


        private void SubscribeToEvents()
        {
            EventManager.AddListener<PlaySoundEvent>(OnPlaySoundEvent);
            EventManager.AddListener<PlayMusicEvent>(OnPlayMusicEvent);
            EventManager.AddListener<StopMusicEvent>(OnStopMusicEvent);
            EventManager.AddListener<SetSFXVolumeEvent>(OnSetSFXVolumeEvent);
            EventManager.AddListener<SetMusicVolumeEvent>(OnSetMusicVolumeEvent);
        }


        private void OnPlaySoundEvent(PlaySoundEvent evt)
        {
            PlaySound(evt.SoundId);
        }


        private void OnPlayMusicEvent(PlayMusicEvent evt)
        {
            PlayMusic(evt.MusicId);
        }


        private void OnStopMusicEvent(StopMusicEvent evt)
        {
            StopMusic();
        }


        private void OnSetSFXVolumeEvent(SetSFXVolumeEvent evt)
        {
            SetSFXVolume(evt.Volume);
        }


        private void OnSetMusicVolumeEvent(SetMusicVolumeEvent evt)
        {
            SetMusicVolume(evt.Volume);
        }


        private void OnDestroy()
        {
            EventManager.RemoveListener<PlaySoundEvent>(OnPlaySoundEvent);
            EventManager.RemoveListener<PlayMusicEvent>(OnPlayMusicEvent);
            EventManager.RemoveListener<StopMusicEvent>(OnStopMusicEvent);
            EventManager.RemoveListener<SetSFXVolumeEvent>(OnSetSFXVolumeEvent);
            EventManager.RemoveListener<SetMusicVolumeEvent>(OnSetMusicVolumeEvent);
        }
    }
}
