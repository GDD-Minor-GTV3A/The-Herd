using Core.Shared.Utilities;
using UnityEngine;

namespace Gameplay.Audio
{
    /// <summary>
    /// Manages whistle's sounds playing.
    /// </summary>
    public class WhistleSoundManager : MonoBehaviour
    {
        [SerializeField, Tooltip("Audio source of whistle."), Required] 
        private AudioSource audioSource;

        [SerializeField, Tooltip("Audio clip for dog move command."), Required] 
        private AudioClip dogMoveClip;

        [SerializeField, Tooltip("Audio clip for dog herd command."), Required] 
        private AudioClip herdClip;


        /// <summary>
        /// Play sound of herd command.
        /// </summary>
        public void PlayHerd()
        {
            if (audioSource != null && herdClip != null)
            {
                audioSource.PlayOneShot(herdClip);
            }
            else
            {
                Debug.LogWarning($"Missing AudioSource or herdClip.");
            }
        }


        /// <summary>
        /// Play sound of move command.
        /// </summary>
        public void PlayMove()
        {
            if (audioSource != null && dogMoveClip != null)
            {
                audioSource.PlayOneShot(dogMoveClip);
            }
            else
            {
                Debug.LogWarning("Missing AudioSource or dogMoveClip.");
            }
        }
    }
}