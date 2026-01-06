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

            playerMovement.Rotate(playerInput);

            animator.Walking(true);
            animator.RotateHead(manager.Input.Look.Value);
        }
    }
}