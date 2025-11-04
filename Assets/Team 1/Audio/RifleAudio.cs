using UnityEngine;

public class RifleAudio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource; // Assign an AudioSource in Inspector
    [SerializeField] private AudioClip shotClip;      // Assign your gunshot clip
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
