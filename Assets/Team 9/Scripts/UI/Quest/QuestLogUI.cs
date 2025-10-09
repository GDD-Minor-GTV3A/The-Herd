using System;
using System.Collections.Generic;

using Core.Events;

using UnityEngine;
using TMPro;
public class QuestLogUI : MonoBehaviour
{

    [SerializeField] private Transform questListContainer;
    [SerializeField] private GameObject questEntryPrefab;

    private readonly Dictionary<string, QuestUIEntry> _questEntries = new();

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
        entry.Setup(quest);
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
        if (!_questEntries.ContainsKey(evt.QuestID))
        {
            Debug.LogWarning($"Quest entry for {evt.QuestID} does not exist!");
            return;
        }

        var questUiEntry = _questEntries[evt.QuestID];
        
        questUiEntry.MarkCompleted();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
