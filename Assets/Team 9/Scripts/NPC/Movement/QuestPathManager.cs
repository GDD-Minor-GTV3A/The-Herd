using System;
using System.Collections.Generic;

using Core.Events;

using UnityEngine;

public class QuestPathManager : MonoBehaviour
{
    [SerializeField] private NpcPath[] allPaths;
    [SerializeField] private string questID;
    [SerializeField] private NpcPathMovement npc;

    private NpcPath currentPath;
    
    private QuestProgress _quest;

    private void OnEnable()
    {
        EventManager.AddListener<QuestStartedEvent>(OnQuestStartedEvent);
        EventManager.AddListener<CompleteObjectiveEvent>(OnCompleteObjectiveEvent);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener<QuestStartedEvent>(OnQuestStartedEvent);
        EventManager.RemoveListener<CompleteObjectiveEvent>(OnCompleteObjectiveEvent);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnCompleteObjectiveEvent(CompleteObjectiveEvent evt)
    {
        _quest = QuestManager.Instance.GetQuestProgressByID(questID);
        if (_quest == null)
        {
            Debug.Log($"QUEST_PATH_MAN: No Quest");
            return;
        }
        if (!_quest.IsObjectiveActive(evt.ObjectiveID) && !_quest.IsObjectiveCompleted(evt.ObjectiveID))
        {
            Debug.Log($"QUEST_PATH_MAN: Obj={evt.ObjectiveID}, Active={_quest.IsObjectiveActive(evt.ObjectiveID)}, Completed={_quest.IsObjectiveCompleted(evt.ObjectiveID)}");
            Debug.Log($"QUEST_PATH_MAN: Objective {evt.ObjectiveID} not active or completed");
            return;
        }

        if (npc.IsRunning)
        {
            Debug.Log("QUEST_PATH_MAN: NPC already running");
            return;
        }
        
        foreach (var path in allPaths)
        {
            if (evt.QuestID != path.QuestID)
            {
                Debug.Log($"QUEST_PATH_MAN: {evt.QuestID} != {path.QuestID}");
                continue;
                
            }
            if (evt.ObjectiveID != path.ObjectiveID)
            {
                Debug.Log($"QUEST_PATH_MAN: {evt.ObjectiveID} != {path.ObjectiveID}");
                continue;
            }
            
            npc.SetNewPath(path);
            Debug.Log("IM HERE");
        }
        
        
        npc.StartPath();
    }

    private void OnQuestStartedEvent(QuestStartedEvent evt)
    {
        if (evt.QuestID != questID) return;
        
        _quest = QuestManager.Instance.GetQuestProgressByID(questID);
        
        if (_quest == null)
            Debug.LogWarning("QUEST_PATH_MAN: No Quest progress found");
    }
    
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
