using UnityEngine;

public class Scare : MonoBehaviour
{
    [SerializeField] private GameObject enemy2Prefab;

    [SerializeField] private EnemySpawnpoint enemiesParent;
    [SerializeField] private Light[] redLights;
    private bool playerEnteredQuestArea = false;
    private bool spawnedScare = false;

    /// <summary>
    /// Handles OnQuestAreaEntered Event. sets playerEnteredQuestArea to true.
    /// </summary>
    /// <param name="spawnPoints"></param>
    public void OnQuestAreaEntered(EnemySpawnpoint[] _spawnPoints)
    {
        playerEnteredQuestArea = true;
        Debug.Log("player entered quest area");
    }

    /// <summary>
    /// Handles OnQuestAreaExit Event. triggers the scare (currently 4 enemies spawn around the player)
    /// </summary>
    /// <param name="spawnPoints"></param>
    public void OnQuestAreaExit(EnemySpawnpoint[] spawnPoints)
    {
        if (!playerEnteredQuestArea || spawnedScare) return;
        spawnedScare = true;

        foreach (Light light in redLights) { light.enabled = true; }
        foreach (EnemySpawnpoint spawnPoint in spawnPoints)
        {
            spawnPoint.spawn();
        }
    }

    /// <summary>
    /// Handles OnChaseTrigger Event. Only spawnes enemies when the scare has already happend.
    /// </summary>
    /// <param name="spawnPoints"></param>
    public void OnChaseTrigger(EnemySpawnpoint[] spawnPoints)
    {
        if (!spawnedScare) return;

        foreach (EnemySpawnpoint spawnPoint in spawnPoints)
        {
            spawnPoint.spawn();
        }
    }

    public void test(EnemySpawnpoint[] spawnPoints)
    {
        foreach (EnemySpawnpoint spawnPoint in spawnPoints)
        {
            spawnPoint.spawn();
        }
    }
}
