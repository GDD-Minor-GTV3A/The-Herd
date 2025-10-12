using UnityEngine;
using UnityEngine.Audio;

namespace Core.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Mixer")]
        [SerializeField] private AudioMixer audioMixer;

        [Header("References")]
        [SerializeField] private AudioSource sourceUI;

        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        /// <summary>
        /// Play UI sound one-shot
        /// </summary>
        /// <param name="clip">Clip to play</param>
        public void PlayUISound(AudioClip clip)
        {
            sourceUI.clip = clip;
            sourceUI.Play();
        }
        /// <summary>
        /// Play a sound effect at source position one-shot
        /// </summary>
        /// <param name="clip">Clip to play</param>
        /// <param name="source">Source of the sound</param>
        public void PlaySoundFX(AudioClip clip, AudioSource source)
        {
            source.clip = clip;
            source.Play();
        }
        /// <summary>
        /// Play a sound effect at position one-shot
        /// </summary>
        /// <param name="clip">Clip to play</param>
        /// <param name="transform">Position of the sound</param>
        public void PlaySoundFX(AudioClip clip, Transform transform)
        {
            AudioSource audioSource = new AudioSource()
            {
                playOnAwake = false,
                loop = false,
                spatialBlend = 1f
            };
            var _source = Instantiate(audioSource, transform.position, Quaternion.identity);

            PlaySoundFX(clip, _source);
        }

        public void PlayMusic() { }
        public void PlayAmbienceMusic() { }
    }
}
