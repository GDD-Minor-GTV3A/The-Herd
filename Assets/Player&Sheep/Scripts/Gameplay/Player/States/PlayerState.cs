using Core.Shared.StateMachine;

namespace Gameplay.Player
{
    /// <summary>
    /// Basic player state class.
    /// </summary>
    public abstract class PlayerState : IState
    {
        protected readonly PlayerStateManager manager;
        protected readonly PlayerMovement playerMovement;
        protected readonly PlayerAnimator animator;


        /// <param name="manager">Manager which uses this state.</param>
        public PlayerState(PlayerStateManager manager)
        {
            this.manager = manager;
            playerMovement = this.manager.MovementController as PlayerMovement;
            animator = this.manager.AnimatorController as PlayerAnimator;
        }


        public abstract void OnStart();
        public abstract void OnStop();
        public abstract void OnUpdate();
    }
}