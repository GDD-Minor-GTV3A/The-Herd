using UnityEngine;

public class torchSFX : MonoBehaviour
{
    public AudioSource torchAudio;
    public Transform player;

    [Header("Distance Settings")]
    public float minDistance = 3f;
    public float maxDistance = 50f;

    [Header("Volume Settings")]
    [Range(0f, 5f)]
    public float maxTorchVolume = 3f;   // <-- NEW: volume can go above 1

    void Start()
    {
        if (torchAudio == null) torchAudio = GetComponent<AudioSource>();

        if (torchAudio != null)
        {
            torchAudio.loop = true;
            torchAudio.spatialBlend = 1f;  // 3D sound
            torchAudio.minDistance = minDistance;
            torchAudio.maxDistance = maxDistance;

            torchAudio.Play();
        }
    }

    void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("[TORCH] No player assigned in " + name);
            return;
        }

        float distance = Vector3.Distance(player.position, transform.position);

        // 0 = far, 1 = near
        float t = Mathf.InverseLerp(maxDistance, minDistance, distance);

        // NEW: Fade to maxTorchVolume instead of only 1
        float targetVolume = Mathf.Lerp(0f, maxTorchVolume, t);

        // Smooth fading
        torchAudio.volume = Mathf.Lerp(torchAudio.volume, targetVolume, Time.deltaTime * 5f);
    }
}
