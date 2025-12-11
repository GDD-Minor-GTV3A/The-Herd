using System;
using System.Collections;
using System.Collections.Generic;
using Core.Events;

using Gameplay.Player;

using Ink.Runtime;
using TMPro;
using UnityEngine;

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
    [SerializeField] private GameObject _choicesPanel; 
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
    
    // Variable persistence storage
    private Dictionary<string, object> _inkVariableState = new Dictionary<string, object>();
    
    private static DialogueManager _instance;

    // Constants
    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";
    private const string DEFAULT_LAYOUT_STATE = "left";
    private const string NARRATOR_LAYOUT_STATE = "narrator";
    private const string SHOW_CHOICES_STATE = "showChoices";

    // Coroutine reference to handle stopping/skipping
    private Coroutine _displayLineCoroutine; 
    private bool _isTyping = false;
    
    [Header("Typewriter Settings")]
    [SerializeField] private float _typingSpeed = 0.04f;
    
    /// <summary>
    /// Gets a value indicating whether dialogue is currently playing.
    /// </summary>
    public bool IsDialoguePlaying { get; private set; } = false;

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
        
        DontDestroyOnLoad(this.gameObject);
    }


    private void Awake()
    {
        Initialize();
    }


    private void OnEnable()
    {
        EventManager.AddListener<QuestCompletedEvent>(OnQuestCompleted);
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


        if (_isTyping)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                FinishTypingImmediately();
            }
            // If we are typing, do not allow choices or continuing yet
            return; 
        }
        
        if (_story.currentChoices.Count > 0)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) { MakeChoice(0); return; }
            if (Input.GetKeyDown(KeyCode.Alpha2)) { MakeChoice(1); return; }
            if (Input.GetKeyDown(KeyCode.Alpha3)) { MakeChoice(2); return; }
            // Since choices exist, we return here so Space doesn't trigger "ContinueStory"
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ContinueStory();
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

        //Set a different PlayerControlMap during dialogue 
        if (PlayerInputHandler.Instance)
        {
            PlayerInputHandler.SwitchControlMap();
        }
        
        _story = new Story(inkJson.text);
        
        // Restore previously saved Ink variables
        RestoreInkVariables();
        
        _story.BindExternalFunction("StartQuest", (string questID) => {
            StartQuest(questID);
        });
        
        _story.BindExternalFunction("CompleteObjective", (string questID, string objectiveID) => {
            CompleteObjective(questID, objectiveID);
        });
        
        IsDialoguePlaying = true;
        _dialoguePanel.SetActive(true);
        _onDialogueFinished = onDialogueFinished;
        _layoutAnimator?.Play(DEFAULT_LAYOUT_STATE);
        
        ContinueStory();
    }

    private void ExitDialogueMode()
    {
        // Save Ink variables before exiting
        SaveInkVariables();
        
        IsDialoguePlaying = false;
        _dialoguePanel.SetActive(false);
        _dialogueText.text = string.Empty;
        _pendingPortraitState = null;
        _layoutAnimator?.Play(DEFAULT_LAYOUT_STATE); 
        
        _onDialogueFinished?.Invoke(); 
        _onDialogueFinished = null;
        
        //Switch back PlayerControlMap after dialogue is finished
        if (PlayerInputHandler.Instance)
        {
            PlayerInputHandler.SwitchControlMap();
        }
    }

    /// <summary>
    /// Saves all Ink variables to persistent storage
    /// </summary>
    private void SaveInkVariables()
    {
        if (_story == null) return;
        
        foreach (string varName in _story.variablesState)
        {
            _inkVariableState[varName] = _story.variablesState[varName];
        }
        
        Debug.Log($"Saved {_inkVariableState.Count} Ink variables");
    }

    /// <summary>
    /// Restores previously saved Ink variables to the current story
    /// </summary>
    private void RestoreInkVariables()
    {
        if (_story == null || _inkVariableState.Count == 0) return;
        
        foreach (var kvp in _inkVariableState)
        {
            try
            {
                _story.variablesState[kvp.Key] = kvp.Value;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Could not restore variable '{kvp.Key}': {e.Message}");
            }
        }
        
        Debug.Log($"Restored {_inkVariableState.Count} Ink variables");
    }

    /// <summary>
    /// Manually set an Ink variable (useful for quest completion triggers)
    /// </summary>
    /// <param name="variableName">Name of the Ink variable</param>
    /// <param name="value">Value to set</param>
    public void SetInkVariable(string variableName, object value)
    {
        _inkVariableState[variableName] = value;
        
        // If a story is currently active, also update it directly
        if (_story != null)
        {
            try
            {
                _story.variablesState[variableName] = value;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Could not set variable '{variableName}': {e.Message}");
            }
        }
    }

    /// <summary>
    /// Get the current value of an Ink variable
    /// </summary>
    public object GetInkVariable(string variableName)
    {
        if (_inkVariableState.ContainsKey(variableName))
        {
            return _inkVariableState[variableName];
        }
        return null;
    }

    private void ContinueStory()
    {
        if (!_story.canContinue)
        {
            ExitDialogueMode();
            return;
        }

        // stop any previous typing coroutine if it exists
        if (_displayLineCoroutine != null) 
        {
            StopCoroutine(_displayLineCoroutine);
        }

        string line = _story.Continue();
        HandleTags(_story.currentTags);
        
        // Hide choices while text is typing
        HideChoices(); 

        // Start the typewriter effect
        _displayLineCoroutine = StartCoroutine(ShowText(line));
    }
    
    private IEnumerator ShowText(string fullText)
    {
        _isTyping = true;
        _dialogueText.text = fullText;
        
        // Ensure the mesh is generated so we know how many characters there are
        _dialogueText.ForceMeshUpdate();

        int totalVisibleCharacters = _dialogueText.textInfo.characterCount;
        int counter = 0;

        // Start with 0 characters visible
        _dialogueText.maxVisibleCharacters = 0;

        while (counter < totalVisibleCharacters)
        {
            int visibleCount = counter % (totalVisibleCharacters + 1);
            _dialogueText.maxVisibleCharacters = visibleCount;
            
            counter++;
            yield return new WaitForSeconds(_typingSpeed);
        }

        // Ensure everything is visible at the end
        _dialogueText.maxVisibleCharacters = totalVisibleCharacters;
        
        FinishTypingLogic();
    }

    private void FinishTypingImmediately()
    {
        if (_displayLineCoroutine != null) StopCoroutine(_displayLineCoroutine);
        
        _dialogueText.maxVisibleCharacters = int.MaxValue; // Show everything
        FinishTypingLogic();
    }

    private void FinishTypingLogic()
    {
        _isTyping = false;
        DisplayChoices(); // Now that text is done, we show choices
    }

    // Helper to hide choices visually before typing starts
    private void HideChoices()
    {
        foreach (var choice in _choices)
        {
            choice.SetActive(false);
        }
        // Don't change Animator state here, wait for DisplayChoices to handle the panel animation
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
            }
        }

        ApplySpeakerAndPortrait(speaker, showPortrait);
        UpdateLayout(speaker);
    }

    /// <summary>
    /// Updates the layout animation based on the speaker
    /// </summary>
    /// <param name="speaker">The current speaker name</param>
    private void UpdateLayout(string speaker)
    {
        if (_layoutAnimator == null)
        {
            return;
        }

        // If speaker is "Narrator", use narrator layout; otherwise use left layout
        if (!string.IsNullOrEmpty(speaker) && speaker.Equals("Narrator", System.StringComparison.OrdinalIgnoreCase))
        {
            _layoutAnimator.Play(NARRATOR_LAYOUT_STATE);
        }
        else
        {
            _layoutAnimator.Play(DEFAULT_LAYOUT_STATE);
        }
    }

    private void ApplySpeakerAndPortrait(string speaker, bool showPortrait)
    {
        if (string.IsNullOrEmpty(speaker) || speaker.Equals("Narrator", System.StringComparison.OrdinalIgnoreCase))
        {
            _displayNameText.text = "Narrator";
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
        // Player has no sanity portraits
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
        if (_isTyping) return;

        var currentChoices = _story.currentChoices;
        
        if (currentChoices.Count > 0)
        {
            _layoutAnimator.Play(SHOW_CHOICES_STATE);
        }
        else
        {
            _layoutAnimator.Play(DEFAULT_LAYOUT_STATE);
        }
        
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
        Debug.Log("Completed Objective through dialogue");
    }

    public void OnObjectiveCompleted(ObjectiveCompletedEvent evt)
    {
        string objectiveID = evt.ObjectiveID;
        string inkVariableName = objectiveID + "_completed";
        if (!string.IsNullOrEmpty(inkVariableName))
        {
            SetInkVariable(inkVariableName, true);
            Debug.Log($"Set Ink variable '{inkVariableName}' to true for completed objective {objectiveID}");
        }
    }
    
    
    /// <summary>
    /// Called through the EventSystem/Manager
    /// </summary>
    public void OnQuestCompleted(QuestCompletedEvent evt)
    {
        string questID = evt.QuestID;
        string inkVariableName = questID + "_completed";
        if (!string.IsNullOrEmpty(inkVariableName))
        {
            SetInkVariable(inkVariableName, true);
            Debug.Log($"Set Ink variable '{inkVariableName}' to true for completed quest {questID}");
        }
    }
}