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
            PlayerMovement movementController = GetComponent<PlayerMovement>();
            PlayerRotation rotationController = GetComponent<PlayerRotation>();
            PlayerStateManager stateManager = GetComponent<PlayerStateManager>();
            PlayerInput playerInput = GetComponent<PlayerInput>();
            ToolSlotsController slotsController = GetComponent<ToolSlotsController>();

            playerInput.Initialize(_inputActions);

            slotsController.Initialize(playerInput, 3);

            CharacterController characterController = GetComponent<CharacterController>();
            movementController.Initialize(characterController, _config);
            rotationController.Initialize(_config.RotationSpeed);

            _stepsSoundManager.Initialize();
            PlayerAnimator animator = new PlayerAnimator(_animator);

            stateManager.Initialize(playerInput, movementController,animator, rotationController);
        }
    }
}