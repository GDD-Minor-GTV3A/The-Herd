using Gameplay.ToolsSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.Player 
{
    /// <summary>
    /// Base player script.
    /// </summary>
    public class Player : MonoBehaviour
    {
        [Tooltip("Reference to player config.")]
        [SerializeField] private PlayerConfig _config;
        [Tooltip("Animator of the player.")]
        [SerializeField] private Animator _animator;
        [Tooltip("Reference to input actions map.")]
        [SerializeField] private InputActionAsset _inputActions;


        // for test, needs to be moved to bootstrap
        private void Start()
        {
            Initialize();
        }

        /// <summary>
        /// Initialization method.
        /// </summary>
        public void Initialize
            ()
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

            PlayerAnimator animator = new PlayerAnimator(_animator);

            stateManager.Initialize(playerInput, movementController,animator, rotationController);
        }
    }
}