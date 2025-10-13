using Core.Shared.StateMachine;

namespace Team_7.Scripts.AI
{
    /// <summary>
    ///     Base enemy state class
    /// </summary>
    public abstract class GenericEnemyState : IState
    {
        //TODO make it accept more generic controllers/managers so it can be reused across enemies.
        protected readonly AudioController _audio;
        protected readonly EnemyMovementController _movement;

        protected GenericEnemyState(EnemyMovementController movement, AudioController audio)
        {
            _movement = movement;
            _audio = audio;
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