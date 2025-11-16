using UnityEngine;

namespace Gameplay.Audio 
{
    public class RifleSoundManager : MonoBehaviour
    {
        [SerializeField, Tooltip("Audio source of rifle.")] 
        private AudioSource audioSource;
        [SerializeField] 
        private AudioClip shotClip;
        [SerializeField] 
        private AudioClip reloadClip;

        [SerializeField]
        private Sprite sprite;


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