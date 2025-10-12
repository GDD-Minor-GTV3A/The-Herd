using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpawnTrigger : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;

    [SerializeField] private SpawnTriggerEvent onSpawnTriggerd;

    /// <summary>
    /// Callback for box collider. Gets triggered when player enters, and invokes the onSpawnTriggered Event passing it all its spawnPoints
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            onSpawnTriggerd.Invoke(spawnPoints);
        }
    }
}

[System.Serializable]
public class SpawnTriggerEvent : UnityEvent<Transform[]> { }
