namespace Core.Shared.StateMachine
{
    /// <summary>
    /// Use this class as a base for state manager for non-static characters.
    /// </summary>
    /// <typeparam name="T">State class for this manager.</typeparam>
    public abstract class CharacterStateManager<T> : StateManager<T> where T : IState
    {
        private MovementController _movementController;
        private AnimatorController _animatorController;


        /// <summary>
        /// Movement controller of character.
        /// </summary>
        public MovementController MovementController => _movementController;
        /// <summary>
        /// Animator controller of character.
        /// </summary>
        public AnimatorController AnimatorController => _animatorController;
    }
}