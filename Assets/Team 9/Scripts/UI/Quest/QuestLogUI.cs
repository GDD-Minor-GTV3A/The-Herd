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

    public static QuestLogUI Instance { get; private set; }
    
    public void Initialize()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        questLogUI.gameObject.SetActive(false);
        
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

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Subscribes to quest-related events when this component is enabled.
    /// </summary>
    private void OnEnable()
    {
        EventManager.AddListener<QuestStartedEvent>(OnQuestStartedEvent);
        EventManager.AddListener<QuestUpdateEvent>(OnQuestUpdateEvent);
        EventManager.AddListener<QuestCompletedEvent>(OnQuestCompletedEvent);
    }

    /// <summary>
    /// Unsubscribes from quest-related events when this component is disabled.
    /// </summary>
    private void OnDisable()
    {
        EventManager.RemoveListener<QuestStartedEvent>(OnQuestStartedEvent);
        EventManager.RemoveListener<QuestUpdateEvent>(OnQuestUpdateEvent);
        EventManager.RemoveListener<QuestCompletedEvent>(OnQuestCompletedEvent);
    }

    /// <summary>
    /// Handles the <see cref="QuestStartedEvent"/> by creating and displaying a new quest entry in the UI.
    /// </summary>
    /// <param name="evt">The event data containing the ID of the started quest.</param>
    private void OnQuestStartedEvent(QuestStartedEvent evt)
    {
        var quest = QuestManager.Instance.GetQuestProgressByID(evt.QuestID);
        if (quest == null) return;

        var entryGO = Instantiate(questEntryPrefab, questListContainer);
        var entry = entryGO.GetComponent<QuestUIEntry>();
        entry.Setup(quest);
        _questEntries.Add(quest.Quest.QuestID, entry);
    }

    /// <summary>
    /// Handles the <see cref="QuestUpdateEvent"/> by refreshing the UI for the corresponding quest.
    /// </summary>
    /// <param name="evt">The event data containing the ID of the updated quest.</param>
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

    /// <summary>
    /// Handles the <see cref="QuestCompletedEvent"/> by marking the quest as completed in the UI.
    /// </summary>
    /// <param name="evt">The event data containing the ID of the completed quest.</param>
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
        
        //Maybe rework this for new log
        Destroy(_questEntries[evt.QuestID].gameObject);
        _questEntries.Remove(evt.QuestID);
    }

    private void Update()
    {
        //Enable/Disable QuestLog
        if (Input.GetKeyDown(KeyCode.L))
        {
            _activeState = !_activeState;
            
            questLogUI.gameObject.SetActive(_activeState);
                
            foreach (Transform child in questListContainer.transform)
            {
                child.gameObject.SetActive(_activeState);
            }

            if (_activeState)
            {
                foreach (var entry in _questEntries.Keys)
                {
                    EventManager.Broadcast(new QuestUpdateEvent(entry));
                }
            }
        }
    }
}
