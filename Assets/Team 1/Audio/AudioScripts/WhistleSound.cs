using UnityEngine;

public class WhistleSound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip herdClip;
    [SerializeField] private AudioClip dogMoveClip;

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
