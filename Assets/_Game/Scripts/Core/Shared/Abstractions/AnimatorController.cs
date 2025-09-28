using UnityEngine;

namespace Core.Shared
{
    /// <summary>
    /// Use this class as a base for character's animator controller.
    /// </summary>
    public abstract class AnimatorController
    {
        private Animator _animator;


        public AnimatorController(Animator animator)
        {
            _animator = animator;
        }
    }
}