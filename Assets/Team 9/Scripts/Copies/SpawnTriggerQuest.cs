using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SpawnStep
{
    [Tooltip("Which spawn points to use in this step")]
    public EnemySpawnpointQuest[] spawnPoints;

    [Tooltip("Which enemies are spawned during this step")]
    public GameObject[] enemies;
    
    [Tooltip("How many enemies to spawn in this step")]
    public int spawnAmount = 5;

    [Tooltip("Delay between each spawn")]
    public float spawnDelay = 1f;

    [Tooltip("How long this step lasts before next one starts")]
    public float stepDuration = 5f;


    public void SetRandomEnemiesOnSpawnPoints()
    {
        foreach (var point in spawnPoints)
        {
            int randomEnemy = Random.Range(0, enemies.Length);
            point.SetEnemies(new [] {enemies[randomEnemy]});
        }
    }
}


public class SpawnTriggerQuest : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private List<SpawnStep> spawnSteps = new List<SpawnStep>();

    [SerializeField] private bool oneTime = false;
    private bool hasTriggered = false;

    [Header("Events")]
    [SerializeField] private SpawnTriggerEvent onSpawnTriggered;

    private Coroutine spawnRoutine;

    private void OnTriggerEnter(Collider other)
    {
        if (oneTime && hasTriggered) return;

        if (other.CompareTag("Player") || other.name == "Vesna")
        {
            Debug.Log($"{other.name} entered the trigger.");
            hasTriggered = true;
            if (spawnRoutine == null)
                spawnRoutine = StartCoroutine(HandleSpawnSequence());
        }
    }

    private IEnumerator HandleSpawnSequence()
    {
        Debug.Log("Starting spawn sequence...");

        foreach (var step in spawnSteps)
        {
            step.SetRandomEnemiesOnSpawnPoints();
            Debug.Log($"Starting spawn step: {step.spawnAmount} enemies over {step.stepDuration}s");
            float elapsed = 0f;
            int spawned = 0;

            while (elapsed < step.stepDuration && spawned < step.spawnAmount)
            {
                foreach (var points in step.spawnPoints)
                {
                    points.spawn();
                }
                spawned++;
                yield return new WaitForSeconds(step.spawnDelay);
                elapsed += step.spawnDelay;
            }

            yield return new WaitForSeconds(Mathf.Max(0f, step.stepDuration - elapsed));
        }

        Debug.Log("Spawn sequence complete.");
        spawnRoutine = null;
    }
}
