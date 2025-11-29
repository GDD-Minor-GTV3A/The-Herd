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

    [SerializeField] private bool oneShot = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public string QuestID => questID;
    public string ObjectiveID => objectiveID;

    /// <summary>
    /// If the player enters the trigger.
    /// A CompleteObjectiveEvent is called
    /// </summary>
    /// <param name="other">Collider that entered the trigger</param>
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        EventManager.Broadcast(new CompleteObjectiveEvent(questID, objectiveID));
        if (oneShot)
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        //Triggers start deactivated and are activated through the trigger activator
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
