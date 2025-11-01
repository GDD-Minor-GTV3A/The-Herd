using UnityEngine;
using Core.Shared;

namespace AI.Phantom
{
    public class PhantomAnimatorController : AnimatorController
    {
        private static readonly int Shooting = Animator.StringToHash("Charging");
        private static readonly int Throwing = Animator.StringToHash("Throwing");

        public PhantomAnimatorController(Animator animator) : base(animator)
        {
        
        }

        public void SetCharging(bool enabled)
        {
            _animator.SetBool(Shooting, enabled);
        }

        public void SetThrowing(bool enabled)
        {
            _animator.SetBool(Throwing, enabled);
        }

        public bool IsThrowingFinished()
        {
            var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            return (stateInfo.IsName("rig_Throw") && stateInfo.normalizedTime >= 1f);
        }
    }
}
