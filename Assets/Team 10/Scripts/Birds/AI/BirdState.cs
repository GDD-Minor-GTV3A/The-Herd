using UnityEngine;

namespace Birds.AI
{
    public abstract class BirdState
    {
        protected BirdFlockController controller;
        protected BirdConfig config;

        public BirdState(BirdFlockController controller, BirdConfig config)
        {
            this.controller = controller;
            this.config = config;
        }

        public abstract void Enter();
        public abstract void Update();
        public abstract void Exit();
    }
}