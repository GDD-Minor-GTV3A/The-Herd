using Core.Shared.Utilities;
using Gameplay.Effects;
using Gameplay.HealthSystem;
using Gameplay.Inventory;
using Gameplay.Map;
using Gameplay.ToolsSystem;
using UI;
using UI.Effects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Gameplay.Player 
{
    /// <summary>
    /// Base player script.
    /// </summary>
    [RequireComponent(typeof(PlayerMovement), typeof(PlayerStateManager))]
    [RequireComponent(typeof(PlayerInput), typeof(CharacterController))]
    public class Player : MonoBehaviour, IDamageable, IHealable, IKillable
    {
        [Header("Animations")]
        [SerializeField, Tooltip("Animator of the player.")]
        private Animator animator;

        [SerializeField, Tooltip("Animation constraints from AnimationRigging of the player.")]
        private PlayerAnimationConstraints animationConstrains;

        [Space, Header("Effects")]
        [SerializeField, Required, Tooltip("Reference to damage effect component.")]
        private DamageEffect dmgEffect;

        [SerializeField, Required, Tooltip("Reference to player vignette effect component.")]
        private PlayerVignetteEffect vignetteEffect;

        [Space, Header("UI")]
        [SerializeField, Required, Tooltip("Reference for HP bar component.")] 
        private HPBarUI hpBar;
        [SerializeField, Required, Tooltip("Reference for InventoryToggle component.")] 
        private InventoryToggle inventoryButton;
        [SerializeField, Tooltip("Reference for MapToggle component.")] 
        private MapToggle mapToggle;

        [Space]
        [SerializeField, Tooltip("Manager of step sounds.")]
        private StepsSoundManager stepsSoundManager;

        [SerializeField, Tooltip("Reference to input actions map.")]
        private InputActionAsset inputActions;

        [SerializeField, Tooltip("Reference to player config.")]
        private PlayerConfig config;


        [field: Space, Header("Events")]
        [field: SerializeField, Tooltip("Invokes when player gets damage.")]
        public UnityEvent DamageEvent { get; set; }

        [field: SerializeField, Tooltip("Invokes when player gets healed.")]
        public UnityEvent HealEvent { get; set; }

        [field: SerializeField, Tooltip("Invokes when player dies.")]
        public UnityEvent DeathEvent { get; set; }


        private PlayerMovement movementController;
        private Health health;


        /// <summary>
        /// Initialization method.
        /// </summary>
        public void Initialize()
        {
            Vector3 _forward = Camera.main.transform.forward;

            _forward.y = 0f;

            _forward.Normalize();

            transform.forward = _forward;


            movementController = GetComponent<PlayerMovement>();
            PlayerStateManager _stateManager = GetComponent<PlayerStateManager>();
            PlayerInput _playerInput = GetComponent<PlayerInput>();
            ToolSlotsController _slotsController = GetComponent<ToolSlotsController>();

            _playerInput.Initialize(inputActions);


            CharacterController _characterController = GetComponent<CharacterController>();
            movementController.Initialize(_characterController, config);


            stepsSoundManager.Initialize();
            PlayerAnimator _playerAnimator = new PlayerAnimator(animator, this, transform, animationConstrains);
            _stateManager.Initialize(_playerInput, movementController, _playerAnimator);

            inventoryButton.Initialize(_playerInput.Inventory);
            if (mapToggle != null)
                mapToggle.Initialize(_playerInput);

            // Init health
            health = new Health(config);

            config.OnValueChanged += UpdateConfigValues;

            if (_slotsController != null)
                _slotsController.Initialize(_playerInput, _playerAnimator, 2);
            
            dmgEffect.Initialize();
            vignetteEffect.Initialize();
            hpBar.Initialize(health);
        }


        private void UpdateConfigValues(PlayerConfig config)
        {
            movementController.UpdateValues(config);
            health.UpdateValuesFromConfig(config);
        }


        public void TakeDamage(float damage)
        {
            if (!health.CanTakeDamage && damage <= 0) return;
            health.ChangeCurrentHealth(-damage);
            DamageEvent?.Invoke();

            if (health.CurrentHealth == 0)
                Die();
        }

        public void Heal(float amount)
        {
            if (!health.CanBeHealed && amount <= 0) return;
            health.ChangeCurrentHealth(amount);
            HealEvent?.Invoke();
        }

        public void Die()
        {
            DeathEvent?.Invoke();
            health.ResetHealth();
        }


        private void OnDestroy()
        {
            config.OnValueChanged -= UpdateConfigValues;
        }
    }
}