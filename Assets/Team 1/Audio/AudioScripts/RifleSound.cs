using UnityEngine;

public class RifleAudio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource; 
    [SerializeField] private AudioClip shotClip;
    [SerializeField] private AudioClip reloadClip;


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
