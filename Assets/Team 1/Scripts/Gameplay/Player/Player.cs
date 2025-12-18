using System;

using Core.AI.Sheep.Event;
using Core.Events;
using Core.Shared.Utilities;
using Gameplay.HealthSystem;
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
    [RequireComponent(typeof(PlayerInputHandler), typeof(CharacterController))]
    public class Player : MonoBehaviour, IKillable
    {
        [Header("Animations")]
        [SerializeField, Tooltip("Animator of the player.")]
        private Animator animator;

        [SerializeField, Tooltip("Animation constraints from AnimationRigging of the player.")]
        private PlayerAnimationConstraints animationConstrains;

        [SerializeField, Tooltip("Animation constraints from AnimationRigging of the player.")]
        private Whistle whistle;

        [Space, Header("Effects")]
        [SerializeField, Required, Tooltip("Reference to player vignette effect component.")]
        private PlayerVignetteEffect vignetteEffect;

        [Space, Header("UI")]
        [SerializeField, Required, Tooltip("Reference for HP bar component.")] 
        private HPBarUI hpBar;

        [Space]
        [SerializeField, Tooltip("Manager of step sounds.")]
        private StepsSoundManager stepsSoundManager;

        [SerializeField, Tooltip("Reference to input actions map.")]
        private InputActionAsset inputActions;

        [SerializeField, Tooltip("Reference to player config.")]
        private PlayerConfig config;


        [field: SerializeField, Tooltip("Invokes when player dies.")]
        public UnityEvent DeathEvent { get; set; }


        private PlayerMovement movementController;


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
            PlayerInputHandler _playerInput = GetComponent<PlayerInputHandler>();

            _playerInput.Initialize(inputActions);

            CharacterController _characterController = GetComponent<CharacterController>();
            movementController.Initialize(_characterController, config);

            stepsSoundManager.Initialize();
            PlayerAnimator _playerAnimator = new PlayerAnimator(animator, transform, animationConstrains);
            _stateManager.Initialize(_playerInput, movementController, _playerAnimator);

            whistle.Initialize(_playerAnimator, _playerInput);

            config.OnValueChanged += UpdateConfigValues;

            vignetteEffect.Initialize();

            EventManager.AddListener<SanityChangeEvent>(OnSanityChanged);
        }

        private void OnSanityChanged(SanityChangeEvent evt)
        {
            if (evt.Percentage <= 0)
            {
                Die();
            }
        }

        private void UpdateConfigValues(PlayerConfig config)
        {
            movementController.UpdateValues(config);
        }


        public void TakeDamage(float damage)
        {
        }


        public void Die()
        {
            Debug.Log("Player died.");
            DeathEvent?.Invoke();
        }


        private void OnDestroy()
        {
            config.OnValueChanged -= UpdateConfigValues;
        }
    }
}