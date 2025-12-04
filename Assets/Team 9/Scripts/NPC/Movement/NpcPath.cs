using System;
using UnityEngine;

public class NpcPath : MonoBehaviour
{
    [SerializeField] private string questID;
    [SerializeField] private string objectiveID;
    [SerializeField] private bool isOneShot;
    private bool _questCompleted;
    
    
    public string QuestID => questID;
    public string ObjectiveID => objectiveID;
    public bool IsOneShot => isOneShot;

    public bool QuestCompleted => _questCompleted;

    
    
    public Transform[] waypoints;
    public bool GetsReverted = false;
    public bool IsCompleted = false;

    private void Start()
    {
        _questCompleted = QuestManager.Instance?.CheckIfQuestCompleted(questID) ?? false;
    }


    public void RevertPath()
    {
        Array.Reverse(waypoints);
    }
}
