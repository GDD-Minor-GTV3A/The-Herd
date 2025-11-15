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
            _animator.SetAnimationRotation(true);
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

            if (_manager.Input.Run)
            {
                _manager.SetState<PlayerRunning>();
                return;
            }
            _playerMovement.ApplyGravity();

            Vector3 movementTarget = _playerMovement.CalculateMovementTargetFromInput(playerInput, false);
            _playerMovement.MoveTo(movementTarget);

            // Rotate based on current rotation mode (movement or mouse)
            //_manager.Rotation.Rotate(playerInput, _manager.Input.Look.Value);
            _animator.RotateCharacterBody(_manager.Input.Look.Value);

            _animator.Walking(_manager.Input.Move);
        }
    }
}
