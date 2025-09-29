using UnityEngine;

public class QuestStarter : MonoBehaviour
{
    //THIS IS JUST FOR TESTING!!!
    public QuestManager manager;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager.Initialize();
    }
}
