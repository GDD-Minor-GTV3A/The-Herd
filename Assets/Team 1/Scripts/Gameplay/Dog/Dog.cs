using UnityEngine;
using UnityEngine.AI;

namespace Gameplay.Dog
{
    /// <summary>
    /// Main script for the dog.
    /// </summary>
    public class Dog : MonoBehaviour
    {
        [Tooltip("Transform of player object to follow.")]
        [SerializeField] private Transform _playerTransform;
        [Tooltip("Config for the dog.")]
        [SerializeField] private DogConfig _config;


        private DogMovementController _movementController;


        /// <summary>
        /// Initialization method.
        /// </summary>
        public void Initialize()
        {
            _movementController = GetComponent<DogMovementController>();
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            _movementController.Initialize(agent, _config);


            DogStateManager stateManager = GetComponent<DogStateManager>();
            stateManager.Initialize(_movementController, _playerTransform, _config);
        }

        // for test, needs to be moved to bootstrap
        void Start()
        {
            Initialize();
        }
    }

}