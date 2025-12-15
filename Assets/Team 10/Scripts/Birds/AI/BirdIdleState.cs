using UnityEngine;

namespace Birds.AI
{
    public class BirdIdleState : BirdState
    {
        public BirdIdleState(BirdFlockController c, BirdConfig conf) : base(c, conf) { }

        public override void Enter() 
        {
            controller.Animator.SetFlying(false);
        }

        public override void Update()
        {
            // Logic is handled by TriggerEnter in Controller
        }

        public override void Exit() { }
    }
}