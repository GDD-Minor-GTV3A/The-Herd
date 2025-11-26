using System;
using System.Collections.Generic;

using UnityEngine;

public class LevelManagerLevel2 : MonoBehaviour
{
    public static LevelManagerLevel2 Instance { get; private set; }
    private bool ForestQuestComplete = false;
    private bool FrontDoorReturn = false;
    private List<GameObject> SpawnedSheep = new List<GameObject>();

    [SerializeField] private SlidingDoor EntranceDoor, MazeDoor, ForestDoor;
    [SerializeField] private GameObject SheepPrefab;
    [SerializeField] private int SheepCount;

    /// <summary>
    /// Create instance of level manager
    /// </summary>
    private void Awake()
    {
        // Ensure there is only one instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Execute when entering the level
    /// </summary>
    public void EnterLevel()
    {
        Debug.Log("Level 2 entered");

        // Spawn sheep in the level
        for (int i = 0; i < SheepCount; i++)
        {
            SpawnSheep();
        }
    }

    /// <summary>
    /// Execute when leaving the level
    /// </summary>
    public void LeaveLevel()
    {
        Debug.Log("Level 2 left");
        MazeDoor.Close();
        ForestDoor.Close();
        FrontDoorReturn = false;
    }

    /// <summary>
    /// Close entrance and open doors based on quest completion
    /// </summary>
    public void EnteredCastle()
    {
        // Player enters castle and front door should not remain open for return
        if (EntranceDoor.isOpen && !FrontDoorReturn)
        {
            EntranceDoor.Lock();
            EntranceDoor.Close();
        }

        // Front door should be open for return, but is closed
        if (FrontDoorReturn && !EntranceDoor.isOpen)
        {
            EntranceDoor.Open();
        }

        // Choose which door in the castle to open
        if (ForestQuestComplete)
        {
            MazeDoor.Open();
        }
        else
        {
            ForestDoor.Open();
        }
    }

    /// <summary>
    /// Execute when player finished the forest quest, make entrance door open for return
    /// </summary>
    public void FinishedForestQuest()
    {
        ForestQuestComplete = true;
        FrontDoorReturn = true;
        EntranceDoor.Open();
    }
    
    /// <summary>
    /// Spawn sheep at default location or at given location
    /// </summary>
    public void SpawnSheep(Vector3? position = null)
    {
        Vector3 spawnPosition = position ?? new Vector3(891, 0, 548);
        GameObject sheep = Instantiate(SheepPrefab, spawnPosition, Quaternion.identity);
        sheep.transform.localScale *= 3;
        SpawnedSheep.Add(sheep);
    }
}
