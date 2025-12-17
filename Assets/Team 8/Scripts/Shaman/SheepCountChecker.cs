using UnityEngine;
using UnityEngine.Events;
using Core.AI.Sheep;

/// <summary>
/// Checks the number of sheep the player has gathered and selects dialogue accordingly.
/// </summary>
public class SheepCountChecker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ShamanDialogueManager dialogueManager;
    [SerializeField] private ShamanBarrier shamanBarrier;

    [Header("Thresholds")]
    [SerializeField] private int requiredSheepCount = 3;

    [Header("Dialogue Sets")]
    [SerializeField] private ShamanDialogueManager.DialogueLine[] enoughSheepDialogue;
    [SerializeField] private ShamanDialogueManager.DialogueLine[] notEnoughSheepDialogue;

    [Header("Events")]
    public UnityEvent onEnoughSheepGathered;
    public UnityEvent onNotEnoughSheep;

    private bool hadEnoughSheep;

    /// <summary>
    /// Returns the current number of sheep in the herd.
    /// </summary>
    public int GetCurrentSheepCount()
    {
        if (SheepTracker.Instance == null)
            return 0;

        return SheepTracker.Instance.GetOrderedSheepList().Count;
    }

    /// <summary>
    /// Returns true if the player has gathered enough sheep.
    /// </summary>
    public bool HasEnoughSheep()
    {
        return GetCurrentSheepCount() >= requiredSheepCount;
    }

    /// <summary>
    /// Returns true if the player has gathered at least the specified number of sheep.
    /// </summary>
    public bool HasAtLeastSheep(int count)
    {
        return GetCurrentSheepCount() >= count;
    }

    /// <summary>
    /// Sets the appropriate dialogue based on sheep count and starts it.
    /// </summary>
    public void StartDialogueBasedOnSheepCount()
    {
        if (dialogueManager == null)
            return;

        if (HasEnoughSheep())
            SetDialogue(enoughSheepDialogue);
        else
            SetDialogue(notEnoughSheepDialogue);

        dialogueManager.StartDialogue();
    }

    /// <summary>
    /// Sets the dialogue lines on the dialogue manager.
    /// </summary>
    public void SetDialogue(ShamanDialogueManager.DialogueLine[] newLines)
    {
        if (dialogueManager == null || newLines == null || newLines.Length == 0)
            return;

        dialogueManager.lines = newLines;
    }

    /// <summary>
    /// Updates the required sheep threshold at runtime.
    /// </summary>
    public void SetRequiredSheepCount(int count)
    {
        requiredSheepCount = Mathf.Max(0, count);
    }

    /// <summary>
    /// Returns the required sheep count threshold.
    /// </summary>
    public int GetRequiredSheepCount()
    {
        return requiredSheepCount;
    }

    /// <summary>
    /// Returns how many more sheep the player needs.
    /// </summary>
    public int GetSheepDeficit()
    {
        return Mathf.Max(0, requiredSheepCount - GetCurrentSheepCount());
    }

    private void Update()
    {
        CheckSheepCountChanged();
    }

    /// <summary>
    /// Checks if sheep count crossed the threshold and fires events.
    /// </summary>
    private void CheckSheepCountChanged()
    {
        bool hasEnough = HasEnoughSheep();

        if (hasEnough && !hadEnoughSheep)
        {
            hadEnoughSheep = true;
            onEnoughSheepGathered?.Invoke();

            if (shamanBarrier != null)
                shamanBarrier.DisableBarrier();
        }
        else if (!hasEnough && hadEnoughSheep)
        {
            hadEnoughSheep = false;
            onNotEnoughSheep?.Invoke();
        }
    }

    /// <summary>
    /// Manually trigger a sheep count check and appropriate dialogue.
    /// </summary>
    public void CheckAndTriggerDialogue()
    {
        StartDialogueBasedOnSheepCount();
    }
}
