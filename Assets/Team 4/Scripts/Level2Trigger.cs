using UnityEngine;
using UnityEngine.Events;

public class Level2Trigger : MonoBehaviour
{
    [Header("Trigger Settings")]
    [Tooltip("Tag of the object that activates this trigger (usually 'Player')")]
    [SerializeField] private string targetTag = "Player";

    [Tooltip("Destroy this trigger after it is hit once?")]
    [SerializeField] private bool oneTimeUse = true;

    // Create the event box in the Inspector
    [Space]
    public UnityEvent onTriggerEnter;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object hitting the trigger is the target (e.g., the Player)
        if (other.CompareTag(targetTag))
        {
            onTriggerEnter.Invoke();

            if (oneTimeUse)
            {
                Destroy(gameObject);
            }
        }
    }
}
