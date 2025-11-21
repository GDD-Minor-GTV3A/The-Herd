using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    [Header("Sound Settings")]
    public int clipIndex = 1; 
    public int boxIndex = 1;
    public float volume = 1f;      

    private void OnTriggerEnter(Collider other)
    {
        // Only trigger for the player
        if (other.CompareTag("Player"))
        {
            // Play the sound via AudioManager
            AudioManager.Instance.PlaySFXAtBox(clipIndex, boxIndex, volume);

            // Destroy this trigger so it only plays once
            Destroy(gameObject);
        }
    }
}
