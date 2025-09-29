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
        private NavMeshAgent _agent;
        
        private float _minSpeed;
        private float _maxSpeed;
        private float _baseSpeed;

        private float _slowDistance;
        private float _maxDistance;


        /// <summary>
        /// True - dog is moving or pending the path, false - dog is in idle state.
        /// </summary>
        public bool IsMoving => (_agent.hasPath || _agent.pathPending);


        /// <summary>
        /// Initialization method.
        /// </summary>
        /// <param name="agent">NavMeshAgent of the dog.</param>
        /// <param name="config">Config of the dog.</param>
        public void Initialize(NavMeshAgent agent, DogConfig config)
        {
            _agent = agent;

            UpdateValues(config);
        }


        public override void MoveTo(Vector3 target)
        {
            _agent.speed = _baseSpeed;

            _agent.destination = target;

        }


        /// <summary>
        /// Changes speed of the dog depending on distance to hte player.
        /// </summary>
        public float CalculateSpeedToPlayer()
        {
            float t = Mathf.InverseLerp(_slowDistance, _maxDistance, _agent.remainingDistance);
            _agent.speed = Mathf.Lerp(_minSpeed, _maxSpeed, t);

            return _agent.speed;
        }


        /// <summary>
        /// Update values according to config.
        /// </summary>
        /// <param name="config">Config of the dog.</param>
        public void UpdateValues(DogConfig config)
        {
            _minSpeed = config.MinSpeed;
            _maxSpeed = config.MaxSpeed;
            _baseSpeed = config.BaseSpeed;

            _slowDistance = config.SlowDistance;
            _maxDistance = config.MaxDistance;

            _agent.speed = _baseSpeed;

            _agent.angularSpeed = config.RotationSpeed;
        }
    }
}