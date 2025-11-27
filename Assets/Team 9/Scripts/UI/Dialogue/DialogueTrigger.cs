using Core.Events;

using TMPro;

using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    //TODO: EVERYTHING IN HERE!!!!
    [SerializeField] private TextAsset _inkJSON;

    //HANDLE UI SOMEWHERE ELSE!!!!
    [SerializeField] private TextMeshProUGUI _interText;

    [SerializeField] private string questID = "";
    [SerializeField] private string objectiveID = "";

    private const string PLAYER_TAG = "Player";

    /// <summary>
    /// When the player enters the trigger zone, a text will appear saying "Press E to interact"
    /// </summary>
    /// <param name="other"></param>
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
        Debug.Log($"Inside trigger: {gameObject.name}");
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
            EventManager.Broadcast(new CompleteObjectiveEvent(questID, objectiveID));
            DialogueManager.GetInstance().EnterDialogueMode(_inkJSON);
        }
        //INPUT
        //FURTHER STUFF
    }


    /// <summary>
    /// When the player exits the trigger zone, the interact text will disappear.
    /// </summary>
    /// <param name="other"></param>
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