using Core.Shared.StateMachine;
using UnityEngine;

namespace Gameplay.Player
{
    /// <summary>
    /// Basic player state class.
    /// </summary>
    public abstract class PlayerState : IState
    {
        protected readonly PlayerStateManager _manager;
        protected readonly PlayerMovement _playerMovement;


        /// <param name="manager">Manager which uses this state.</param>
        public PlayerState(PlayerStateManager manager)
        {
            _manager = manager;
            _playerMovement = _manager.MovementController as PlayerMovement;
        }


        public abstract void OnStart();
        public abstract void OnStop();
        public abstract void OnUpdate();
    }
}