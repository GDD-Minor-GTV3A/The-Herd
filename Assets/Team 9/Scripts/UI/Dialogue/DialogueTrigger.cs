using System;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueTrigger : MonoBehaviour
{
    //TODO: EVERYTHING IN HERE!!!!
    [SerializeField] private TextAsset _inkJSON;
    
    //HANDLE UI SOMEWHERE ELSE!!!!
    [SerializeField] private TextMeshProUGUI _interText;
    

    /// <summary>
    /// Should enable the info "Press E to interact" but doesnt.....
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Press E to Start Conversation");
            if (!_interText) return;
            _interText.enabled = true;
        }
    }

    
    /// <summary>
    /// Gives the player the ability to interact with NPC's
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if (DialogueManager.GetInstance().IsDialoguePlaying)
        {
            if (!_interText) return;
            _interText.enabled = false;
            return;
        }
        if (_interText && !_interText.enabled)
        {
            _interText.enabled = true;
        }
        if (Input.GetKey(KeyCode.E))
        {
            DialogueManager.GetInstance().EnterDialogueMode(_inkJSON);
        }
        //INPUT
        //FURTHER STUFF
    }

    
    /// <summary>
    /// Should disable the info "Press E to interact" but doesnt.....
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Press E to Start Conversation");
            if (!_interText) return;
            _interText.enabled = false;
        }
    }
}
