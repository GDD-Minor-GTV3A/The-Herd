using System;

using Core.Events;

using UnityEngine;

public class ObjectiveTrigger : MonoBehaviour
{
    [SerializeField] private string objectiveID = "";

    [SerializeField] private string questID = "";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

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
