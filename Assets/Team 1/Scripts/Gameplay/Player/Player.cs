using Gameplay.HealthSystem;
using Gameplay.ToolsSystem;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.Player
{
    /// <summary>
    /// Base player script.
    /// </summary>
    [RequireComponent(typeof(PlayerMovement), typeof(PlayerStateManager))]
    [RequireComponent(typeof(PlayerInput), typeof(ToolSlotsController), typeof(CharacterController))]
    public class Player : MonoBehaviour
    {
        [Header("Animations")]
        [Tooltip("Animator of the player.")]
        [SerializeField] private Animator _animator;
        [SerializeField] private PlayerAnimationConstraints animationConstrains;

        [Space]
        [Tooltip("Manager of step sounds.")]
        [SerializeField] private StepsSoundManager _stepsSoundManager;
        [Tooltip("Reference to input actions map.")]
        [SerializeField] private InputActionAsset _inputActions;
        [Tooltip("Reference to player config.")]
        [SerializeField] private PlayerConfig _config;


        private PlayerMovement _movementController;
        private Health _health;



        // for test, needs to be moved to bootstrap
        private void Start()
        {
            Initialize();
        }

        /// <summary>
        /// Initialization method.
        /// </summary>
        public void Initialize()
        {
            Vector3 forward = Camera.main.transform.forward;

            forward.y = 0f;

            forward.Normalize();

            transform.forward = forward;

            _movementController = GetComponent<PlayerMovement>();
            PlayerStateManager stateManager = GetComponent<PlayerStateManager>();
            PlayerInput playerInput = GetComponent<PlayerInput>();
            ToolSlotsController slotsController = GetComponent<ToolSlotsController>();

            playerInput.Initialize(_inputActions);

            slotsController.Initialize(playerInput, 2);

            CharacterController characterController = GetComponent<CharacterController>();
            _movementController.Initialize(characterController, _config);


            _stepsSoundManager.Initialize();
            PlayerAnimator animator = new PlayerAnimator(_animator,transform, animationConstrains);
            stateManager.Initialize(playerInput, _movementController, animator);

            // Init health
            _health = new Health(
                _config.MaxHealth,
                _config.CurrentHealth,
                _config.CanTakeDamage,
                _config.CanBeHealed,
                _config.CanDie
            );
            _health.OnHealthChanged += HandleHealthChanged;
            _health.OnDeath += HandleDeath;

            _config.OnValueChanged += UpdateConfigValues;

        }
        private void HandleHealthChanged(float current, float max)
        {
            Debug.Log($"Player health updated: {current}/{max}");
        }

        private void HandleDeath()
        {
            Debug.Log("Player died!");
            _movementController.enabled = false;
            _rotationController.enabled = false;
            _animator.SetTrigger("Die");
        }

        // Gameplay entry points
        public void TakeDamage(float amount) => _health.TakeDamage(amount);
        public void Heal(float amount) => _health.Heal(amount);


        /// <summary>
        /// Update values according to config.
        /// </summary>
        /// <param name="config">Config of the player.</param>
        private void UpdateConfigValues(PlayerConfig config)
        {
            _movementController.UpdateValues(config);
        }


        private void OnDestroy()
        {
            _config.OnValueChanged -= UpdateConfigValues;
        }
    }
}