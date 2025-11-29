using System;
using Core.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerActivator : MonoBehaviour
{
    [SerializeField] private ObjectiveTrigger[] triggers;
    [SerializeField] private string questID;

    private QuestProgress progress;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

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

    private void OnQuestStartedEvent(QuestStartedEvent evt)
    {
        if (evt.QuestID != questID)
            return;

        progress = QuestManager.Instance.GetQuestProgressByID(evt.QuestID);
    }

    private void OnQuestUpdateEvent(QuestUpdateEvent evt)
    {
        if (progress == null || evt.QuestID != questID)
            return;

        foreach (var trigger in triggers)
        {
            if (trigger.QuestID == questID && progress.IsObjectiveActive(trigger.ObjectiveID))
            {
                trigger.gameObject.SetActive(true);
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FetchTriggers();

        if (progress == null)
            progress = QuestManager.Instance.GetQuestProgressByID(questID);

        if (progress != null)
            OnQuestUpdateEvent(new QuestUpdateEvent(questID));
    }

    private void FetchTriggers()
    {
        triggers = Array.FindAll(FindObjectsOfType<ObjectiveTrigger>(true),
            t => t.QuestID == questID);

        Debug.Log($"TRIGGER_ACTIVATOR: Found {triggers.Length} triggers for quest {questID}");
    }
}
