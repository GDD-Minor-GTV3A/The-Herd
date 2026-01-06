using Core.Shared.Utilities;
using UnityEngine;

namespace Gameplay.Audio
{
    /// <summary>
    /// Manages dog's sounds playing.
    /// </summary>
    public class DogSoundManager : MonoBehaviour
    {
        [SerializeField, Tooltip("Audio source."), Required] 
        private AudioSource audioSource;

        [SerializeField, Tooltip("Bark command sound."), Required] 
        private AudioClip bark;

        [Space]
        [SerializeField, Tooltip("Audio clips for ambient sounds.")] 
        private AudioClip[] ambientClips;
        
        [Space, Header("Timings")]
        [SerializeField, Tooltip("Minimum seconds between ambient sounds.")] 
        private float minDelay = 4f;   // minimum seconds between sounds

        [SerializeField, Tooltip("Maximum seconds between ambient sounds.")] 
        private float maxDelay = 10f;  // maximum seconds between sounds


        private float nextSoundTime;


        /// <summary>
        /// Initialization method.
        /// </summary>
        public void Initialize()
        {
            ScheduleNextSound();
        }


        private void Update()
        {
            if (Time.time >= nextSoundTime)
            {
                PlayRandomAmbientSound();
                ScheduleNextSound();
            }
        }


        private void PlayRandomAmbientSound()
        {
            if (ambientClips == null || ambientClips.Length == 0) return;

            AudioClip _clip = ambientClips[Random.Range(0, ambientClips.Length)];
            audioSource.PlayOneShot(_clip);
        }


        private void ScheduleNextSound()
        {
            nextSoundTime = Time.time + Random.Range(minDelay, maxDelay);
        }


        /// <summary>
        /// Plays bark sound one time.
        /// </summary>
        public void PlayBarkSound()
        {
            if (bark == null)
            {
                Debug.LogWarning("DamageSoundPlayer: No damage sounds assigned!");
                return;
            }
            AudioClip _clip = bark;
            audioSource.PlayOneShot(_clip);
        }


        private void OnValidate()
        {
            if (minDelay > maxDelay)
                minDelay = maxDelay;
        }
    }
}