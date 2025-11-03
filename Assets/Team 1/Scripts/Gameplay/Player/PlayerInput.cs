using Core.Shared;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.Player
{
    /// <summary>
    /// Handles all input from the player.
    /// </summary>
    public class PlayerInput : MonoBehaviour
    {
        [Tooltip("Layers of the ground for ray casting.")]
        [SerializeField] LayerMask _groundLayers;


        private InputActionAsset _inputActions;
        private Camera _mainCamera;

        private readonly Observable<Vector3> _cursorWorldPosition = new Observable<Vector3>();


        #region InputActions
        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _runAction;
        private InputAction _reloadAction;
        private InputAction _mainUsageAction;
        private InputAction _SecondaryUsageAction;
        private InputAction _slotsScrollAction;
        private InputAction _slot1_Action;
        private InputAction _slot2_Action;
        private InputAction _slot3_Action;
        #endregion InputActions


        #region InputActionProps
        /// <summary>
        /// Vector2 value of movement input(WASD).
        /// </summary>
        public Vector2 Move => _moveAction.ReadValue<Vector2>();

        /// <summary>
        /// Current position of the cursor in the world.
        /// </summary>
        public Observable<Vector3> Look
        {
            get
            {
                UpdateCursorPositionInWorld();
                return _cursorWorldPosition;
            }
        }

        /// <summary>
        /// True when sprint button is hold.
        /// </summary>
        public bool Run => _runAction.IsPressed();
        /// <summary>
        /// Input action for reload button. Use this actions: started, performed, canceled.
        /// </summary>
        public InputAction Reload => _reloadAction;
        /// <summary>
        /// Input action for main usage button(LMB). Use this actions: started, performed, canceled.
        /// </summary>
        public InputAction MainUsage => _mainUsageAction;
        /// <summary>
        /// Input action for secondary usage button(RMB). Use this actions: started, performed, canceled.
        /// </summary>
        public InputAction SecondaryUsage => _SecondaryUsageAction;
        /// <summary>
        /// Input action for scrolling. Use this actions: started, performed, canceled.
        /// </summary>
        public InputAction SlotsScroll => _slotsScrollAction;
        /// <summary>
        /// Input action for slot 1 button. Use this actions: started, performed, canceled.
        /// </summary>
        public InputAction Slot_1 => _slot1_Action;
        /// <summary>
        /// Input action for slot 2 button. Use this actions: started, performed, canceled.
        /// </summary>
        public InputAction Slot_2 => _slot2_Action;
        /// <summary>
        /// Input action for slot 3 button. Use this actions: started, performed, canceled.
        /// </summary>
        public InputAction Slot_3 => _slot3_Action;
        #endregion InputActionProps


        /// <summary>
        /// Initialization method.
        /// </summary>
        /// <param name="inputActions">Asset with input actions.</param>
        public void Initialize(InputActionAsset inputActions)
        {
            _inputActions = inputActions;
            _mainCamera = Camera.main;

            // Get Input Actions
            _moveAction = _inputActions.FindActionMap("Player").FindAction("Move");
            _lookAction = _inputActions.FindActionMap("Player").FindAction("Look");
            _runAction = _inputActions.FindActionMap("Player").FindAction("Sprint");
            _reloadAction = _inputActions.FindActionMap("Player").FindAction("Reload");
            _mainUsageAction = _inputActions.FindActionMap("Player").FindAction("MainUsage");
            _SecondaryUsageAction = _inputActions.FindActionMap("Player").FindAction("SecondaryUsage");
            _slotsScrollAction = _inputActions.FindActionMap("Player").FindAction("SlotsScroll");
            _slot1_Action = _inputActions.FindActionMap("Player").FindAction("Slot_1");
            _slot2_Action = _inputActions.FindActionMap("Player").FindAction("Slot_2");
            _slot3_Action = _inputActions.FindActionMap("Player").FindAction("Slot_3");

            // Enable input actions
            _moveAction.Enable();
            _lookAction.Enable();
            _runAction.Enable();
            _reloadAction.Enable();
            _mainUsageAction.Enable();
            _SecondaryUsageAction.Enable();
            _slotsScrollAction.Enable();
            _slot1_Action.Enable();
            _slot2_Action.Enable();
            _slot3_Action.Enable();
        }


        private void UpdateCursorPositionInWorld()
        {
            Ray ray = _mainCamera.ScreenPointToRay(_lookAction.ReadValue<Vector2>());

            Vector3 worldCursorPosition;

            Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, _groundLayers);
            worldCursorPosition = hitInfo.point;
            

            _cursorWorldPosition.Value = worldCursorPosition;
        }
    }
}