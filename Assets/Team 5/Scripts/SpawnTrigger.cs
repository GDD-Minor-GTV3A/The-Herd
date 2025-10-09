using UnityEngine;
using UnityEngine.Events;

public class SpawnTrigger : MonoBehaviour
{
    //private bool triggered = false;

    // [SerializeField] private UnityEvent onTriggered;
    [SerializeField] private Transform enemySpawnpoint;

    [SerializeField] private LevelManager manager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // if (triggered) return;
            // triggered = true;
            // onTriggered.Invoke();   
            Instantiate(manager.GetEnemy2Prefab(), enemySpawnpoint.position, Quaternion.identity, enemySpawnpoint);
        }
    }
}
