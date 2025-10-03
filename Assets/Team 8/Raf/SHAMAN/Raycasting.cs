using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class NPCRaycaster : MonoBehaviour
{
    public Transform player;             // assign the Player in inspector
    public float sightRange = 20f;       // how far NPC can see
    public float minVolumeDistance = 2f; // distance where bell is loudest
    public float maxVolumeDistance = 15f; // distance where bell is quietest

    private AudioSource bell;

    void Awake()
    {
        bell = GetComponent<AudioSource>();
        bell.loop = true;
        bell.playOnAwake = false;
    }

    void Update()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, player.position);

        // Raycast toward the player
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, sightRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                if (!bell.isPlaying)
                    bell.Play();

                // Volume based on distance (closer = louder)
                float t = Mathf.InverseLerp(maxVolumeDistance, minVolumeDistance, distance);
                bell.volume = Mathf.Clamp01(t);
            }
            else
            {
                if (bell.isPlaying)
                    bell.Stop();
            }

            // Debug line in Scene view
            Debug.DrawRay(transform.position, direction * sightRange, Color.red);
        }
    }
}
