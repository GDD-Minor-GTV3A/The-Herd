using UnityEngine;

public class DamageSound : MonoBehaviour
{
    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] damageClips; 

    
    [SerializeField] private float minPitch = 1f;     
    [SerializeField] private float maxPitch = 1f;

    
    
    public void PlayDamageSound()
    {
        if (damageClips == null || damageClips.Length == 0)
        {
            Debug.LogWarning("DamageSoundPlayer: No damage sounds assigned!");
            return;
        }

        
        AudioClip clip = damageClips[Random.Range(0, damageClips.Length)];

        
        audioSource.pitch = Random.Range(minPitch, maxPitch);

        
        audioSource.PlayOneShot(clip);
    }
}
