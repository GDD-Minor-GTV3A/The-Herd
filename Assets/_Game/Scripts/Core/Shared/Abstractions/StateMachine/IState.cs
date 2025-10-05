namespace Core.Shared.StateMachine
{
    /// <summary>
    /// Interface for states which are used in state managers.
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// This method is called on start of the state.
        /// </summary>
        public abstract void OnStart();

        /// <summary>
        /// This method is called on each frame of the state.
        /// </summary>
        public abstract void OnUpdate();

        /// <summary>
        /// This method is called on stop of the state.
        /// </summary>
        public abstract void OnStop();
    }
}