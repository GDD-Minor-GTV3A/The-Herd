using Core.Shared.StateMachine;

namespace Gameplay.Dog
{
    /// <summary>
    /// Abstract class for all state for Dog.
    /// </summary>
    public abstract class DogState : IState
    {
        protected DogStateManager _manager;


        /// <param name="manager">Menager which will use this state.</param>
        public DogState(DogStateManager manager)
        {
            _manager = manager;
        }


        public abstract void OnStart();

        public abstract void OnStop();

        public abstract void OnUpdate();
    }
}