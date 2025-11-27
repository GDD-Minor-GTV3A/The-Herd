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
            animator.SetAnimationRotation(true);
        }

        public override void OnStop()
        {
        }

        public override void OnUpdate()
        {
            if (manager.Input.Move.magnitude > 0)
                manager.SetState<PlayerWalking>();

            playerMovement.ApplyGravity();

            //Vector2 _playerInput = manager.Input.Move;
            //_manager.Rotation.Rotate(playerInput, _manager.Input.Look.Value);

            animator.RotateCharacterBody(manager.Input.Look.Value);

            animator.Walking(Vector2.zero);
        }
    }
}