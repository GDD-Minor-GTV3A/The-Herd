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
    [SerializeField] private SpawnTrigger spawnTrigger;

    private Player player;
    private Dog dog;


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
            Instantiate(sheepPrefab, new Vector3(21.6790237f, 0.261999995f, 20.3814964f), Quaternion.identity, sheepParent);
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

    public void test()
    {
        Debug.Log("trigger event!");
    }
}
