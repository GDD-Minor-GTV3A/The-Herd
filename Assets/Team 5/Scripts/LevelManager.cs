using UnityEngine;
using System;
using System.Collections;
using Gameplay.Player;
using Gameplay.Dog;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab, dogPrefab, sheepPrefab, enemy1Prefab, enemy2Prefab;
    [SerializeField] private Transform playerParent, sheepParent, enemiesParent;
    [SerializeField] private int sheepCount; // Might become a field in the Player class, in which case it wouldn't be needed here.

    private Player player;
    private Dog dog;

    private bool playerEnteredQuestArea = false;
    private bool spawnedScare = false;
    /// <summary>
    /// Initialization method for the dynamic parts of the level.
    /// </summary>
    void Start()
    {
        /* Places the playerprefab at position 0, 0, 0. Position will require changes later.
        *  Also, it currently uses the playerprefab, but the player will probably have a bunch of data attached to it, so it would need to have that data as well.
        */
        GameObject playerObject = Instantiate(playerPrefab, new Vector3(71.9899979f, 24.6000004f, 275.230011f), Quaternion.identity, playerParent);
        player = playerObject.GetComponent<Player>();
        player.Initialize();
        GameObject dogObject = Instantiate(dogPrefab, new Vector3(71.9899979f, 24.6000004f, 275.230011f), Quaternion.identity, playerParent);
        dog = dogObject.GetComponent<Dog>();
        dog.Initialize();

        // Places all the sheep at position 0, 0, 0. Positions will require changes later.
        for (int i = 0; i < sheepCount; i++)
        {
            Instantiate(sheepPrefab, new Vector3(71.9899979f, 24.6000004f, 275.230011f), Quaternion.identity, sheepParent);
        }

        // for (int i = 0; i < 4; i++)
        // {
        //     Instantiate(enemy2Prefab, new Vector3(71.9899979f, 24.6000004f, 275.230011f), Quaternion.identity, enemiesParent);
        // }
    }

    /// <summary>
    /// Gets the prefab of enemy 2
    /// </summary>
    /// <returns>GameObject of the enemy 2 prefab.</returns>
    public GameObject GetEnemy2Prefab()
    {
        return enemy2Prefab;
    }

    /// <summary>
    /// Handles OnQuestAreaEntered Event. sets playerEnteredQuestArea to true.
    /// </summary>
    /// <param name="spawnPoints"></param>
    public void OnQuestAreaEntered(Transform[] _spawnPoints)
    {
        playerEnteredQuestArea = true;
    }

    /// <summary>
    /// Handles OnQuestAreaExit Event. triggers the scare (currently 4 enemies spawn around the player)
    /// </summary>
    /// <param name="spawnPoints"></param>
    public void OnQuestAreaExit(Transform[] spawnPoints)
    {
        if (!playerEnteredQuestArea || spawnedScare) return;
        spawnedScare = true;

        foreach (Transform spawnPoint in spawnPoints)
        {
            Instantiate(enemy2Prefab, spawnPoint.position, Quaternion.identity, enemiesParent);
        }
    }

    /// <summary>
    /// Handles OnChaseTrigger Event. Only spawnes enemies when the scare has already happend.
    /// </summary>
    /// <param name="spawnPoints"></param>
    public void OnChaseTrigger(Transform[] spawnPoints)
    {
        if (!spawnedScare) return;

        foreach (Transform spawnPoint in spawnPoints)
        {
            Instantiate(enemy2Prefab, spawnPoint.position, Quaternion.identity, enemiesParent);
        }
    }

    private IEnumerator SpawnEnemies(Transform[] spawnPoints)
    {
        bool spawnOpportunity = true;

        while (spawnOpportunity && enemiesParent.childCount <= 10)
        {
            Instantiate(enemy2Prefab, spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].position, Quaternion.identity, enemiesParent);
            
            int roll = UnityEngine.Random.Range(0, 20);
            if (roll < 2) // 2 is currently a random value, if this is too difficult it can be increased, or decreased if it's too easy.
            {
                spawnOpportunity = false;
            }
            else
            {
                yield return new WaitForSeconds(20.0f); // The value is the amount of seconds between each roll.
            }
        }
    }
}
