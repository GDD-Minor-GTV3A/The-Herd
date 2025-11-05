using UnityEngine;

public class PickUpRelic : MonoBehaviour
{
    [SerializeField] private SlidingDoor[] doorsToOpen;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // // Add relic to player's inventory
            // PlayerInventory inventory = other.GetComponent<PlayerInventory>();
            // if (inventory != null)
            // {
            //     inventory.AddRelic();
            //     Destroy(gameObject); // Remove the relic from the scene
            // }

            foreach (SlidingDoor door in doorsToOpen)
            {
                door.Open();
            }

            Destroy(gameObject); // Remove the relic from the scene
        }
    }
}
