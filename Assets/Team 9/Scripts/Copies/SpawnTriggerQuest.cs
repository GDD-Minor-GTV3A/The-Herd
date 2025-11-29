using System.Collections;
using System.Collections.Generic;

using Core.Events;

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
    [Header("Quest Settings")]
    [SerializeField] private string questID;
    [SerializeField] private string objectiveID;
    [SerializeField] private string npcName;
    [SerializeField] private bool npcNeeded = false;

    private bool npcEntered = false;
    
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
        if (!QuestManager.Instance.GetQuestProgressByID(questID).IsObjectiveActive(objectiveID))
        {
            Debug.Log("OBJECTIVE NOT ACTIVE");
        }

        if (npcNeeded)
        {
            if (other.name == npcName)
            {
                npcEntered = true;
            }
        }
        
        if (other.CompareTag("Player") )
        {
            if (npcNeeded && !npcEntered) return;
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
        EventManager.Broadcast(new CompleteObjectiveEvent(questID, objectiveID));
        DestroyAllSpawns();
        spawnRoutine = null;
    }
    
    
    //Since enemies cannot die at the moment, destroy after spawn
    private void DestroyAllSpawns()
    {
        foreach (var step in spawnSteps)
        {
            foreach (var point in step.spawnPoints)
            {
                point.DestroyEnemies();
            }            
        }
    }
}
