using Core.Shared.Utilities;
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
        [SerializeField, Tooltip("Transform of player object to follow."), Required]
        private Transform playerTransform;
        [SerializeField, Tooltip("Herd zone reference."), Required]
        private HerdZone herdZone;
        [SerializeField, Tooltip("Manager of step sounds."), Required]
        private StepsSoundManager stepsSoundManager;
        [SerializeField, Tooltip("Animator of the dog."), Required]
        private Animator animator;
        [SerializeField, Tooltip("Config for the dog."), Required]
        private DogConfig config;


        private DogMovementController movementController;
        private DogAnimator dogAnimator;
        private DogBark bark;


        /// <summary>
        /// Initialization method.
        /// </summary>
        public void Initialize()
        {
            movementController = GetComponent<DogMovementController>();
            NavMeshAgent _agent = GetComponent<NavMeshAgent>();
            movementController.Initialize(_agent, config);

            stepsSoundManager.Initialize();

            dogAnimator = new DogAnimator(animator, config);

            bark = GetComponent<DogBark>();
            bark.Initialize(config);

            DogStateManager _stateManager = GetComponent<DogStateManager>();
            _stateManager.Initialize(movementController, dogAnimator, herdZone, playerTransform, config);

            config.OnValueChanged += UpdateValues;
            UpdateValues(config);
        }


        private void UpdateValues(DogConfig config)
        {
            movementController.UpdateValues(config);
            dogAnimator.UpdateAnimationValues(config);

            bark.Initialize(this.config);
        }


        private void OnDestroy()
        {
            config.OnValueChanged -= UpdateValues;
        }
    }
}