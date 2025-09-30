using System;

using Gameplay.ToolsSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.Player 
{
    /// <summary>
    /// Base player script.
    /// </summary>
    [RequireComponent(typeof(PlayerMovement), typeof(PlayerRotation), typeof(PlayerStateManager))]
    [RequireComponent(typeof(PlayerInput), typeof(ToolSlotsController), typeof(CharacterController))]
    public class Player : MonoBehaviour
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

            slotsController.Initialize(playerInput, 1);

            CharacterController characterController = GetComponent<CharacterController>();
            _movementController.Initialize(characterController, _config);

            _rotationController.Initialize(_config.RotationSpeed);

            _stepsSoundManager.Initialize();
            PlayerAnimator animator = new PlayerAnimator(_animator);
            stateManager.Initialize(playerInput, _movementController, animator, _rotationController);

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
    }
}