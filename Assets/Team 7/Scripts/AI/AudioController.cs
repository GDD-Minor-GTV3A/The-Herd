using UnityEngine;

namespace Core
{
    public class AudioController
    {
        private readonly AudioSource _audioSource;
        public AudioController(AudioSource audioSource)
        {
            _audioSource = audioSource;
        }
        
        public void PlayClip(AudioClip clip)
        {
            _audioSource.clip = clip;
            _audioSource.Play();
        }
    }
}
