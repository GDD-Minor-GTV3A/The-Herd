using UnityEngine;

namespace _Game.Team_7.Scripts.Drekavac
{
    /// <summary>
    ///     Handles playing audio for an enemy.
    /// </summary>
    public class DrekavacAudioController
    {
        private readonly AudioSource _audioSource;
        private readonly AudioClip _screech;
        private readonly AudioClip _chomp;
        private readonly AudioClip _snarl;

        public DrekavacAudioController(AudioSource audioSource, AudioClip screech, AudioClip chomp, AudioClip snarl)
        {
            _audioSource = audioSource;
            _screech = screech;
            _chomp = chomp;
            _snarl = snarl;
        }

        public void PlayScreech()
        {
            PlayClip(_screech);
        }

        public void PlayChomp()
        {
            PlayClip(_chomp);
        }

        public void PlaySnarl()
        {
            PlayClip(_snarl);
        }

        private void PlayClip(AudioClip clip)
        {
            _audioSource.clip = clip;
            _audioSource.Play();
        }
    }
}