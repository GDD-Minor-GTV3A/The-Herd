using UnityEngine;

public class LevelManagerLevel2 : MonoBehaviour
{
    public static LevelManagerLevel2 Instance { get; private set; }
    private bool ForestQuestComplete = false;

    [SerializeField] private SlidingDoor EntranceDoor, MazeDoor, ForestDoor;

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

    public void EnterLevel()
    {
        Debug.Log("Level 2 entered");
    }

    public void EnteredCastle()
    {
        if (EntranceDoor.isOpen)
        {
            EntranceDoor.Lock();
            EntranceDoor.Close();
        }

        if (ForestQuestComplete)
        {
            MazeDoor.Open();
        }
        else
        {
            ForestDoor.Open();
        }
    }
    
}
