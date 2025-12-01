using System;
using System.Collections.Generic;
using Core.Events;
using UnityEngine;
using TMPro;

/// <summary>
/// Manages the quest log UI, handling quest entries as they are started, updated, and completed.
/// Listens for quest-related events and updates the UI accordingly.
/// </summary>
public class QuestLogUI : MonoBehaviour
{
    /// <summary>
    /// The container Transform that holds all instantiated quest entry UI elements.
    /// </summary>
    [SerializeField] private Transform questListContainer;

    [SerializeField] private Transform questLogUI;
    
    /// <summary>
    /// The prefab used to create a new quest UI entry when a quest starts.
    /// </summary>
    [SerializeField] private GameObject questEntryPrefab;

    /// <summary>
    /// A dictionary that maps quest IDs to their corresponding <see cref="QuestUIEntry"/> components.
    /// </summary>
    private readonly Dictionary<string, QuestUIEntry> _questEntries = new();

    private bool _activeState = false;
    private CanvasGroup _questLogCanvasGroup;

    public static QuestLogUI Instance { get; private set; }
    
    public void Initialize()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Get or Add CanvasGroup to handle visibility without disabling scripts
        if (questLogUI != null)
        {
            _questLogCanvasGroup = questLogUI.GetComponent<CanvasGroup>();
            if (_questLogCanvasGroup == null)
            {
                _questLogCanvasGroup = questLogUI.gameObject.AddComponent<CanvasGroup>();
            }
        }
        
        // Hide on startup (Visuals only, objects remain active)
        SetQuestLogVisibility(false);
        
        Instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }

    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        if (Instance == this)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnEnable()
    {
        EventManager.AddListener<QuestStartedEvent>(OnQuestStartedEvent);
        EventManager.AddListener<QuestUpdateEvent>(OnQuestUpdateEvent);
        EventManager.AddListener<QuestCompletedEvent>(OnQuestCompletedEvent);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener<QuestStartedEvent>(OnQuestStartedEvent);
        EventManager.RemoveListener<QuestUpdateEvent>(OnQuestUpdateEvent);
        EventManager.RemoveListener<QuestCompletedEvent>(OnQuestCompletedEvent);
    }

    private void OnQuestStartedEvent(QuestStartedEvent evt)
    {
        var quest = QuestManager.Instance.GetQuestProgressByID(evt.QuestID);
        if (quest == null) return;

        var entryGO = Instantiate(questEntryPrefab, questListContainer);
        var entry = entryGO.GetComponent<QuestUIEntry>();
        
        // 1. Setup the data (Internal script logic runs)
        entry.Setup(quest);

        // 2. FORCE ACTIVE: QuestUIEntry.Setup() disables the GameObject by default.
        // We must re-enable it so its scripts/coroutines run in the background, 
        // while the parent CanvasGroup keeps it visually hidden.
        entryGO.SetActive(true);

        _questEntries.Add(quest.Quest.QuestID, entry);
    }

    private void OnQuestUpdateEvent(QuestUpdateEvent evt)
    {
        if (!_questEntries.ContainsKey(evt.QuestID))
        {
            Debug.LogWarning($"Quest entry for {evt.QuestID} does not exist!");
            return;
        }

        var questUiEntry = _questEntries[evt.QuestID];
        questUiEntry.RefreshObjectives();
    }

    private void OnQuestCompletedEvent(QuestCompletedEvent evt)
    {
        Debug.Log("UI COMPLETE CALLED");
        if (!_questEntries.ContainsKey(evt.QuestID))
        {
            Debug.LogWarning($"Quest entry for {evt.QuestID} does not exist!");
            return;
        }

        Debug.Log($"{evt.QuestID} has been found");

        var questUiEntry = _questEntries[evt.QuestID];
        questUiEntry.MarkCompleted();
        
        // If you want the "Completion Fade Out" coroutine inside QuestUIEntry to run,
        // you should delay this Destroy or let the Entry destroy itself.
        // For now, we destroy immediately as per original logic.
        Destroy(_questEntries[evt.QuestID].gameObject);
        _questEntries.Remove(evt.QuestID);
    }

    private void Update()
    {
        // Toggle QuestLog Visibility
        if (Input.GetKeyDown(KeyCode.L))
        {
            _activeState = !_activeState;
            SetQuestLogVisibility(_activeState);
        }
    }

    /// <summary>
    /// Controls visibility via Alpha/Raycasting. 
    /// Does NOT disable child objects, so their scripts continue to run.
    /// </summary>
    private void SetQuestLogVisibility(bool isVisible)
    {
        if (_questLogCanvasGroup != null)
        {
            _questLogCanvasGroup.alpha = isVisible ? 1f : 0f;
            _questLogCanvasGroup.interactable = isVisible;
            _questLogCanvasGroup.blocksRaycasts = isVisible;
        }
        else
        {
            // Fallback for safety
            questLogUI.gameObject.SetActive(isVisible);
        }
    }
}