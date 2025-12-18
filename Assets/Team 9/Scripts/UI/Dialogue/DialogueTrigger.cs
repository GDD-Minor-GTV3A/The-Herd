using Core.Events;

using TMPro;

using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    //TODO: EVERYTHING IN HERE!!!!
    [SerializeField] private TextAsset _inkJSON;

    //HANDLE UI SOMEWHERE ELSE!!!!
    [SerializeField] private TextMeshProUGUI _interText;

    [SerializeField] private string[] questID;
    [SerializeField] private string objectiveID = "";

    private const string PLAYER_TAG = "Player";

    // ... (OnTriggerEnter is unchanged) ...
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PLAYER_TAG))
        {
            Debug.Log("Press E to Start Conversation");
            if (!_interText) return;

            _interText.gameObject.SetActive(true);
            _interText.enabled = true;
        }
        Debug.Log("weirdo detected");
    }


    /// <summary>
    /// Gives the player the ability to interact with NPC's
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(PLAYER_TAG)) return;
        if (!DialogueManager.GetInstance()) return;
        
        // 1. If dialogue is already playing, hide the interaction prompt and return
        if (DialogueManager.GetInstance().IsDialoguePlaying)
        {
            if (!_interText) return;
            _interText.enabled = false;
            return;
        }
        
        // 2. Otherwise, make sure the prompt is visible
        if (_interText && !_interText.enabled)
        {
            _interText.enabled = true;
        }
        
        // 3. Handle interaction input
        if (Input.GetKey(KeyCode.E))
        {
            // Complete any objectives associated with starting this dialogue (if needed)
            foreach(var id in questID)
            {
                // NOTE: It is unusual to complete an objective *before* the dialogue begins, 
                // but following your existing logic:
                EventManager.Broadcast(new CompleteObjectiveEvent(id, objectiveID));
            }
            
            // --- CRITICAL CHANGE HERE ---
            // We pass 'this.gameObject' (the NPC) as the speakerObject
            DialogueManager.GetInstance().EnterDialogueMode(_inkJSON, this.gameObject);
        }
    }


    // ... (OnTriggerExit is unchanged) ...
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(PLAYER_TAG))
        {
            Debug.Log("NPC_TRIGGER: EXIT");
            if (!_interText) return;
            _interText.enabled = false;
        }
    }
}