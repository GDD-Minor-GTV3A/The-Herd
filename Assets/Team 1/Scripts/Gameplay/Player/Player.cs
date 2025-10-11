using Gameplay.HealthSystem;
using Gameplay.ToolsSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Gameplay.Player 
{
    /// <summary>
    /// Base player script.
    /// </summary>
    [RequireComponent(typeof(PlayerMovement), typeof(PlayerRotation), typeof(PlayerStateManager))]
    [RequireComponent(typeof(PlayerInput), typeof(ToolSlotsController), typeof(CharacterController))]
    public class Player : MonoBehaviour, IDamageable, IHealable, IKillable
    {
        [Tooltip("Animator of the player.")]
        [SerializeField] private Animator _animator;
        [Tooltip("Manager of step sounds.")]
        [SerializeField] private StepsSoundManager _stepsSoundManager;
        [Tooltip("Reference to input actions map.")]
        [SerializeField] private InputActionAsset _inputActions;
        [Tooltip("Reference to player config.")]
        [SerializeField] private PlayerConfig _config;


        private PlayerMovement _movementController;
        private PlayerRotation _rotationController;
        private Health _health;


        public UnityEvent OnDamageTaken;
        public UnityEvent OnHealed;
        public UnityEvent OnDied;


        public UnityEvent DamageEvent { get => OnDamageTaken; set => OnDamageTaken = value; }
        public UnityEvent HealEvent { get => OnHealed; set => OnHealed = value; }
        public UnityEvent DeathEvent { get => OnDied; set => OnDied = value; }


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
            _movementController = GetComponent<PlayerMovement>();
            _rotationController = GetComponent<PlayerRotation>();
            PlayerStateManager stateManager = GetComponent<PlayerStateManager>();
            PlayerInput playerInput = GetComponent<PlayerInput>();
            ToolSlotsController slotsController = GetComponent<ToolSlotsController>();

            playerInput.Initialize(_inputActions);

            slotsController.Initialize(playerInput, 2);

            CharacterController characterController = GetComponent<CharacterController>();
            _movementController.Initialize(characterController, _config);

            _rotationController.Initialize(_config.RotationSpeed);

            _stepsSoundManager.Initialize();
            PlayerAnimator animator = new PlayerAnimator(_animator);
            stateManager.Initialize(playerInput, _movementController, animator, _rotationController);

            // Init health
            _health = new Health(
                _config.MaxHealth,
                _config.CurrentHealth,
                _config.CanTakeDamage,
                _config.CanBeHealed,
                _config.CanDie
            );
            _config.OnValueChanged += UpdateConfigValues;

        }


        /// <summary>
        /// Update values according to config.
        /// </summary>
        /// <param name="config">Config of the player.</param>
        private void UpdateConfigValues(PlayerConfig config)
        {
            _movementController.UpdateValues(config);
            _rotationController.UpdateRotationSpeed(config.RotationSpeed);
        }


        private void OnDestroy()
        {
            _config.OnValueChanged -= UpdateConfigValues;
        }


        public void TakeDamage(float damage)
        {
            if (!_health.CanTakeDamage && damage <= 0) return;
            _health.ChangeCurrentHealth(-damage);
            OnDamageTaken?.Invoke();

            if (_health.CurrentHealth == 0)
                Die();
        }

        public void Heal(float amount)
        {
            if (!_health.CanBeHealed && amount <= 0) return;
            _health.ChangeCurrentHealth(amount);
            OnHealed?.Invoke();
        }

        public void Die()
        {
            OnDied?.Invoke();
            Debug.Log("Died!");
        }
    }
}