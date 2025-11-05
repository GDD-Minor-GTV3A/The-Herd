using UnityEngine;

public class RifleAudio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource; 
    [SerializeField] private AudioClip shotClip;      
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
}
