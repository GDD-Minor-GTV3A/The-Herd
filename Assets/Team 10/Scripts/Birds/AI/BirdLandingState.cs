using UnityEngine;

namespace Birds.AI
{
    /// <summary>
    /// State where birds fly from a random outer point back to their perch.
    /// </summary>
    public class BirdLandingState : BirdState
    {
        public BirdLandingState(BirdFlockController c, BirdConfig conf) : base(c, conf) { }

        public override void Enter()
        {
            controller.Animator.SetFlying(true);
        }

        public override void Update()
        {
            // 1. Calculate direction towards the perch (Local Zero)
            // Since the VisualRoot is moving, "Home" is simply Vector3.zero in local space
            Vector3 currentPos = controller.VisualRoot.localPosition;
            Vector3 directionHome = (Vector3.zero - currentPos).normalized;

            // 2. Move
            float step = config.FlightSpeed * Time.deltaTime;
            controller.VisualRoot.Translate(directionHome * step, Space.World);

            // 3. Rotate to face home
            if (directionHome != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionHome);
                controller.VisualRoot.rotation = Quaternion.Slerp(controller.VisualRoot.rotation, targetRotation, Time.deltaTime * 10f);
            }

            // 4. Check if we have arrived
            // We check the distance to zero
            if (Vector3.Distance(controller.VisualRoot.localPosition, Vector3.zero) < 0.5f)
            {
                // Snap to exact position and finish
                controller.VisualRoot.localPosition = Vector3.zero;
                controller.VisualRoot.localRotation = Quaternion.identity;
                controller.SetState(new BirdIdleState(controller, config));
            }
        }

        public override void Exit() { }
    }
}