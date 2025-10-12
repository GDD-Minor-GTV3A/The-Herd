using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpawnTrigger : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private Transform[] spawnPoints;

    /// <summary>
    /// Callback for box collider. Gets triggered when player enters, and calls OnSpawnTriggered function giving it the spawn points
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            levelManager.OnSpawnTriggered(spawnPoints);
        }
    }
}
