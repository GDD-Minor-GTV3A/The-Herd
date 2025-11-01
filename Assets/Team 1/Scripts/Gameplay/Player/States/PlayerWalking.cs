using UnityEngine;

namespace Gameplay.Player
{
    /// <summary>
    /// Player is walking.
    /// </summary>
    public class PlayerWalking : PlayerState
    {
        private readonly PlayerAnimator _animator;

        public PlayerWalking(PlayerStateManager stateMachine) : base(stateMachine)
        {
            _animator = _manager.AnimatorController as PlayerAnimator;
        }

        public override void OnStart()
        {
            _animator.SetWalking(true);
            _animator.SetWalkSpeed(false);
        }

        public override void OnStop()
        {
        }

        public override void OnUpdate()
        {
            // If there's no input, go idle.
            if (_manager.Input.Move.magnitude == 0)
            {
                _manager.SetState<PlayerIdle>();
                return;
            }

            _playerMovement.ApplyGravity();

            bool isSprinting = _manager.Input.Run;
            Vector2 playerInput = _manager.Input.Move;

            Vector3 movementTarget = _playerMovement.CalculateMovementTargetFromInput(playerInput, isSprinting);
            _playerMovement.MoveTo(movementTarget);

            // Rotate based on current rotation mode (movement or mouse)
            _manager.Rotation.Rotate(playerInput, _manager.Input.Look.Value);


            _animator.SetWalkSpeed(isSprinting);
        }
    }
}
