using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    private void Start()
    {
        // Find and initialize the DialogueManager
        DialogueManager dialogueManager = FindFirstObjectByType<DialogueManager>();
        if (dialogueManager != null)
        {
            dialogueManager.Initialize();
        }


        //Not needed anymore
        // Find and initialize the DialogueTrigger
        //DialogueTrigger dialogueTrigger = FindFirstObjectByType<DialogueTrigger>();
        //if (dialogueTrigger != null)
        // {
        //     dialogueTrigger.Initialize();
        //}

        // Add more managers and initializers here as your game grows
    }
}