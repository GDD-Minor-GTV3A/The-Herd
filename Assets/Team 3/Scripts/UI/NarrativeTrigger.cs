using Core.Events;

using TMPro;

using UnityEngine;

/// <summary>
/// Detects when the players enters a narrative trigger zone, and display the narrative
/// </summary>
public class NarrativeTrigger : MonoBehaviour
{
    [Header("Narrative file JSON")]
    [Tooltip("The file with the narrative that will be shown after entering the trigger.")]
    [SerializeField] private TextAsset _inkJSON;

    private const string PLAYER_TAG = "Player";

    /// <summary>
    /// When the player enters the trigger zone, the narrative will open"
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PLAYER_TAG))
        { 
            // DialogueManager.GetInstance().EnterDialogueMode(_inkJSON);  <- Chris: This is a compile error. Please consult it with Team 9
            Destroy(gameObject);
        }
    }
}