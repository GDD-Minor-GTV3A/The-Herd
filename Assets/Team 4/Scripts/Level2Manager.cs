using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class LevelManagerLevel2 : MonoBehaviour
{
    public static LevelManagerLevel2 Instance { get; private set; }
    private bool ForestQuestComplete = false;
    private bool FrontDoorReturn = false;
    private List<GameObject> SpawnedSheep = new List<GameObject>();
    private List<GameObject> EscapedSheep = new List<GameObject>();
    private bool SheepEscaped = false;
    private int SheepCollectedCount = 0;

    [SerializeField] private SlidingDoor EntranceDoor, MazeDoor, ForestDoor, MazeExitDoor, ReturnPathDoor;
    [SerializeField] private GameObject SheepPrefab;
    [SerializeField] private GameObject SheepModel;
    [SerializeField] private GameObject[] EscapeSheepPlaces;
    [SerializeField] private int SheepCount;
    [SerializeField] private float SheepWalkSpeed = 15f;

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
            // Spawn sheep in the level
            for (int i = 0; i < SheepCount; i++)
            {
                SpawnSheep();
            }
        }
    }

    /// <summary>
    /// Execute when entering the level
    /// </summary>
    public void EnterLevel()
    {
        Debug.Log("Level 2 entered");
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
        MazeDoor.Open();
    }
    
    /// <summary>
    /// Spawn sheep at default location or at given location
    /// </summary>
    public void SpawnSheep(Vector3? position = null)
    {
        Vector3 spawnPosition = position ?? new Vector3(879, 0, 513);
        GameObject sheep = Instantiate(SheepPrefab, spawnPosition, Quaternion.identity);
        SpawnedSheep.Add(sheep);
    }

    /// <summary>
    /// Make all sheep escape to designated points
    /// </summary>
    public void SheepEscape()
    {
        // If sheep have already escaped, do nothing
        if (SheepEscaped) return;

        foreach (GameObject spawnedSheep in SpawnedSheep)
        {
            EscapedSheep.Add(spawnedSheep);
            GameObject sheepModel = Instantiate(SheepModel, spawnedSheep.transform.position, Quaternion.identity);
            sheepModel.transform.localScale *= 3;
            spawnedSheep.transform.position = new Vector3(0, -100, 0);
            // Walk sheep to first available escape point
            foreach (GameObject escapePoint in EscapeSheepPlaces)
            {
                if (!escapePoint.GetComponent<FrozenSheepTrigger>().ContainsSheep)
                {
                    escapePoint.GetComponent<FrozenSheepTrigger>().SetSheep(spawnedSheep);
                    StartCoroutine(WalkSheepToEscapePoint(sheepModel, escapePoint.transform.position));

                    break;
                }
            }
        }

        SheepEscaped = true;
    }

    /// <summary>
    /// Coroutine to walk the sheep model gradually to the escape point and remove it
    /// </summary>
    private IEnumerator WalkSheepToEscapePoint(GameObject sheepModel, Vector3 escapePoint)
    {
        if (sheepModel == null) yield break;

        while (sheepModel != null && Vector3.Distance(sheepModel.transform.position, escapePoint) > 0.1f)
        {
            sheepModel.transform.position = Vector3.MoveTowards(sheepModel.transform.position, escapePoint, SheepWalkSpeed * Time.deltaTime);
            yield return null;
        }

        // Ensure the sheep reaches the exact position
        if (sheepModel != null)
        {
            sheepModel.transform.position = escapePoint;
            
            // Remove the sheep model
            Destroy(sheepModel);
        }
    }

    /// <summary>
    /// When all sheep collected, open the maze exit and return path doors
    /// </summary>
    public void SheepCollected()
    {
        SheepCollectedCount++;
        if (SheepCollectedCount >= SheepCount)
        {
            MazeExitDoor.Open();
            ReturnPathDoor.Open();
        }
    }
}
