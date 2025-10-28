using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpawnTrigger : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;

    [SerializeField] private SpawnTriggerEvent onSpawnTriggered;

    [SerializeField] private bool oneTime = false;

    private bool firstSpawn = false;

    /// <summary>
    /// Callback for box collider. Gets triggered when player enters, and invokes the onSpawnTriggered Event passing it all its spawnPoints
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (oneTime && firstSpawn) return;
        firstSpawn = true;
        if (other.tag == "Player")
        {
            onSpawnTriggered.Invoke(spawnPoints);
        }
    }
}

[System.Serializable]
public class SpawnTriggerEvent : UnityEvent<Transform[]> { }
