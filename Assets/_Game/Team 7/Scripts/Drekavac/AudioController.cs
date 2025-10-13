using UnityEngine;

namespace _Game.Team_7.Scripts.Drekavac
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
