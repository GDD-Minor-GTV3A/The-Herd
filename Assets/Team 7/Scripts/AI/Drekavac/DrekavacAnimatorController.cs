using Core.Shared;

using UnityEngine;

namespace Team_7.Scripts.AI.Drekavac
{
    /// <summary>
    ///     Handles the animation states of an enemy.
    /// </summary>
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
}