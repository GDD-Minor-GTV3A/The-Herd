using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Temporary remake of 'SpawnEnemy.cs' for spawning enemy with triggers at specific location.
/// </summary>
public class SpawnEnemyAtLocation : MonoBehaviour
{
    [Header("Enemy to spawn")]
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private Transform[] spawnPoints;

    private bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        { 
            if(!activated)
            {
                foreach (Transform _spawnPoint in spawnPoints)
                {
                    // Pick a random enemy out of the chosen ones
                    GameObject _enemy = enemies[Random.Range(0, enemies.Length - 1)];
                    
                    int _maxAttempts = 50;      // safety cap
                    
                    Debug.Log("Attempting to spawn '" + _enemy + "'");
                    // Spawn at all spawnpoints
                    for (int i = 0; i < _maxAttempts; i++)
                    {
                        // Compute spawn candidate
                        Vector3 spawnPosition = _spawnPoint.position;
                        
                        // Try to find a valid NavMesh position nearby
                        NavMeshHit hit;
                        if (NavMesh.SamplePosition(spawnPosition, out hit, 5.0f, NavMesh.AllAreas))
                        {
                            Instantiate(_enemy, hit.position, Quaternion.identity);
                            return; // success, stop the loop
                        }
                    }
                    
                    // If we got here, all attempts failed
                    Debug.LogWarning("Failed to find NavMesh position for '" + _enemy + "' after multiple attempts.");
                }
                activated = true;
            }
        }
    }
}
