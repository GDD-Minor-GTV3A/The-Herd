using System;
using System.Collections.Generic;
using System.Linq;
using Core.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerActivator : MonoBehaviour
{
    // Dictionary maps QuestID -> List of triggers for that quest
    private Dictionary<string, List<ObjectiveTrigger>> questTriggers = new();

    // Keeps track of active quest progress data
    private Dictionary<string, QuestProgress> questProgress = new();
    
    private void OnEnable()
    {
        EventManager.AddListener<QuestStartedEvent>(OnQuestStartedEvent);
        EventManager.AddListener<QuestUpdateEvent>(OnQuestUpdateEvent);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        EventManager.RemoveListener<QuestStartedEvent>(OnQuestStartedEvent);
        EventManager.RemoveListener<QuestUpdateEvent>(OnQuestUpdateEvent);
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Always rebuild the trigger map for the new scene
        FetchAllTriggers();

        // Re-apply active quests to the new scene’s triggers
        foreach (var questID in questProgress.Keys)
        {
            OnQuestUpdateEvent(new QuestUpdateEvent(questID));
        }
    }

    private void OnQuestStartedEvent(QuestStartedEvent evt)
    {
        string questID = evt.QuestID;

        if (!questProgress.ContainsKey(questID))
        {
            questProgress[questID] = QuestManager.Instance.GetQuestProgressByID(questID);
        }

        Debug.Log($"TRIGGER_ACTIVATOR: Quest '{questID}' started — progress registered.");
    }

    private void OnQuestUpdateEvent(QuestUpdateEvent evt)
    {
        string questID = evt.QuestID;

        if (!questProgress.TryGetValue(questID, out var progress))
            return;

        if (!questTriggers.TryGetValue(questID, out var triggers))
            return;

        foreach (var trigger in triggers)
        {
            if (progress.IsObjectiveActive(trigger.ObjectiveID))
            {
                Debug.Log($"TRIGGER_ACTIVATOR: Activating {trigger.ObjectiveID}");
                trigger.gameObject.SetActive(true);
            }
            else
            {
                Debug.Log($"TRIGGER_ACTIVATOR: Deactivating {trigger.ObjectiveID}");
                trigger.gameObject.SetActive(false);
            }
        }
    }

    void nothingDings()
    {
        return;
    }
    
    private void FetchAllTriggers()
    {
        questTriggers.Clear();

        var allTriggers = FindObjectsOfType<ObjectiveTrigger>(true);

        foreach (var trigger in allTriggers)
        {
            if (string.IsNullOrEmpty(trigger.QuestID))
                continue;

            if (!questTriggers.ContainsKey(trigger.QuestID))
            {
                questTriggers[trigger.QuestID] = new List<ObjectiveTrigger>();
            }

            questTriggers[trigger.QuestID].Add(trigger);
        }

        Debug.Log($"TRIGGER_ACTIVATOR: Found {allTriggers.Length} total triggers across {questTriggers.Keys.Count} quests.");
    }
}
