using System;
using UnityEngine;

public class NpcPath : MonoBehaviour
{
    [SerializeField] private string questID;
    [SerializeField] private string objectiveID;
    [SerializeField] private bool isOneShot;
    
    public string QuestID => questID;
    public string ObjectiveID => objectiveID;
    public bool IsOneShot => isOneShot;

    
    
    public Transform[] waypoints;
    public bool GetsReverted = false;
    public bool IsCompleted = false;
    
    public void RevertPath()
    {
        Array.Reverse(waypoints);
    }
}
