using Core.Shared;
using UnityEngine;
using UnityEngine.AI;

namespace Gameplay.Dog 
{
    /// <summary>
    /// Movement controller for the dog.
    /// </summary>
    public class DogMovementController : MovementController
    {
        private NavMeshAgent agent;
        
        private float minSpeed;
        private float maxSpeed;
        private float baseSpeed;

        private float slowDistance;
        private float maxDistance;


        /// <summary>
        /// True - dog is moving or pending the path, false - dog is in idle state.
        /// </summary>
        public bool IsMoving => (agent.hasPath || agent.pathPending || agent.remainingDistance > agent.stoppingDistance || agent.velocity.sqrMagnitude > 0.1f);


        /// <summary>
        /// Initialization method.
        /// </summary>
        /// <param name="agent">NavMeshAgent of the dog.</param>
        /// <param name="config">Config of the dog.</param>
        public void Initialize(NavMeshAgent agent, DogConfig config)
        {
            this.agent = agent;

            UpdateValues(config);
        }


        public override void MoveTo(Vector3 target)
        {
            agent.speed = baseSpeed;

            agent.destination = target;

        }


        /// <summary>
        /// Changes speed of the dog depending on distance to hte player.
        /// </summary>
        public float CalculateSpeedToPlayer()
        {
            float _t = Mathf.InverseLerp(slowDistance, maxDistance, agent.remainingDistance);
            agent.speed = Mathf.Lerp(minSpeed, maxSpeed, _t);

            return agent.speed;
        }


        /// <summary>
        /// Update values according to config.
        /// </summary>
        /// <param name="config">Config of the dog.</param>
        public void UpdateValues(DogConfig config)
        {
            minSpeed = config.MinSpeed;
            maxSpeed = config.MaxSpeed;
            baseSpeed = config.BaseSpeed;

            slowDistance = config.SlowDistance;
            maxDistance = config.MaxDistance;

            agent.speed = baseSpeed;

            agent.angularSpeed = config.RotationSpeed;
        }
    }
}