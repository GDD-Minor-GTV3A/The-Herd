// AmalgationStateEcho.cs
using UnityEngine;

public class AmalgamationAnimBridge : MonoBehaviour
{
    public Animator animator;                    // assign the Armature’s Animator

    [Header("State names on layer 0")]
    public string idleState   = "Idle";
    public string movingState = "Moving";

    [Header("Optional Trigger Names")]
    [Tooltip("Animator trigger name for slam (leave blank to do nothing).")]
    public string slamTrigger = "Slam";

    static readonly int SpeedHash = Animator.StringToHash("Speed");

    void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (!animator) animator = GetComponentInChildren<Animator>(true);
    }

    public void PlayRunImmediate()
    {
        if (!animator) return;

        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        animator.Rebind();
        animator.Update(0f);

        animator.SetFloat(SpeedHash, 1f);
        animator.CrossFadeInFixedTime(movingState, 0.05f, 0, 0f);
    }

    public void PlayIdleImmediate()
    {
        if (!animator) return;

        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        animator.Rebind();
        animator.Update(0f);

        animator.SetFloat(SpeedHash, 0f);
        animator.CrossFadeInFixedTime(idleState, 0.05f, 0, 0f);
    }

    // ✅ Added so AmalgamationSlamAttack.cs can call it safely
    public void TriggerSlam()
    {
        if (!animator) return;
        if (string.IsNullOrWhiteSpace(slamTrigger)) return;

        animator.SetTrigger(slamTrigger);
    }
}
