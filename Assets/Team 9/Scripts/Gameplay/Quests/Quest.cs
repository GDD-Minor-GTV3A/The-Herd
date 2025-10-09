using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Represents a quest definition stored as a ScriptableObject.
/// Contains multiple stages, each with one or more objectives.
/// </summary>
[CreateAssetMenu(menuName = "Quest/Quests")]
public class Quest : ScriptableObject
{
    [SerializeField] private string _questID;
    [SerializeField] private string _questName;
    [SerializeField] private string _questDescription;
    [SerializeField] private List<QuestStage> _stages;

    /// <summary>
    /// Gets the unique ID of the quest.
    /// </summary>
    public string QuestID => _questID;

    /// <summary>
    /// Gets the name/title of the quest.
    /// </summary>
    public string QuestName => _questName;

    /// <summary>
    /// Gets the description of the quest.
    /// </summary>
    public string QuestDescription => _questDescription;

    /// <summary>
    /// Gets the list of stages for this quest.
    /// </summary>
    public IReadOnlyList<QuestStage> Stages => _stages;
}


/// <summary>
/// Represents a stage in a quest, which contains one or more objectives.
/// Stages can be sequential or parallel.
/// </summary>
[Serializable]
public class QuestStage
{
    [SerializeField] private string _stageID;
    [SerializeField] private string _stageDescription;
    [SerializeField] private List<QuestObjective> _objectives;
    [SerializeField] private bool _isParallel = false;

    /// <summary>
    /// Gets the unique ID of the stage.
    /// </summary>
    public string StageID => _stageID;

    /// <summary>
    /// Gets the description of this stage.
    /// </summary>
    public string StageDescription => _stageDescription;

    /// <summary>
    /// Gets the objectives of this stage.
    /// </summary>
    public IReadOnlyList<QuestObjective> Objectives => _objectives;

    /// <summary>
    /// Gets whether objectives in this stage are active in parallel.
    /// </summary>
    public bool IsParallel => _isParallel;

    /// <summary>
    /// Returns true if all objectives in this stage are completed.
    /// </summary>
    public bool IsCompleted => _objectives.TrueForAll(o => o.IsCompleted);

    /// <summary>
    /// Sets all objectives in this stage as active or inactive.
    /// </summary>
    /// <param name="state">True to activate, false to deactivate.</param>
    public void SetActive(bool state)
    {
        foreach (var obj in _objectives)
            obj.SetIsActive(state);
    }
}


/// <summary>
/// Represents a single objective within a quest.
/// </summary>
[Serializable]
public class QuestObjective
{
    [SerializeField] private string objectiveID;
    [SerializeField] private string objectiveDescription;
    [SerializeField] private ObjectiveType type;
    [SerializeField] private int requiredAmount;
    [SerializeField] private int currentAmount;

    /// <summary>
    /// Gets the unique ID of the objective.
    /// </summary>
    public string ObjectiveID => objectiveID;

    /// <summary>
    /// Gets the description of this objective.
    /// </summary>
    public string ObjectiveDescription => objectiveDescription;

    /// <summary>
    /// Gets the type of this objective.
    /// </summary>
    public ObjectiveType Type => type;

    /// <summary>
    /// Gets the required amount to complete this objective.
    /// </summary>
    public int RequiredAmount => requiredAmount;

    /// <summary>
    /// Gets the current progress amount for this objective.
    /// </summary>
    public int CurrentAmount => currentAmount;

    /// <summary>
    /// Gets whether this objective is currently active.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Returns true if the current amount meets or exceeds the required amount.
    /// </summary>
    public bool IsCompleted => CurrentAmount >= RequiredAmount;

    /// <summary>
    /// Adds progress to this objective.
    /// </summary>
    /// <param name="amount">The amount to add.</param>
    public void AddProgress(int amount)
    {
        currentAmount += amount;
    }

    /// <summary>
    /// Sets the active state of this objective.
    /// </summary>
    /// <param name="state">True to activate, false to deactivate.</param>
    public void SetIsActive(bool state)
    {
        IsActive = state;
    }

    /// <summary>
    /// Returns the progress of the objective as a string (current / required).
    /// </summary>
    /// <returns>Progress string.</returns>
    public string GetProgressText()
    {
        return $"{CurrentAmount} / {RequiredAmount}";
    }

    /// <summary>
    /// Returns a string representing this objective.
    /// </summary>
    /// <returns>Objective description and progress.</returns>
    public override string ToString()
    {
        return $"{ObjectiveDescription} - {CurrentAmount} / {RequiredAmount}";
    }

    /// <summary>
    /// Constructor to create a new QuestObjective.
    /// </summary>
    /// <param name="id">Unique ID of the objective.</param>
    /// <param name="desc">Description of the objective.</param>
    /// <param name="type">Type of the objective.</param>
    /// <param name="reqAm">Required amount to complete the objective.</param>
    /// <param name="currAm">Starting progress amount.</param>
    public QuestObjective(string id, string desc, ObjectiveType type, int reqAm, int currAm)
    {
        objectiveID = id;
        objectiveDescription = desc;
        this.type = type;
        requiredAmount = reqAm;
        currentAmount = currAm;
    }
}


/// <summary>
/// Enum representing different types of objectives.
/// </summary>
public enum ObjectiveType { CollectItem, DefeatEnemy, ReachLocation, TalkNPC, Custom }


/// <summary>
/// Tracks the progress of a quest instance.
/// Clones objectives from the ScriptableObject to track runtime progress.
/// </summary>
[Serializable]
public class QuestProgress
{
    /// <summary>
    /// The quest being tracked.
    /// </summary>
    public Quest Quest;

    /// <summary>
    /// List of stage progress objects for this quest.
    /// </summary>
    public List<QuestStageProgress> StageProgresses;

    private int _currentStageIndex = 0;

    /// <summary>
    /// Creates a new QuestProgress instance from a quest definition.
    /// </summary>
    /// <param name="quest">The quest to track.</param>
    public QuestProgress(Quest quest)
    {
        Quest = quest;
        StageProgresses = new List<QuestStageProgress>();

        foreach (var stage in quest.Stages)
            StageProgresses.Add(new QuestStageProgress(stage));

        // Activate first stage
        if (StageProgresses.Count > 0)
            StageProgresses[0].SetActive(true);
    }

    /// <summary>
    /// Adds progress to an objective in the current stage.
    /// </summary>
    /// <param name="objectiveID">The objective ID to update.</param>
    /// <param name="amount">The amount of progress to add.</param>
    public void AddProgress(string objectiveID, int amount)
    {
        var currentStage = GetCurrentStage();
        currentStage?.AddProgress(objectiveID, amount);

        if (currentStage != null && currentStage.IsCompleted)
            AdvanceStage();
    }

    /// <summary>
    /// Advances to the next stage, deactivating the previous stage.
    /// </summary>
    private void AdvanceStage()
    {
        GetCurrentStage()?.SetActive(false);

        _currentStageIndex++;
        if (_currentStageIndex < StageProgresses.Count)
            GetCurrentStage()?.SetActive(true);
    }

    /// <summary>
    /// Gets the current stage being progressed.
    /// </summary>
    /// <returns>The current stage progress, or null if none.</returns>
    private QuestStageProgress GetCurrentStage()
    {
        if (_currentStageIndex < 0 || _currentStageIndex >= StageProgresses.Count)
            return null;
        return StageProgresses[_currentStageIndex];
    }

    /// <summary>
    /// Returns true if all stages and objectives in the quest are completed.
    /// </summary>
    public bool IsCompleted => StageProgresses.TrueForAll(s => s.IsCompleted);
}

/// <summary>
/// Tracks the progress of a single quest stage.
/// Manages activation and progress of objectives in that stage.
/// </summary>
[Serializable]
public class QuestStageProgress
{
    /// <summary>
    /// The original stage definition.
    /// </summary>
    public QuestStage Stage;

    /// <summary>
    /// Cloned objectives for tracking runtime progress.
    /// </summary>
    public List<QuestObjective> Objectives;

    /// <summary>
    /// Creates a new QuestStageProgress instance from a stage definition.
    /// </summary>
    /// <param name="stage">The stage to track.</param>
    public QuestStageProgress(QuestStage stage)
    {
        Stage = stage;
        Objectives = new List<QuestObjective>();

        foreach (var obj in stage.Objectives)
        {
            Objectives.Add(new QuestObjective(
                obj.ObjectiveID,
                obj.ObjectiveDescription,
                obj.Type,
                obj.RequiredAmount,
                0
            ));
        }
    }

    /// <summary>
    /// Sets the stage objectives active or inactive based on stage type.
    /// </summary>
    /// <param name="state">True to activate, false to deactivate.</param>
    public void SetActive(bool state)
    {
        if (Stage.IsParallel)
        {
            foreach (var o in Objectives)
                o.SetIsActive(state);
        }
        else
        {
            if (state && Objectives.Count > 0)
                Objectives[0].SetIsActive(true);
        }
    }

    /// <summary>
    /// Adds progress to an active objective in this stage.
    /// </summary>
    /// <param name="objectiveID">The ID of the objective to update.</param>
    /// <param name="amount">The amount to add to progress.</param>
    public void AddProgress(string objectiveID, int amount)
    {
        var activeObj = Objectives.Find(o => o.ObjectiveID == objectiveID && o.IsActive);
        if (activeObj == null) return;

        activeObj.AddProgress(amount);

        if (activeObj.IsCompleted)
        {
            activeObj.SetIsActive(false);

            if (!Stage.IsParallel)
            {
                int i = Objectives.IndexOf(activeObj);
                if (i + 1 < Objectives.Count)
                    Objectives[i + 1].SetIsActive(true);
            }
        }
    }

    /// <summary>
    /// Returns true if all objectives in this stage are completed.
    /// </summary>
    public bool IsCompleted => Objectives.TrueForAll(o => o.IsCompleted);
}
