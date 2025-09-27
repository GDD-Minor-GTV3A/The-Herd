using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Ink JSON")]
    [SerializeField] private TextAsset _inkJson;
    
    public void Initialize()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            DialogueManager dialogueManager = DialogueManager.GetInstance();
            if (dialogueManager != null && !dialogueManager.IsDialoguePlaying)
            {
                dialogueManager.EnterDialogueMode(_inkJson);
            }
        }
    }
}