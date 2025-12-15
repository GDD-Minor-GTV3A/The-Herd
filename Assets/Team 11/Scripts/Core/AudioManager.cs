using UnityEngine;
using UnityEngine.Audio;
using Core.Shared.Utilities;

namespace Core
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Mixer")]
        [Required][SerializeField] private AudioMixer mixer;

        [Header("Source")]
        [SerializeField] private AudioSource audioUISource;

        void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        public void PlayUIAudio(AudioClip clip)
        {
            audioUISource.clip = clip;
            audioUISource.Play();
        }

        public void PlayAudio(AudioClip clip, AudioSource source, bool loop = false)
        {
            source.clip = clip;
            source.loop = loop;
            source.Play();
        }

        public void PlayAudioAtPoint(AudioClip clip, Vector3 position, bool loop = false)
        {
            var _source = gameObject.AddComponent<AudioSource>();

            AudioSource source = Instantiate(_source, position, Quaternion.identity);

            source.loop = loop;
            source.clip = clip;
            source.Play();
        }

        public void StopLoopedAudio(AudioSource source)
        {
            source.loop = false;
            source.Stop();
        }
    }
}
