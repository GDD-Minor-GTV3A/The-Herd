using Core.Shared.StateMachine;

namespace Gameplay.Dog 
{
    /// <summary>
    /// Abstract class for all state for Dog.
    /// </summary>
    public abstract class DogState : IState
    {
        protected readonly DogStateManager manager;
        protected readonly DogMovementController movement;
        protected readonly DogAnimator animator;


        /// <param name="manager">Manager which will use this state.</param>
        public DogState(DogStateManager manager)
        {
            this.manager = manager;
            movement = manager.MovementController as DogMovementController;
            animator = manager.AnimatorController as DogAnimator;
        }


        public abstract void OnStart();

        public abstract void OnStop();

        public abstract void OnUpdate();
    }
}