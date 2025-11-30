using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Handles the UI representation of a single quest entry.
/// Uses a persistent header approach to fix layout spacing issues, 
/// with a 2-phase fade transition for seamless stage updates.
/// </summary>
public class QuestUIEntry : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI questNameText;
    [SerializeField] private TextMeshProUGUI questDescriptionText;
    [SerializeField] private Transform objectiveListContainer;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject objectiveEntryPrefab; 
    
    // The Data
    private QuestProgress _quest;
    private int _lastVisualStageIndex = -1;

    // UI References
    private TextMeshProUGUI _currentHeaderEntry; // Persistent header
    private readonly Dictionary<string, TextMeshProUGUI> _objectiveTexts = new();
    
    // State Tracking
    private readonly HashSet<string> _completedObjectives = new();
    private bool _isAnimatingTransition = false;

    public void Setup(QuestProgress quest)
    {
        _quest = quest;
        questNameText.text = quest.Quest.QuestName;
        questDescriptionText.text = quest.Quest.QuestDescription;

        // Clear container
        foreach (Transform child in objectiveListContainer)
            Destroy(child.gameObject);
            
        _objectiveTexts.Clear();
        _completedObjectives.Clear();
        _currentHeaderEntry = null;
        _isAnimatingTransition = false;

        // Determine the current active stage index
        int activeStageIndex = GetActiveStageIndex();
        _lastVisualStageIndex = activeStageIndex;

        // 1. Create the persistent Header (if quest has multiple stages)
        // We create this ONCE and never destroy it, only fade/change text.
        if (_quest.StageProgresses.Count > 1)
        {
            CreateHeader(quest.StageProgresses[activeStageIndex].Stage.StageDescription);
        }

        // 2. Instantiate the CURRENT stage's objectives
        var stage = quest.StageProgresses[activeStageIndex];
        foreach (var obj in stage.Objectives)
        {
            if (obj.IsCompleted)
            {
                // If loaded as completed, mark it so we don't re-animate it later
                _completedObjectives.Add(obj.ObjectiveID);
            }
            else
            {
                CreateObjective(obj, startVisible: true);
            }
        }
        
        gameObject.SetActive(false);
    }

    public void RefreshObjectives()
    {
        if (_quest == null || _isAnimatingTransition) return;

        int currentStageIndex = GetActiveStageIndex();
        
        // Scenario A: Same Stage Update
        if (currentStageIndex == _lastVisualStageIndex)
        {
            UpdateCurrentStageObjectives();
        }
        // Scenario B: New Stage Transition
        else
        {
            StartCoroutine(AnimateStageTransition(_lastVisualStageIndex, currentStageIndex));
            _lastVisualStageIndex = currentStageIndex;
        }
    }

    private int GetActiveStageIndex()
    {
        for (int i = 0; i < _quest.StageProgresses.Count; i++)
        {
            foreach(var obj in _quest.StageProgresses[i].Objectives)
            {
                if (obj.IsActive) return i;
            }
        }
        return Mathf.Clamp(_quest.StageProgresses.Count - 1, 0, 100);
    }

    private void UpdateCurrentStageObjectives()
    {
        var stage = _quest.StageProgresses[_lastVisualStageIndex];

        foreach (var obj in stage.Objectives)
        {
            if (_objectiveTexts.TryGetValue(obj.ObjectiveID, out var textComp))
            {
                // Update Progress Text
                textComp.text = $"{obj.ObjectiveDescription} ({obj.CurrentAmount}/{obj.RequiredAmount})";

                // Handle Single Objective Completion
                if (obj.IsCompleted && !_completedObjectives.Contains(obj.ObjectiveID))
                {
                    _completedObjectives.Add(obj.ObjectiveID);
                    StartCoroutine(AnimateSingleObjectiveCompletion(obj.ObjectiveID, textComp));
                }
            }
            else if (obj.IsActive && !obj.IsCompleted)
            {
                // New parallel objective appeared
                CreateObjective(obj, startVisible: true);
            }
        }
    }

    // --- TRANSITION LOGIC ---

    private IEnumerator AnimateStageTransition(int oldStageIndex, int newStageIndex)
    {
        _isAnimatingTransition = true;

        var oldStage = _quest.StageProgresses[oldStageIndex];
        var newStage = _quest.StageProgresses[newStageIndex];

        // 1. Identify Old Elements
        List<TextMeshProUGUI> oldUiElements = new List<TextMeshProUGUI>();
        foreach(var obj in oldStage.Objectives)
        {
            if (_objectiveTexts.TryGetValue(obj.ObjectiveID, out var t))
            {
                // Turn Green immediately
                t.color = Color.green; 
                t.text = $"{obj.ObjectiveDescription} ({obj.RequiredAmount}/{obj.RequiredAmount})";
                oldUiElements.Add(t);
            }
        }

        // 2. Wait 2 Seconds (User reads the success)
        yield return new WaitForSeconds(2.0f);

        // 3. PHASE ONE: FADE OUT OLD (Including Header)
        // We fade the Header text out too, so we don't see the text swap happen.
        float fadeOutDuration = 0.5f;
        float elapsed = 0f;
        
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);

            // Fade Header
            if (_currentHeaderEntry != null) _currentHeaderEntry.alpha = alpha;
            
            // Fade Old Objectives
            foreach (var t in oldUiElements) if (t != null) t.alpha = alpha;

            yield return null;
        }

        // 4. CLEANUP & SWAP (Everything is invisible now)
        
        // Destroy Old Objectives so they stop taking up layout space
        foreach (var t in oldUiElements) if (t != null) Destroy(t.gameObject);
        foreach (var obj in oldStage.Objectives) _objectiveTexts.Remove(obj.ObjectiveID);

        // Update Header Text (While invisible)
        if (_currentHeaderEntry != null)
        {
            _currentHeaderEntry.text = $"<b>{newStage.Stage.StageDescription}</b>";
        }

        // Prepare New Objectives (Invisible)
        List<TextMeshProUGUI> newUiElements = new List<TextMeshProUGUI>();
        foreach (var obj in newStage.Objectives)
        {
            if (obj.IsActive)
            {
                var newText = CreateObjective(obj, startVisible: false);
                newUiElements.Add(newText);
            }
        }

        // Wait a single frame to allow Unity's VerticalLayoutGroup to recalculate 
        // now that old items are destroyed and new ones are instantiated.
        yield return null;

        // 5. PHASE TWO: FADE IN NEW (Including Header)
        float fadeInDuration = 0.5f;
        elapsed = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);

            // Fade Header Back In
            if (_currentHeaderEntry != null) _currentHeaderEntry.alpha = alpha;

            // Fade New Objectives In
            foreach (var t in newUiElements) if (t != null) t.alpha = alpha;

            yield return null;
        }

        // Ensure fully visible
        if (_currentHeaderEntry != null) _currentHeaderEntry.alpha = 1f;
        foreach (var t in newUiElements) if (t != null) t.alpha = 1f;

        _isAnimatingTransition = false;
    }

    private IEnumerator AnimateSingleObjectiveCompletion(string id, TextMeshProUGUI textComp)
    {
        // 1. Green
        textComp.color = Color.green;
        yield return new WaitForSeconds(2.0f);

        // 2. Fade Out
        float duration = 0.5f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            if (textComp == null) yield break;
            elapsed += Time.deltaTime;
            textComp.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }

        // 3. Destroy
        if (textComp != null) Destroy(textComp.gameObject);
        if (_objectiveTexts.ContainsKey(id)) _objectiveTexts.Remove(id);
    }

    // --- HELPERS ---

    private void CreateHeader(string title)
    {
        var go = Instantiate(objectiveEntryPrefab, objectiveListContainer);
        _currentHeaderEntry = go.GetComponent<TextMeshProUGUI>();
        _currentHeaderEntry.text = $"<b>{title}</b>";
        _currentHeaderEntry.color = new Color32(97, 59, 59, 255);
        
        go.transform.SetAsFirstSibling();
        // Zero margin because this is the persistent top element
        _currentHeaderEntry.margin = Vector4.zero;
    }

    private TextMeshProUGUI CreateObjective(QuestObjective obj, bool startVisible)
    {
        var go = Instantiate(objectiveEntryPrefab, objectiveListContainer);
        var text = go.GetComponent<TextMeshProUGUI>();
        
        text.text = $"{obj.ObjectiveDescription} ({obj.CurrentAmount}/{obj.RequiredAmount})";
        text.color = obj.IsActive ? Color.white : new Color32(100, 100, 100, 255);

        if (!startVisible) text.alpha = 0f;

        _objectiveTexts[obj.ObjectiveID] = text;
        return text;
    }

    public void MarkCompleted()
    {
        questNameText.text += " X";
        questNameText.color = Color.green;
        questDescriptionText.color = Color.green;
    }
}