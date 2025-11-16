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
            animator.SetAnimationRotation(true);
        }

        public override void OnStop()
        {
        }

        public override void OnUpdate()
        {
            Vector2 playerInput = manager.Input.Move;

            // If there's no input, go idle.
            if (playerInput.magnitude == 0)
            {
                manager.SetState<PlayerIdle>();
                return;
            }

            if (manager.Input.Run)
            {
                manager.SetState<PlayerRunning>();
                return;
            }
            playerMovement.ApplyGravity();

            Vector3 movementTarget = playerMovement.CalculateMovementTargetFromInput(playerInput, false);
            playerMovement.MoveTo(movementTarget);

            // Rotate based on current rotation mode (movement or mouse)z
            //_manager.Rotation.Rotate(playerInput, _manager.Input.Look.Value);
            animator.RotateCharacterBody(manager.Input.Look.Value);

            animator.Walking(manager.Input.Move);
        }
    }
}