using UnityEngine;

public class DogSoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip bark;

    public void PlayBarkSound()
    {
        if (bark == null)
        {
            Debug.LogWarning("DamageSoundPlayer: No damage sounds assigned!");
            return;
        }
        AudioClip clip = bark;
        audioSource.PlayOneShot(clip);
    }
}
