using Core;

namespace AI.Drekavac.States
{
    public class DrekavacState : GenericEnemyState
    {
        protected readonly DrekavacAnimatorController _animator;
        protected readonly DrekavacStateManager _manager;
        protected readonly DrekavacStats _stats;

        protected DrekavacState(DrekavacStateManager manager, EnemyMovementController movement, DrekavacStats stats, DrekavacAnimatorController animator, AudioController audio)
            : base(movement, audio)
        {
            _animator = animator;
            _manager = manager;
            _stats = stats;
        }
    }
}
