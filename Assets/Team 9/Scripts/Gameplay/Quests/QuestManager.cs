using System.Collections.Generic;
using System.Linq;

using Core.Events;

using UnityEngine;

/// <summary>
/// Manager class to handle active quests and updates
/// </summary>
public class QuestManager : MonoBehaviour
{
    [SerializeField] private List<Quest> _allQuests = new List<Quest>();

    private List<QuestProgress> _activeQuests = new List<QuestProgress>();
    private List<QuestProgress> _completedQuests = new List<QuestProgress>();


    /// <summary>
    /// Singleton instance of the QuestManager.
    /// </summary>
    public static QuestManager Instance { get; private set; }


    /// <summary>
    /// Initialise the singleton instance.
    /// </summary>
    public void Initialize()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    /// <summary>
    /// Adding event listeners
    /// </summary>
    private void OnEnable()
    {
        EventManager.AddListener<StartQuestEvent>(OnStartQuestEvent);
        EventManager.AddListener<CompleteObjectiveEvent>(OnCompleteObjectiveEvent);
    }

    /// <summary>
    /// Removin event listeners
    /// </summary>
    private void OnDisable()
    {
        EventManager.RemoveListener<StartQuestEvent>(OnStartQuestEvent);
        EventManager.RemoveListener<CompleteObjectiveEvent>(OnCompleteObjectiveEvent);
    }

    /// <summary>
    /// Starts a new quest and adds it to active quest list
    /// </summary>
    /// <param name="quest">Quest ScriptableObject to start</param>
    public void StartQuest(Quest quest)
    {
        var progress = new QuestProgress(quest);
        _activeQuests.Add(progress);
    }

    private void OnStartQuestEvent(StartQuestEvent evt)
    {
        Debug.Log($"Starting Quest: {evt.QuestID}");
        bool checker = false;
        if (CheckIfQuestRunningOrComplete(evt.QuestID))
        {
            Debug.Log("QUEST IS ALREADY RUNNING OR COMPLETED!!!");
            return;
        }
        var quest = GetQuestByID(evt.QuestID);
        StartQuest(quest);
    }

    /// <summary>
    /// Gets called when the CompleteObjectiveEvent is triggered
    /// </summary>
    /// <param name="evt"></param>
    private void OnCompleteObjectiveEvent(CompleteObjectiveEvent evt)
    {
        CompleteObjective(evt.QuestID, evt.ObjectiveID, 1);
    }


    /// <summary>
    /// Checks if a Quest is running or completed
    /// </summary>
    /// <param name="questID"> QuestID </param>
    /// <returns> true = quest is running or complete/ false = quest hasn't been started yet </returns>
    private bool CheckIfQuestRunningOrComplete(string questID)
    {
        bool result = _activeQuests.Exists(q => q.Quest.QuestID == questID);
        if (!result)
            result = _completedQuests.Exists(q => q.Quest.QuestID == questID);
        return result;
    }


    /// <summary>
    /// Completes progress on a specific objective within a quest.
    /// </summary>
    /// <param name="questID">The ID of the quest to update.</param>
    /// <param name="objectiveID">The ID of the objective to complete.</param>
    /// <param name="amount">The amount to increment the objective's progress (default 1).</param>
    private void CompleteObjective(string questID, string objectiveID, int amount = 1)
    {
        foreach (var quest in _activeQuests)
        {
            if (quest.Quest.QuestID != questID) continue;

            var obj = quest.Objectives.FirstOrDefault(o => o.ObjectiveID == objectiveID);

            if (obj == null)
            {
                Debug.LogWarning("QUEST MANAGER: Quest Objective is null!");
                return;
            }

            obj.AddProgress(amount);
            Debug.Log($"Current Progress: {obj.CurrentAmount} / {obj.RequiredAmount}");
            if (quest.IsCompleted)
            {
                OnQuestCompleted(quest);
                Debug.Log("QUEST COMPLETE");
            }
            //TODO: QUEST UI UPDATE
        }
    }


    /// <summary>
    /// Returns a Quest by it's ID
    /// </summary>
    /// <param name="questID"></param>
    /// <returns></returns>
    public Quest GetQuestByID(string questID)
    {
        var quest = _allQuests?.Find(q => q.QuestID == questID);
        return quest;
    }


    /// <summary>
    /// Retrieves a quest by its ID from active or completed quests.
    /// </summary>
    /// <param name="questID">The unique identifier of the quest.</param>
    /// <returns>
    /// The <see cref="QuestProgress"/> object if found; otherwise, null.
    /// </returns>
    public QuestProgress GetQuestProgressByID(string questID)
    {
        var questProg = _activeQuests.Find(q => q.Quest.QuestID == questID)
            ?? _completedQuests.Find(q => q.Quest.QuestID == questID);
        return questProg;
    }


    /// <summary>
    /// Returns a List of all Objective descriptions of a quest
    /// </summary>
    /// <param name="questID"></param>
    /// <returns>List<string></returns>
    public List<string> GetAllQuestObjectiveDescriptions(string questID)
    {
        var questObjectives = GetQuestProgressByID(questID)?.Objectives;

        if (questObjectives == null)
            return new List<string>();

        List<string> questObjDescriptionList = new List<string>();
        foreach (var objective in questObjectives)
        {
            questObjDescriptionList.Add(objective.ObjectiveDescription);
        }

        return questObjDescriptionList;
    }


    /// <summary>
    /// For testing CompleteObjective with UI-Buttons
    /// To be Removed
    /// </summary>
    /// <param name="objectiveID"></param>
    public void CompleteObjectiveString(string objectiveID)
    {
        CompleteObjective("TESTQUEST_001", objectiveID, 1);
    }


    /// <summary>
    /// Called when a quest is completed.
    /// Moves quest from active quests to completed quests.
    /// Handles completion logic.
    /// </summary>
    /// <param name="quest">The completed QuestProgress object</param>
    private void OnQuestCompleted(QuestProgress quest)
    {
        _activeQuests.Remove(quest);
        _completedQuests.Add(quest);

        EventManager.Broadcast(new QuestCompletedEvent(quest.Quest.QuestID));
        Debug.Log($"Quest completed: {quest.Quest.QuestName}");
        //TODO: Get a reward????
    }
}