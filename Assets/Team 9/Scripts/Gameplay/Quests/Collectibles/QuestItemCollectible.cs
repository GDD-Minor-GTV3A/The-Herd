using System;

using Core.Events;

using Gameplay.Inventory;

using UnityEngine;

public class QuestItemCollectible : ObjectiveTrigger
{
    
    [SerializeField] private InventoryItem item;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("PLAYER ENTERED ITEM TRIGGER");
            EventManager.Broadcast(new CompleteObjectiveEvent(questID, objectiveID));
            PlayerInventory.Instance.AddItem(item);

            if (oneShot)
            {
                //Destroy(this.gameObject);
                Debug.Log("SMZSFDGFsd");
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
