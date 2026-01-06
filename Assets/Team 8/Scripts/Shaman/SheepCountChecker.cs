using UnityEngine;
using Core.AI.Sheep;

public class SheepCountChecker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ShamanDialogueManager dialogueManager;
    [SerializeField] private ShamanBarrier shamanBarrier;

    [Header("Settings")]
    [SerializeField] private int requiredSheepCount;
    [SerializeField] private bool isEnding;

    [Header("Dialogue")]
    [SerializeField] private ShamanDialogueManager.DialogueLine[] enoughSheepDialogue;
    [SerializeField] private ShamanDialogueManager.DialogueLine[] notEnoughSheepDialogue;

    private void Start()
    {
        if (dialogueManager != null)
            dialogueManager.onDialogueEnded.AddListener(OnDialogueEnded);
    }

    private void OnDestroy()
    {
        if (dialogueManager != null)
            dialogueManager.onDialogueEnded.RemoveListener(OnDialogueEnded);
    }

    private void OnDialogueEnded()
    {
        if (HasEnough() && shamanBarrier != null)
            shamanBarrier.Disable();
    }

    public int GetSheepCount()
    {
        return SheepTracker.Instance != null
            ? SheepTracker.Instance.GetOrderedSheepList().Count
            : 0;
    }

    public bool HasEnough()
    {
        return GetSheepCount() >= requiredSheepCount;
    }

    public void StartDialogue()
    {
        if (dialogueManager == null)
            return;

        if (HasEnough())
        {
            dialogueManager.lines = enoughSheepDialogue;
            dialogueManager.SetIsEnding(isEnding);
        }
        else
        {
            dialogueManager.lines = notEnoughSheepDialogue;
            dialogueManager.SetIsEnding(false);
        }

        dialogueManager.StartDialogue();
    }
}
