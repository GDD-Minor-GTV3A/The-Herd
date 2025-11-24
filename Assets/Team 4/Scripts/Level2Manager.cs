using UnityEngine;

public class LevelManagerLevel2 : MonoBehaviour
{
    public static LevelManagerLevel2 Instance { get; private set; }
    private bool ForestQuestComplete = false;
    private bool FrontDoorReturn = false;

    [SerializeField] private SlidingDoor EntranceDoor, MazeDoor, ForestDoor;

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
    
}
