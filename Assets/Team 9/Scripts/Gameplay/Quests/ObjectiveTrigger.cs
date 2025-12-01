using System;

using Core.Events;

using UnityEngine;


/// <summary>
/// This is used to trigger QuestObjectives
/// </summary>
public class ObjectiveTrigger : MonoBehaviour
{
    /// <summary>
    /// Objective ID that gets triggered
    /// </summary>
    [SerializeField] private string objectiveID = "";

    /// <summary>
    /// Quest ID of the Objective
    /// </summary>
    [SerializeField] private string questID = "";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    /// <summary>
    /// If the player enters the trigger.
    /// A CompleteObjectiveEvent is called
    /// </summary>
    /// <param name="other">Collider that entered the trigger</param>
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        EventManager.Broadcast(new CompleteObjectiveEvent(questID, objectiveID));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
