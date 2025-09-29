using UnityEngine;
using UnityEngine.AI;

namespace Gameplay.Dog
{
    /// <summary>
    /// Main script for the dog.
    /// </summary>
    [RequireComponent(typeof(DogMovementController), typeof(NavMeshAgent), typeof(DogStateManager))]
    public class Dog : MonoBehaviour
    {
        [Tooltip("Transform of player object to follow.")]
        [SerializeField] private Transform _playerTransform;
        [Tooltip("Manager of step sounds.")]
        [SerializeField] private StepsSoundManager _stepsSoundManager;
        [Tooltip("Animator of the dog.")]
        [SerializeField] private Animator _animator;
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

            _stepsSoundManager.Initialize();

            DogAnimator animator = new DogAnimator(_animator, _config);

            DogStateManager stateManager = GetComponent<DogStateManager>();
            stateManager.Initialize(_movementController, animator, _playerTransform, _config);
        }

        // for test, needs to be moved to bootstrap
        void Start()
        {
            Initialize();
        }
    }
}