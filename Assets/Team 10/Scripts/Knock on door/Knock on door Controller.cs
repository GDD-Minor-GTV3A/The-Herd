using UnityEngine;
using TMPro;

public class KnockOnDoorController : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private bool playerInRange = false;
    private bool isInteracting = false;

    public bool PlayerInRange => playerInRange;
    public bool IsInteracting => isInteracting;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Player entered door knocking range");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("Player left door knocking range");
        }
    }

    void Update()
    {
        if (!playerInRange || isInteracting) return;

        // Check for interaction input
        if (Input.GetKeyDown(interactKey))
        {
            isInteracting = true;
            Debug.Log("Player knocked on the door");
            Invoke("ResetInteraction", 3f); // Reset interaction after 3 seconds
        }
    }
    private void ResetInteraction()
    {
        isInteracting = false;
    }
}
