namespace Core.Shared.StateMachine
{
    /// <summary>
    /// Use this class as a base for state manager for non-static characters.
    /// </summary>
    /// <typeparam name="T">State class for this manager.</typeparam>
    public abstract class CharacterStateManager<T> : StateManager<T>, IPausable where T : IState
    {
        private bool isPaused;


        protected MovementController _movementController;
        protected AnimatorController _animatorController;


        /// <summary>
        /// Movement controller of character.
        /// </summary>
        public MovementController MovementController => _movementController;
        /// <summary>
        /// Animator controller of character.
        /// </summary>
        public AnimatorController AnimatorController => _animatorController;


        protected override void InitializeStatesMap()
        {
            Resume();
        }



        protected override void Update()
        {
            if (isPaused) return;

            base.Update();
        }

        public void Pause()
        {
            isPaused = true;
        }

        public void Resume()
        {
            isPaused = false;
        }
    }
}