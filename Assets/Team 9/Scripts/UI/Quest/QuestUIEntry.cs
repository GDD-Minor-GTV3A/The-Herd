using System;
using System.Collections.Generic;

using Core.Events;

using UnityEngine;
using TMPro;

public class QuestUIEntry : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI questNameText;
    [SerializeField] private TextMeshProUGUI questDescriptionText;
    [SerializeField] private Transform objectiveListContainer;
    [SerializeField] private GameObject objectiveEntryPrefab;

    private QuestProgress _quest;
    private readonly Dictionary<string, TextMeshProUGUI> _objectiveTexts = new();
    
    
    public void Setup(QuestProgress quest)
    {
        _quest = quest;
        questNameText.text = quest.Quest.QuestName;
        questDescriptionText.text = quest.Quest.QuestDescription;

        // Clear existing entries (if this UI is reused)
        foreach (Transform child in objectiveListContainer)
            Destroy(child.gameObject);
        _objectiveTexts.Clear();

        // Iterate through all stages and objectives
        foreach (var stage in quest.StageProgresses)
        {
            // Optional: add stage header
            if (quest.StageProgresses.Count > 1)
            {
                var stageHeader = Instantiate(objectiveEntryPrefab, objectiveListContainer);
                var headerText = stageHeader.GetComponent<TextMeshProUGUI>();
                headerText.text = $"<b>{stage.Stage.StageDescription}</b>";
                headerText.color = Color.blue;
            }

            foreach (var obj in stage.Objectives)
            {
                var entry = Instantiate(objectiveEntryPrefab, objectiveListContainer);
                var text = entry.GetComponent<TextMeshProUGUI>();
                text.text = $"{obj.ObjectiveDescription} ({obj.CurrentAmount}/{obj.RequiredAmount})";
            
                // Color logic
                if (obj.IsCompleted)
                    text.color = Color.green;
                else if (obj.IsActive)
                    text.color = Color.black;
                else
                    text.color = Color.red;

                _objectiveTexts[obj.ObjectiveID] = text;
            }
        }
    }
    
    
    public void RefreshObjectives()
    {
        if (_quest == null)
            return;

        foreach (var stage in _quest.StageProgresses)
        {
            foreach (var obj in stage.Objectives)
            {
                if (!_objectiveTexts.TryGetValue(obj.ObjectiveID, out var text))
                {
                    // New objective became active (for parallel/next stage)
                    var entry = Instantiate(objectiveEntryPrefab, objectiveListContainer);
                    text = entry.GetComponent<TextMeshProUGUI>();
                    _objectiveTexts[obj.ObjectiveID] = text;
                }

                text.text = $"{obj.ObjectiveDescription} ({obj.CurrentAmount}/{obj.RequiredAmount})";
            
                if (obj.IsCompleted)
                    text.color = Color.green;
                else if (obj.IsActive)
                    text.color = Color.black;
                else
                    text.color = Color.red;
            }
        }
    }

    public void MarkCompleted()
    {
        questNameText.text += " X";
        questNameText.color = Color.green;
        questDescriptionText.color = Color.green;
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
