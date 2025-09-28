using UnityEngine;

namespace Gameplay.Player
{
    /// <summary>
    /// Player is walking.
    /// </summary>
    public class PlayerWalking : PlayerState
    {
        public PlayerWalking(PlayerStateManager stateMachine) : base(stateMachine)
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
            if (_manager.Input.Move.magnitude == 0)
                _manager.SetState<PlayerIdle>();

            _playerMovement.ApplyGravity();

            Vector3 movementTarget = _playerMovement.CalculateMovementTargetFromInput(_manager.Input.Move, _manager.Input.Run);

            _playerMovement.MoveTo(movementTarget);
            _manager.Rotation.RotateToCamera();
        }
    }
}