using Core;
using Phantom;

using Team_7.Scripts.AI.Phantom;

namespace AI.Phantom.States
{
    public class PhantomState : GenericEnemyState
    {
        protected readonly PhantomAnimatorController _animator;
        protected readonly PhantomStateManager _manager;
        protected readonly PhantomStats _stats;

        protected PhantomState(PhantomStateManager manager, EnemyMovementController movement, PhantomStats stats, PhantomAnimatorController animator, AudioController audio)
            : base(movement, audio)
        {
            _animator = animator;
            _manager = manager;
            _stats = stats;
        }
    }
}