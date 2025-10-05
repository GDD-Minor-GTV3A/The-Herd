using UnityEngine;

namespace Core.AI.Sheep
{
    [RequireComponent(typeof(Animator))]
    public sealed class SheepAnimationDriver : MonoBehaviour
    {
        static readonly int HashState = Animator.StringToHash("State");
        static readonly int HashSpeed = Animator.StringToHash("Speed");
        //static readonly int HashIdleVariant = Animator.StringToHash("IdleVariant");
        //static readonly int HashGrazeNibble = Animator.StringToHash("GrazeNibble");

        [SerializeField] private Animator _animator;

        private void Reset()
        {
            _animator = GetComponent<Animator>();
        }

        public void SetState(int s)
        {
            if (_animator)
            {
                _animator.SetInteger(HashState, s);
            }
        }

        public void SetSpeed(float v)
        {
            if (_animator)
            {
                _animator.SetFloat(HashSpeed, v);
            }
        }

        public void ApplyOverrideController(AnimatorOverrideController overrideController)
        {
            if(_animator && overrideController)
            {
                _animator.runtimeAnimatorController = overrideController;
            }
        }
    }

}
