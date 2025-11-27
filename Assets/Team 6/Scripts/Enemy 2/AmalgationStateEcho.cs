using UnityEngine;

public class AmalgamationAnimBridge : MonoBehaviour
{
    public Animator animator;                    // assign the Armature’s Animator
    [Header("State names on layer 0")]
    public string idleState   = "Idle";
    public string movingState = "Moving";

    static readonly int SpeedHash = Animator.StringToHash("Speed");

    void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (!animator) animator = GetComponentInChildren<Animator>(true);
    }

    public void PlayRunImmediate()
    {
        if (!animator) return;

        // make sure it can animate even if offscreen
        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

        // ensure a clean binding (helps after attack states)
        animator.Rebind();
        animator.Update(0f);

        // force the param and crossfade to Moving right now
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
}
