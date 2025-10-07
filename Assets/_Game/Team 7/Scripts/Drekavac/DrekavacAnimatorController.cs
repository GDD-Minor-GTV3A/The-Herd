using Core.Shared;

using UnityEngine;

public class DrekavacAnimatorController : AnimatorController
{
    private static readonly int Stalking = Animator.StringToHash("Stalking");
    private static readonly int Dragging = Animator.StringToHash("Retreating");

    public DrekavacAnimatorController(Animator animator) : base(animator)
    {
        
    }

    public void SetStalking(bool enabled)
    {
        _animator.SetBool(Stalking, enabled);
    }

    public void SetDragging(bool enabled)
    {
        _animator.SetBool(Dragging, enabled);
    }
}
