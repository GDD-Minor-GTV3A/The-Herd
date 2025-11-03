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
        [Tooltip("Heard zone reference.")]
        [SerializeField] private HeardZone _heardZone;
        [Tooltip("Manager of step sounds.")]
        [SerializeField] private StepsSoundManager _stepsSoundManager;
        [Tooltip("Animator of the dog.")]
        [SerializeField] private Animator _animator;
        [Tooltip("Config for the dog.")]
        [SerializeField] private DogConfig _config;


        private DogMovementController _movementController;
        private DogAnimator _dogAnimator;
        private DogBark bark;


        /// <summary>
        /// Initialization method.
        /// </summary>
        public void Initialize()
        {
            _movementController = GetComponent<DogMovementController>();
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            _movementController.Initialize(agent, _config);

            _stepsSoundManager.Initialize();

            _dogAnimator = new DogAnimator(_animator, _config);

            bark = GetComponent<DogBark>();
            bark.Initialize(_config);

            DogStateManager stateManager = GetComponent<DogStateManager>();
            stateManager.Initialize(_movementController, _dogAnimator, _heardZone, _playerTransform, _config);

            _config.OnValueChanged += UpdateValues;
            UpdateValues(_config);
        }

        private void UpdateValues(DogConfig config)
        {
            _movementController.UpdateValues(config);
            _dogAnimator.UpdateAnimationValues(config);

            bark.Initialize(_config);
        }

        // for test, needs to be moved to bootstrap
        void Start()
        {
            Initialize();
        }


        private void OnDestroy()
        {
            _config.OnValueChanged -= UpdateValues;
        }
    }
}