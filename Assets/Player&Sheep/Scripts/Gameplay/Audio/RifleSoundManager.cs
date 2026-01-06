using Core.Shared.Utilities;
using UnityEngine;

namespace Gameplay.Audio 
{
    /// <summary>
    /// Manages rifle's sounds playing.
    /// </summary>
    public class RifleSoundManager : MonoBehaviour
    {
        [SerializeField, Tooltip("Audio source of rifle."), Required] 
        private AudioSource audioSource;

        [SerializeField, Tooltip("Audio clip of shot sound."), Required] 
        private AudioClip shotClip;

        [SerializeField, Tooltip("Audio clip of reload sound."), Required] 
        private AudioClip reloadClip;


        /// <summary>
        /// Plays shot sound.
        /// </summary>
        public void PlayShot()
        {
            if (audioSource != null && shotClip != null)
            {
                audioSource.PlayOneShot(shotClip);
            }
            else
            {
                Debug.LogWarning($"PlayAudioOnShot on {name}: Missing AudioSource or shotClip.");
            }
        }


        /// <summary>
        /// Plays reload sound.
        /// </summary>
        public void PlayReload()
        {
            if (audioSource != null && reloadClip != null)
            {
                audioSource.PlayOneShot(reloadClip);
            }
            else
            {
                Debug.LogWarning($"PlayAudioOnShot on {name}: Missing AudioSource or shotClip.");
            }
        }
    }
}