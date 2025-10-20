using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerRunning: PlayerState
    {
        private readonly PlayerAnimator _animator;
        private readonly PlayerMovement _movement;

        public PlayerRunning(PlayerStateManager stateMachine) : base(stateMachine)
        {
            _animator = _manager.AnimatorController as PlayerAnimator;
            _movement = _manager.MovementController as PlayerMovement;
        }

        public override void OnStart()
        {
            _animator.SetWalking(true);
            _animator.SetWalkSpeed(true);

            _animator.SetAnimationRotation(false);
        }

        public override void OnStop()
        {
        }

        public override void OnUpdate()
        {
            Vector2 playerInput = _manager.Input.Move;

            // If there's no input, go idle.
            if (playerInput.magnitude == 0)
            {
                _manager.SetState<PlayerIdle>();
                return;
            }

            if (!_manager.Input.Run)
            {
                _manager.SetState<PlayerWalking>();
                return;
            }

            _playerMovement.ApplyGravity();


            Vector3 movementTarget = _playerMovement.CalculateMovementTargetFromInput(playerInput, true);
            _playerMovement.MoveTo(movementTarget);

            // Rotate based on current rotation mode (movement or mouse)
            _movement.Rotate(playerInput);
        }
    }
}
