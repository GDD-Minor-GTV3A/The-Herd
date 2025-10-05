using Core.Shared;

using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerAnimator : AnimatorController
    {
        private const string WalkingParam = "Walk";
        private const string WalkSpeedParam = "WalkSpeed";


        public PlayerAnimator(Animator animator) : base(animator)
        {
        }


        /// <summary>
        /// Sets walking animation state.
        /// </summary>
        /// <param name="walking">Is walking.</param>
        public void SetWalking(bool walking)
        {
            _animator.SetBool(WalkingParam, walking);
        }


        /// <summary>
        /// Changes walk speed animation multiplier.
        /// </summary>
        /// <param name="sprint"> Is player sprinting.</param>
        public void SetWalkSpeed(bool sprint)
        {
            if (sprint)
                _animator.SetFloat(WalkSpeedParam, 2f);
            else
                _animator.SetFloat(WalkSpeedParam, 1f);
        }

    }
}