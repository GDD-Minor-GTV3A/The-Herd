using UnityEngine;
using UnityEngine.AI;

namespace Team_7.Scripts.Core
{
    /// <summary>
    ///     Handles the logic for spawning enemies.
    /// </summary>
    public class SpawnEnemy : MonoBehaviour
    {
        [Header("Enemy to spawn")]
        public GameObject Drekavac;
        public GameObject Phantom;

        [Header("Spawn distance")]
        public float maxDistance;
        public float minDistance;

        void Update()
        {
            // TEMPORARY press E to spawn the enemy
            if (Input.GetKeyDown("e"))
            {
                Spawn(Drekavac);
            }
            else if (Input.GetKeyDown("f"))
            {
                Spawn(Phantom);
            }
        }

        private void Spawn(GameObject enemy)
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
                    Instantiate(enemy, hit.position, Quaternion.identity);
                    return; // success, stop the loop
                }
            }

            // If we got here, all attempts failed
            Debug.LogWarning("Failed to find NavMesh position for enemy spawn after multiple attempts.");
        }
    }
}
