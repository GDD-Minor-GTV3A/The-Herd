using System;
using System.Collections.Generic;
using Core.Events;
using UnityEngine;
using TMPro;

/// <summary>
/// Handles the UI representation of a single quest entry in the quest log.
/// Displays the quest name, description, and objective progress dynamically.
/// </summary>
public class QuestUIEntry : MonoBehaviour
{
    /// <summary>
    /// Reference to the text component that displays the quest name.
    /// </summary>
    [SerializeField] private TextMeshProUGUI questNameText;
    /// <summary>
    /// Reference to the text component that displays the quest description.
    /// </summary>
    [SerializeField] private TextMeshProUGUI questDescriptionText;
    /// <summary>
    /// Container transform used to hold all instantiated objective entries.
    /// </summary>
    [SerializeField] private Transform objectiveListContainer;
    /// <summary>
    /// Prefab used for displaying individual objective entries (TextMeshProUGUI).
    /// </summary>
    [SerializeField] private GameObject objectiveEntryPrefab;
    
    /// <summary>
    /// The current quest progress data being displayed by this UI entry.
    /// </summary>
    private QuestProgress _quest;

    /// <summary>
    /// Cached dictionary mapping objective IDs to their corresponding text UI components.
    /// </summary>
    private readonly Dictionary<string, TextMeshProUGUI> _objectiveTexts = new();

    
    /// <summary>
    /// Initializes this quest UI entry using the specified quest progress data.
    /// Creates and populates UI elements for each objective.
    /// </summary>
    /// <param name="quest">The quest progress data to display.</param>
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
            // Optional: add stage header if multiple stages exist
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
        
        this.gameObject.SetActive(false);
    }
    

    /// <summary>
    /// Updates the UI to reflect the latest progress of the quest objectives.
    /// Creates new UI entries for any newly activated objectives.
    /// </summary>
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
                Debug.Log($"Refreshing obj: {obj.ObjectiveID}, Desc: {obj.ObjectiveDescription}, Active: {obj.IsActive}, Completed: {obj.IsCompleted}");
                if (obj.IsCompleted)
                    text.color = Color.green;
                else if (obj.IsActive)
                    text.color = Color.black;
                else
                    text.color = Color.red;
            }
        }
    }

    
    /// <summary>
    /// Marks this quest as completed visually by changing text color and adding a completion indicator.
    /// </summary>
    public void MarkCompleted()
    {
        questNameText.text += " X";
        questNameText.color = Color.green;
        questDescriptionText.color = Color.green;
    }
    
    
}
