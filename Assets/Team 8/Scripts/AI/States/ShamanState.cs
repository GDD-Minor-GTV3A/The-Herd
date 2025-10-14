using Core.Shared.StateMachine;

namespace AI.Shaman
{
    /// <summary>
    /// shaman state class.
    /// </summary>
    public abstract class ShamanState : IState
    {
        protected readonly ShamanStateManager _manager;


        /// <param name="manager">manager which uses this state.</param>
        public ShamanState(ShamanStateManager manager)
        {
            _manager = manager;
        }


        public abstract void OnStart();
        public abstract void OnStop();
        public abstract void OnUpdate();
    }
}

