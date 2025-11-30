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
    [SerializeField] protected string objectiveID = "";

    /// <summary>
    /// Quest ID of the Objective
    /// </summary>
    [SerializeField] protected string questID = "";

    [SerializeField] protected bool oneShot = false;
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
            gameObject.SetActive(false);
            Debug.Log("MAYBE DONT DESTROY? Don't destroy is correct");
        }
    }

    private void Start()
    {
        //Triggers start deactivated and are activated through the trigger activator
        //this.gameObject.SetActive(false);
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
