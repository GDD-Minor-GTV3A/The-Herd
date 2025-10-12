using UnityEngine;
using UnityEngine.AI;

public class SpawnEnemy : MonoBehaviour
{
    public GameObject Drekavac;
    public float maxDistance;
    public float minDistance;
    public NavMeshAgent agent;

    void Update()
    {
        // TEMPORARY press E to spawn the enemy
        if (Input.GetKeyDown("e"))
        {
            Spawn();
        }
    }

    private void Spawn()
    {
        int maxAttempts = 50;      // safety cap

        for (int i = 0; i < maxAttempts; i++)
        {
            // Pick a random direction on the unit sphere
            Vector3 randomDirection = Random.onUnitSphere;

            // Pick a random distance between min and max
            float randomDistance = Random.Range(minDistance, maxDistance);

            // Compute spawn candidate
            Vector3 spawnPosition = transform.position + randomDirection * randomDistance;

            // Try to find a valid NavMesh position nearby
            NavMeshHit hit;
            if (NavMesh.SamplePosition(spawnPosition, out hit, 5.0f, NavMesh.AllAreas))
            {
                Instantiate(Drekavac, hit.position, Quaternion.identity);
                return; // success, stop the loop
            }
        }

        // If we got here, all attempts failed
        Debug.LogWarning("Failed to find NavMesh position for enemy spawn after multiple attempts.");
    }
}
