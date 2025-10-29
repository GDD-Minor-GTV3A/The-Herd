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
    [RequireComponent(typeof(PlayerMovement), typeof(PlayerStateManager))]
    [RequireComponent(typeof(PlayerInput), typeof(ToolSlotsController), typeof(CharacterController))]
    public class Player : MonoBehaviour, IDamageable, IHealable, IKillable
    {
        [Header("Animations")]
        [Tooltip("Animator of the player.")]
        [SerializeField] private Animator animator;
        [SerializeField] private PlayerAnimationConstraints animationConstrains;

        [Space]
        [Tooltip("Manager of step sounds.")]
        [SerializeField] private StepsSoundManager stepsSoundManager;
        [Tooltip("Reference to input actions map.")]
        [SerializeField] private InputActionAsset inputActions;
        [Tooltip("Reference to input actions map.")]
        [SerializeField] private CanvasGroup vignette;
        [Tooltip("Reference to player config.")]
        [SerializeField] private PlayerConfig config;


        private PlayerMovement _movementController;
        private PlayerAnimator playerAnimator;
        private Health _health;
        private Coroutine vigneteRoutine;


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
            Vector3 forward = Camera.main.transform.forward;

            forward.y = 0f;

            forward.Normalize();

            transform.forward = forward;

            _movementController = GetComponent<PlayerMovement>();
            PlayerStateManager stateManager = GetComponent<PlayerStateManager>();
            PlayerInput playerInput = GetComponent<PlayerInput>();
            ToolSlotsController slotsController = GetComponent<ToolSlotsController>();

            playerInput.Initialize(inputActions);


            CharacterController characterController = GetComponent<CharacterController>();
            _movementController.Initialize(characterController, config);


            stepsSoundManager.Initialize();
            playerAnimator = new PlayerAnimator(animator, transform, animationConstrains, vignette);
            stateManager.Initialize(playerInput, _movementController, playerAnimator);

            // Init health
            _health = new Health(
                config.MaxHealth,
                config.CurrentHealth,
                config.CanTakeDamage,
                config.CanBeHealed,
                config.CanDie
            );
            config.OnValueChanged += UpdateConfigValues;

            slotsController.Initialize(playerInput, playerAnimator, 2);
        }


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
            config.OnValueChanged -= UpdateConfigValues;
        }


        public void TakeDamage(float damage)
        {
            if (!_health.CanTakeDamage && damage <= 0) return;
            _health.ChangeCurrentHealth(-damage);
            if (vigneteRoutine != null)
                StopCoroutine(vigneteRoutine);
            vigneteRoutine = StartCoroutine(playerAnimator.ShowVignetteRoutine(.7f));
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