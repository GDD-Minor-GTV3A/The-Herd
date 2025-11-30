using UnityEngine;
using Core.AI.Sheep;

/// <summary>
/// Controls the vesna sheep interaction behavior.
/// When a sheep enters the zone, vesna can occasionally interact with it.
/// </summary>
public class VesnaSheepInteractionZone : MonoBehaviour
{
    // Define the different interaction states.
    public enum State { Idle, Interacting, Moving}

    [Header("Settings")]
    public float animationTime = 2; // Animation duration
    public float pointReachThreshold = 0.5f; // Distance to vesna to consider reached for interaction
    
    private State currentState = State.Idle;
    private bool inRange = false;
    private float timer = 0;
    private SheepStateManager sheep;
    private float randomDelay;

    // Checks if player entered Collider
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        inRange = true;
        randomDelay = 5f;
    }

    // Checks if player exited Collider
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        inRange = false;
    }

    private void Update()
    {
        // Update only if player is in range
        if (inRange)
        {
            // Update based on current state
            switch (currentState)
            {
                case State.Moving: UpdateMoving(); break;
                case State.Idle: UpdateIdle(); break;
                case State.Interacting: UpdateInteracting(); break;
            }
        }
    }

    // Logic for idle state
    private void UpdateIdle()
    {
        // Countdown timer to next interaction
        timer += Time.deltaTime;

        // If timer exceeds random delay, switch to moving state
        if (timer >= randomDelay)
        {
            currentState = State.Moving;
            timer = 0;
            SendSheepToVesna();
        }
    }

    // Logic for moving state
    private void UpdateMoving()
    {
        // If sheep reached vesna, switch to interacting state
        if (sheep.Agent.remainingDistance <= pointReachThreshold)
        {
            currentState = State.Interacting;
            // TODO : Trigger interaction animation here
        }
    }

    // Logic for interacting state
    private void UpdateInteracting()
    {
        // Countdown animation timer
        timer += Time.deltaTime;

        // When animation time is up, return to idle state
        if (timer >= animationTime)
        {
            currentState = State.Idle;
            timer = 0;
            randomDelay = Random.Range(3f, 6f);
            ReturnSheepToPlayer();
        }
    }

    // Sends a random sheep to vesna for interaction
    private void SendSheepToVesna()
    {
        // Chooses a random sheep from all active sheep
        SheepStateManager[] sheepManagers = FindObjectsOfType<SheepStateManager>();
        if (sheepManagers.Length == 0)
            return;
        int choice = Random.Range(0, sheepManagers.Length);
        sheep = sheepManagers[choice];

        // Temporarily disable AI logic so it does not override movement
        sheep.enabled = false;

        // Set destination to vesna's position
        Vector3 destination = transform.position;
        sheep.Agent.SetDestination(destination);
        Debug.Log("Sheep will now go to Vesna.");
    }

    private void ReturnSheepToPlayer()
    {
        if (sheep == null || !sheep.CanControlAgent())
            return;

        // Re-enable AI logic after interaction
        sheep.enabled = true;
        sheep.OnRejoinedHerd();
    }
}