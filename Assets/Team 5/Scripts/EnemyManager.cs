using Unity.VisualScripting;

using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    [SerializeField] private SpawnTrigger[] spawnTriggers;


/// <summary>
/// Handles spawnTrigger trigger and spawns enemies of the spawnTriggers
/// </summary>
/// <param name="spawnPoints"></param>
    public void OnSpawnTriggered(EnemySpawnpoint[] spawnPoints)
    {
        foreach (EnemySpawnpoint spawnPoint in spawnPoints)
        {
            spawnPoint.spawn();
        }
    }

/// <summary>
/// Removes all enemies currently in the scene
/// </summary>
    public void removeAllEnemies()
    {
        foreach (Transform child in transform){
            Destroy(child.gameObject);
        }
    }
}
