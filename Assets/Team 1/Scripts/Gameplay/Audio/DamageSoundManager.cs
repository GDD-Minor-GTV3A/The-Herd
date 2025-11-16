using Core.Shared.Utilities;
using UnityEngine;

namespace Gameplay.Audio
{
    public class DamageSoundManager : MonoBehaviour
    {

        [SerializeField, Tooltip("Audio source."), Required]
        private AudioSource audioSource;

        [SerializeField, Tooltip("List of possible damage clips.")] 
        private AudioClip[] damageClips;

        [Space, Header("Configurations")]
        [SerializeField, Tooltip("Min pitch of sound.")] 
        private float minPitch = 1f;

        [SerializeField, Tooltip("Max pitch of sound.")] 
        private float maxPitch = 1f;


        public void PlayDamageSound()
        {
            if (damageClips == null || damageClips.Length == 0)
            {
                Debug.LogWarning("DamageSoundPlayer: No damage sounds assigned!");
                return;
            }


            AudioClip _clip = damageClips[Random.Range(0, damageClips.Length)];


            audioSource.pitch = Random.Range(minPitch, maxPitch);


            audioSource.PlayOneShot(_clip);
        }


        private void OnValidate()
        {
            if (minPitch > maxPitch)
            {
                minPitch = maxPitch;
            }
        }
    }
}