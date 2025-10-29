using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.Player
{
    /// <summary>
    /// Player is staying.
    /// </summary>
    public class PlayerIdle : PlayerState
    {
        private readonly PlayerAnimator _animator;


        public PlayerIdle(PlayerStateManager stateMachine) : base(stateMachine)
        {
            _animator = _manager.AnimatorController as PlayerAnimator;
        }


        public override void OnStart()
        {
            _animator.SetWalking(false);
            _animator.SetAnimationRotation(true);
        }

        public override void OnStop()
        {
        }

        public override void OnUpdate()
        {
            if (_manager.Input.Move.magnitude > 0)
                _manager.SetState<PlayerWalking>();

            _playerMovement.ApplyGravity();
            Vector2 playerInput = _manager.Input.Move;
            //_manager.Rotation.Rotate(playerInput, _manager.Input.Look.Value);

            _animator.RotateCharacterBody(_manager.Input.Look.Value);
        }

    }
}