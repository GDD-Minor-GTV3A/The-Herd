using UnityEngine;
using Core.AI.Sheep; // gives access to SheepStateManager

public class SheepPenZone : MonoBehaviour
{
    [Tooltip("The decoy Transform inside the pen that sheep should follow.")]
    public Transform decoyTransform;

    [Tooltip("Reference to the player Transform.")]
    public Transform playerTransform;

    private bool sheepFollowingDecoy = false;
    private SheepStateManager[] sheepManagers;

    private void Start()
    {
        sheepManagers = FindObjectsOfType<SheepStateManager>();

        // Make sure they start active and following the player normally
        foreach (var sheep in sheepManagers)
        {
            if (sheep == null) continue;
            sheep.OnRejoinedHerd();
        }

        Debug.Log("Sheep initialized to follow player normally.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        sheepFollowingDecoy = !sheepFollowingDecoy;

        if (sheepFollowingDecoy)
        {
            SendSheepToDecoy();
            Debug.Log("Sheep are now following the DECOY (pen).");
        }
        else
        {
            ReturnSheepToPlayer();
            Debug.Log("Sheep are now following the PLAYER.");
        }
    }

    private void SendSheepToDecoy()
    {
        foreach (var sheep in sheepManagers)
        {
            if (sheep == null || !sheep.CanControlAgent()) continue;

            // Stop their state logic so it doesn’t override movement
            sheep.enabled = false;

            Vector2 offset = Random.insideUnitCircle * 1.5f;
            Vector3 dest = decoyTransform.position + new Vector3(offset.x, 0, offset.y);
            sheep.Agent.SetDestination(dest);
        }
    }

    private void ReturnSheepToPlayer()
    {
        foreach (var sheep in sheepManagers)
        {
            if (sheep == null) continue;

            // Reactivate the AI logic
            sheep.enabled = true;

            // Move them toward player instantly so they resume near the player
            if (sheep.CanControlAgent())
            {
                Vector2 offset = Random.insideUnitCircle * 1.5f;
                Vector3 dest = playerTransform.position + new Vector3(offset.x, 0, offset.y);
                sheep.Agent.SetDestination(dest);
            }

            // Trigger the “back to herd” event
            sheep.OnRejoinedHerd();
        }
    }
}
