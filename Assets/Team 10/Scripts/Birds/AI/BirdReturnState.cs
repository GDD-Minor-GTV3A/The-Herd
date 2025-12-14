using UnityEngine;

namespace Birds.AI
{
    /// <summary>
    /// Handles the delay while birds are "gone", then picks a respawn point.
    /// </summary>
    public class BirdReturnState : BirdState
    {
        // private float timer;

        public BirdReturnState(BirdFlockController c, BirdConfig conf) : base(c, conf) { }

        public override void Enter()
        {
            // timer = config.ReturnDelay;
        }

        public override void Update()
        {
            // timer -= Time.deltaTime;
            
            // if (timer <= 0)
            // {
            //     SpawnAtRandomAngle();
            // }
        }

        // private void SpawnAtRandomAngle()
        // {
        //     // 1. Get a random direction on a sphere
        //     Vector3 randomDirection = Random.onUnitSphere;

        //     // 2. Ensure they always spawn ABOVE the ground (y positive)
        //     if (randomDirection.y < 0) randomDirection.y = -randomDirection.y;

        //     // 3. Calculate the spawn position relative to the Flock Center
        //     Vector3 spawnOffset = randomDirection * config.SpawnRadius;

        //     // 4. Teleport the visual root to this new "outer" position
        //     controller.VisualRoot.localPosition = spawnOffset;

        //     // 5. Make them visible again
        //     controller.SetVisualsActive(true);

        //     // 6. Switch to the Landing state to fly them home
        //     controller.SetState(new BirdLandingState(controller, config));
        // }

        public override void Exit() { }
    }
}