using UnityEngine;

namespace Birds.AI
{
    /// <summary>
    /// State where birds fly away from the player.
    /// </summary>
    public class BirdFleeState : BirdState
    {
        private Vector3 flightDirection;

        public BirdFleeState(BirdFlockController c, BirdConfig conf) : base(c, conf) { }

        public override void Enter()
        {

            controller.Animator.SetFlying(true);
            // 1. Get positions
            Vector3 playerPos = controller.PlayerTransform.position;
            Vector3 flockPos = controller.VisualRoot.position; // Use VisualRoot position
            
            // 2. Calculate direction: (Bird - Player) gives vector pointing AWAY from player
            Vector3 directionAway = (flockPos - playerPos).normalized;
            
            // 3. Add an "Up" factor so they fly into the sky, not just along the ground
            // We mix the "Away" vector with Vector3.up
            flightDirection = (directionAway + Vector3.up).normalized;
        }

        public override void Update()
        {
            // --- MOVEMENT ---
            // Move the visuals in the calculated direction
            float step = config.FlightSpeed * Time.deltaTime;
            controller.VisualRoot.Translate(flightDirection * step, Space.World);

            // --- ROTATION ---
            // Make the birds face the direction they are flying
            // We use Slerp for a smooth turn, though usually they turn instantly when scared
            if (flightDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(flightDirection);
                controller.VisualRoot.rotation = Quaternion.Slerp(controller.VisualRoot.rotation, targetRotation, Time.deltaTime * 10f);
            }

            // --- EXIT CONDITION ---
            // Check if they have flown high enough to "vanish"
            // We check local Y position relative to where they started, or absolute Y
            if (controller.VisualRoot.localPosition.y > config.FlightHeight) 
            {
                controller.SetState(new BirdReturnState(controller, config));
            }
        }

        public override void Exit()
        {
            // Now that they are high up and out of sight, we turn them off
            controller.SetVisualsActive(false);
        }
    }
}