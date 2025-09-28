using UnityEngine;

namespace Gameplay.Player 
{
    /// <summary>
    /// Player is staying.
    /// </summary>
    public class PlayerIdle : PlayerState
    {
        public PlayerIdle(PlayerStateManager stateMachine) : base(stateMachine)
        {
        }


        public override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        public override void OnUpdate()
        {
            if (_manager.Input.Move.magnitude > 0)
                _manager.SetState<PlayerWalking>();

            _playerMovement.ApplyGravity();
            //_manager.Rotation.Rotate();
        }
    }
}