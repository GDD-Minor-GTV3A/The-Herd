using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerRunning: PlayerState
    {
        public PlayerRunning(PlayerStateManager stateMachine) : base(stateMachine)
        {
        }


        public override void OnStart()
        {
            animator.SetAnimationRotation(false);
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

            if (!manager.Input.Run)
            {
                manager.SetState<PlayerWalking>();
                return;
            }

            playerMovement.ApplyGravity();


            Vector3 movementTarget = playerMovement.CalculateMovementTargetFromInput(playerInput, true);
            playerMovement.MoveTo(movementTarget);

            playerMovement.Rotate(playerInput);

            animator.Walking(manager.Input.Move, true);
        }
    }
}