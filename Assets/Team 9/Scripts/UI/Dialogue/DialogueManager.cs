using System.Collections.Generic;
using Core.Events;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// A singleton class that manages the dialogue system,
/// including displaying dialogue, handling choices, and
/// managing character portraits and layouts.
/// </summary>
public class DialogueManager : MonoBehaviour
{
    // Serialized Fields
    [Header("Dialogue UI")]
    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private TextMeshProUGUI _displayNameText;
    [SerializeField] private Animator _portraitAnimator;
    
    [Header("Choices UI")]
    [SerializeField] private GameObject[] _choices;
    private TextMeshProUGUI[] _choicesText;
    
    [Header("Sanity")]
    [Range(1, 3)]
    [SerializeField] private int _sanity = 2; // 1=low, 2=medium, 3=high

    // Private Fields
    private Story _story;
    private Animator _layoutAnimator;
    private string _pendingPortraitState;
    private System.Action _onDialogueFinished;
    
    private static DialogueManager _instance;

    // Constants
    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";
    private const string LAYOUT_TAG = "layout";
    private const string DEFAULT_LAYOUT_STATE = "left";

    /// <summary>
    /// Gets a value indicating whether dialogue is currently playing.
    /// </summary>
    public bool IsDialoguePlaying { get; private set; } 

    /// <summary>
    /// Gets the singleton instance of the DialogueManager.
    /// </summary>
    public static DialogueManager GetInstance() => _instance;

    public Story Story => _story;
    
    /// <summary>
    /// Initializes the DialogueManager, setting up the UI and singleton instance.
    /// This method is called from the game bootstrap.
    /// </summary>
    public void Initialize()
    {
        if (_instance != null)
        {
            Debug.LogWarning("More than one DialogueManager in scene! Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        _instance = this;
        
        IsDialoguePlaying = false;
        _dialoguePanel.SetActive(false);
        _layoutAnimator = _dialoguePanel.GetComponent<Animator>();
        
        _choicesText = new TextMeshProUGUI[_choices.Length];
        for (int i = 0; i < _choices.Length; i++)
        {
            _choicesText[i] = _choices[i].GetComponentInChildren<TextMeshProUGUI>();
        }
        
    }
    

    private void Update()
    {
        if (!IsDialoguePlaying)
        {
            return;
        }

        // Handle pending portrait state
        if (!string.IsNullOrEmpty(_pendingPortraitState) && _portraitAnimator.gameObject.activeInHierarchy)
        {
            PlayPortraitState(_pendingPortraitState);
            _pendingPortraitState = null;
        }

        // Use Space for both advancing text and confirming a selection.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_story.currentChoices.Count == 0)
            {
                ContinueStory();
            }
            else
            {
                ConfirmChoiceSelection();
            }
        }
    }

    /// <summary>
    /// Handles the input logic for selecting the currently highlighted choice.
    /// </summary>
    private void ConfirmChoiceSelection()
    {
        GameObject selectedObject = EventSystem.current?.currentSelectedGameObject;
        
        if (selectedObject == null && _story.currentChoices.Count > 0)
        {
            // Fallback: If no button is selected but choices exist, force-select the first one.
            EventSystem.current?.SetSelectedGameObject(_choices[0]);
            selectedObject = _choices[0];
        }

        if (selectedObject != null)
        {
            for (int i = 0; i < _choices.Length; i++)
            {
                // Check if the selected object is one of our choice buttons
                if (_choices[i] == selectedObject)
                {
                    MakeChoice(i);
                    return; 
                }
            }
        }
    }

    /// <summary>
    /// Enters dialogue mode by loading an Ink story and displaying the first line.
    /// </summary>
    /// <param name="inkJson">The Ink JSON file containing the dialogue script.</param>
    /// <param name="onDialogueFinished">Optional callback action when dialogue exits.</param>
    public void EnterDialogueMode(TextAsset inkJson, System.Action onDialogueFinished = null)
    {
        if (IsDialoguePlaying)
        {
            return;
        }

        _story = new Story(inkJson.text);
        
        _story.BindExternalFunction("StartQuest", (string questID) => {
            StartQuest(questID);
        });
        
        _story.BindExternalFunction("CompleteObjective", (string questID, string objectiveID) => {
            CompleteObjective(questID, objectiveID);
        });
        IsDialoguePlaying = true;
        _dialoguePanel.SetActive(true);
        _onDialogueFinished = onDialogueFinished;
        ContinueStory();
    }

    private void ExitDialogueMode()
    {
        IsDialoguePlaying = false;
        _dialoguePanel.SetActive(false);
        _dialogueText.text = string.Empty;
        _pendingPortraitState = null;
        _layoutAnimator?.Play(DEFAULT_LAYOUT_STATE);
        
        _onDialogueFinished?.Invoke(); 
        _onDialogueFinished = null;
    }

    private void ContinueStory()
    {
        if (!_story.canContinue)
        {
            ExitDialogueMode();
            return;
        }

        string line = _story.Continue();
        HandleTags(_story.currentTags);
        _dialogueText.text = line;
        DisplayChoices();
    }

    private void HandleTags(List<string> tags)
    {
        string speaker = null;
        bool showPortrait = true;

        foreach (var tag in tags)
        {
            var split = tag.Split(':');
            if (split.Length != 2)
            {
                continue;
            }

            string key = split[0].Trim();
            string value = split[1].Trim();

            switch (key)
            {
                case SPEAKER_TAG:
                    speaker = value;
                    break;
                case PORTRAIT_TAG:
                    showPortrait = !value.Equals("false", System.StringComparison.OrdinalIgnoreCase); 
                    break;
                case LAYOUT_TAG:
                    _layoutAnimator?.Play(value);
                    break;
            }
        }

        ApplySpeakerAndPortrait(speaker, showPortrait);
    }

    private void ApplySpeakerAndPortrait(string speaker, bool showPortrait)
    {
        if (string.IsNullOrEmpty(speaker) || speaker.Equals("Narrator", System.StringComparison.OrdinalIgnoreCase))
        {
            _displayNameText.text = string.Empty;
            _portraitAnimator?.gameObject.SetActive(false);
            return;
        }

        _displayNameText.text = speaker;

        if (_portraitAnimator == null)
        {
            return;
        }

        _portraitAnimator.gameObject.SetActive(showPortrait);

        if (showPortrait)
        {
            string stateName = BuildPortraitStateName(speaker);
            if (!string.IsNullOrEmpty(stateName))
            {
                PlayPortraitState(stateName);
            }
        }
    }

    private string BuildPortraitStateName(string speaker)
    {
        // Player has no sanityy portraits
        if (speaker.Equals("Player", System.StringComparison.OrdinalIgnoreCase))
        {
            return "Player";
        }

        // Example: "Vesna_lowsanity"
        return $"{speaker}_{SanitySuffix()}";
    }

    private string SanitySuffix()
    {
        return _sanity switch
        {
            3 => "highsanity",
            1 => "lowsanity",
            _ => "mediumsanity"
        };
    }

    private void PlayPortraitState(string stateName)
    {
        if (_portraitAnimator == null)
        {
            return;
        }

        // If the object is active, play the animation directly
        if (_portraitAnimator.gameObject.activeInHierarchy)
        {
            int hash = Animator.StringToHash(stateName);
            // Check if the state exists to avoid warnings or hard errors
            if (_portraitAnimator.HasState(0, hash)) 
            {
                _portraitAnimator.Play(stateName, 0, 0f);
            }
            else
            {
                Debug.LogWarning($"Portrait animator does NOT have a state named '{stateName}' for character.");
            }
        }
        else
        {
            // If the object is not active, store the state to be played in the next frame
            _pendingPortraitState = stateName;
        }
    }

    private void DisplayChoices()
    {
        var currentChoices = _story.currentChoices;
        int i = 0;
        for (; i < currentChoices.Count && i < _choices.Length; i++)
        {
            _choices[i].SetActive(true);
            _choicesText[i].text = currentChoices[i].text;
        }

        for (; i < _choices.Length; i++)
        {
            _choices[i].SetActive(false);
        }

        // Auto-select the first choice for controller/keyboard navigation
        if (currentChoices.Count > 0)
        {
            EventSystem.current?.SetSelectedGameObject(_choices[0]);
        }
    }

    /// <summary>
    /// Makes a choice in the current dialogue story and continues the flow.
    /// This is called by the UI Buttons' OnClick events and by the keyboard handler.
    /// </summary>
    /// <param name="index">The index of the choice to select.</param>
    public void MakeChoice(int index)
    {
        // Only allow selection if the story hasn't advanced (e.g., via quick clicks)
        if (_story.currentChoices.Count > index)
        {
            _story.ChooseChoiceIndex(index);
            // Immediately continue the story after a choice is made
            ContinueStory(); 
        }
    }

    private void StartQuest(string questID)
    {
        EventManager.Broadcast(new StartQuestEvent(questID));
    }

    private void CompleteObjective(string questID, string objectiveID)
    {
        EventManager.Broadcast(new CompleteObjectiveEvent(questID, objectiveID));
    }
}