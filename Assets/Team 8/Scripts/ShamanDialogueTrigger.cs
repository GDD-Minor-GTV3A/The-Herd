using UnityEngine;
using TMPro;

public class ShamanDialogueTrigger : MonoBehaviour
{
    public ShamanDialogueManager shamanDialogueManager;
    [SerializeField] private TextMeshProUGUI _interText;
    private bool playerInRange = false;

    private void Start()
    {
        if (_interText != null)
            _interText.enabled = false; // start hidden
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            shamanDialogueManager.StartDialogue();
            _interText.enabled = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (_interText != null)
                _interText.enabled = true; // show text
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (_interText != null)
                _interText.enabled = false; // hide text
        }
    }
    
    public void ShowInterText()
    {
        if (playerInRange && _interText != null)
            _interText.enabled = true;
    }

}