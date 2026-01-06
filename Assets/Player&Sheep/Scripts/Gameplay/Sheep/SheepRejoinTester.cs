using UnityEngine;
using Core.AI.Sheep;

[DisallowMultipleComponent]
public sealed class SheepRejoinTester : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private SheepStateManager sheep;

    [Header("Behavior")]
    [SerializeField] private bool markAsStragglerOnStart = true;
    [SerializeField] private float graceAfterJoin = 3f;
    [SerializeField] private bool clearThreatsOnJoin = true;

    [Header("Hotkey")]
    [SerializeField] private KeyCode rejoinHotkey = KeyCode.R;

    private void Reset()
    {
        sheep = GetComponent<SheepStateManager>();
    }

    private void Start()
    {
        if (sheep == null) sheep = GetComponent<SheepStateManager>();
        if (markAsStragglerOnStart && sheep != null)
        {
            sheep.MarkAsStraggler();
            Debug.Log($"[SheepRejoinTester] Marked as straggler → press {rejoinHotkey} to rejoin.", this);
        }
    }

    private void Update()
    {
        if (sheep == null) return;

        if (Input.GetKeyDown(rejoinHotkey))
        {
            RejoinNow();
        }
    }

    /// <summary>
    /// Inspector button (via context menu) to trigger join without hotkey.
    /// </summary>
    [ContextMenu("Rejoin Now")]
    public void RejoinNow()
    {
        sheep.SummonToHerd(graceAfterJoin, clearThreatsOnJoin);
        Debug.Log($"[SheepRejoinTester] SummonToHerd() called (grace={graceAfterJoin}, clearThreats={clearThreatsOnJoin}).", this);
    }

    // Optional: little on-screen hint
    private void OnDrawGizmosSelected()
    {
        if (sheep == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
