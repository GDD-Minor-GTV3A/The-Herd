using _Game.Team_7.Scripts.Drekavac;
using Core.Shared.StateMachine;

namespace _Game.Team_7.Scripts
{
    /// <summary>
    ///     Base enemy state class
    /// </summary>
    public abstract class GenericEnemyState : IState
    {
        //TODO make it accept more generic controllers/managers so it can be reused across enemies.
        protected readonly DrekavacAnimatorController _animator;
        protected readonly DrekavacAudioController _audio;
        protected readonly DrekavacStateManager _manager;
        protected readonly EnemyMovementController _movement;
        protected readonly DrekavacStats _stats;

        protected GenericEnemyState(DrekavacStateManager manager, EnemyMovementController movement, DrekavacStats stats, DrekavacAnimatorController animator, DrekavacAudioController audio)
        {
            _manager = manager;
            _movement = movement;
            _animator = animator;
            _audio = audio;
            _stats = stats;
        }
    
        public virtual void OnStart()
        {
        }

        public virtual void OnUpdate()
        {
        }

        public virtual void OnStop()
        {
        }
    }
}
