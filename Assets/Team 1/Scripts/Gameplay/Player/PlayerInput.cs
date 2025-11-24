using Core.Events;
using Core.Shared;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.Player
{
    /// <summary>
    /// Handles all input from the player.
    /// </summary>
    public class PlayerInput : MonoBehaviour, IPausable
    {
        [SerializeField, Tooltip("Layers of the ground for ray casting.")]
        LayerMask groundLayers;


        private InputActionAsset inputActions;
        private Camera mainCamera;
        private bool isPaused;

        private readonly Observable<Vector3> cursorWorldPosition = new Observable<Vector3>();


        #region InputActions
        private InputAction moveAction;
        private InputAction lookAction;
        private InputAction runAction;
        private InputAction reloadAction;
        private InputAction mainUsageAction;
        private InputAction secondaryUsageAction;
        private InputAction slotsScrollAction;
        private InputAction slot1_Action;
        private InputAction slot2_Action;
        private InputAction slot3_Action;
        #endregion InputActions


        #region InputActionProps
        /// <summary>
        /// Vector2 value of movement input(WASD).
        /// </summary>
        public Vector2 Move => moveAction.ReadValue<Vector2>();

        /// <summary>
        /// Current position of the cursor in the world.
        /// </summary>
        public Observable<Vector3> Look
        {
            get
            {
                return cursorWorldPosition;
            }
        }

        /// <summary>
        /// True when sprint button is hold.
        /// </summary>
        public bool Run => runAction.IsPressed();
        /// <summary>
        /// Input action for reload button. Use this actions: started, performed, canceled.
        /// </summary>
        public InputAction Reload => reloadAction;
        /// <summary>
        /// Input action for main usage button(LMB). Use this actions: started, performed, canceled.
        /// </summary>
        public InputAction MainUsage => mainUsageAction;
        /// <summary>
        /// Input action for secondary usage button(RMB). Use this actions: started, performed, canceled.
        /// </summary>
        public InputAction SecondaryUsage => secondaryUsageAction;
        /// <summary>
        /// Input action for scrolling. Use this actions: started, performed, canceled.
        /// </summary>
        public InputAction SlotsScroll => slotsScrollAction;
        /// <summary>
        /// Input action for slot 1 button. Use this actions: started, performed, canceled.
        /// </summary>
        public InputAction Slot_1 => slot1_Action;
        /// <summary>
        /// Input action for slot 2 button. Use this actions: started, performed, canceled.
        /// </summary>
        public InputAction Slot_2 => slot2_Action;
        /// <summary>
        /// Input action for slot 3 button. Use this actions: started, performed, canceled.
        /// </summary>
        public InputAction Slot_3 => slot3_Action;

        #endregion InputActionProps


        /// <summary>
        /// Initialization method.
        /// </summary>
        /// <param name="inputActions">Asset with input actions.</param>
        public void Initialize(InputActionAsset inputActions)
        {
            this.inputActions = inputActions;
            mainCamera = Camera.main;

            // Get Input Actions
            moveAction = this.inputActions.FindActionMap("Player").FindAction("Move");
            lookAction = this.inputActions.FindActionMap("Player").FindAction("Look");
            runAction = this.inputActions.FindActionMap("Player").FindAction("Sprint");
            reloadAction = this.inputActions.FindActionMap("Player").FindAction("Reload");
            mainUsageAction = this.inputActions.FindActionMap("Player").FindAction("MainUsage");
            secondaryUsageAction = this.inputActions.FindActionMap("Player").FindAction("SecondaryUsage");
            slotsScrollAction = this.inputActions.FindActionMap("Player").FindAction("SlotsScroll");
            slot1_Action = this.inputActions.FindActionMap("Player").FindAction("Slot_1");
            slot2_Action = this.inputActions.FindActionMap("Player").FindAction("Slot_2");
            slot3_Action = this.inputActions.FindActionMap("Player").FindAction("Slot_3");

            EventManager.Broadcast(new RegisterNewPausableEvent(this));

            // Enable input actions
            Resume();
        }


        private void LateUpdate()
        {
            if (isPaused) return;
            Ray ray = mainCamera.ScreenPointToRay(lookAction.ReadValue<Vector2>());

            Vector3 worldCursorPosition;

            Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, groundLayers);
            worldCursorPosition = hitInfo.point;

            cursorWorldPosition.Value = worldCursorPosition;
        }

        public void Pause()
        {
            moveAction.Disable();
            lookAction.Disable();
            runAction.Disable();
            reloadAction.Disable();
            mainUsageAction.Disable();
            secondaryUsageAction.Disable();
            slotsScrollAction.Disable();
            slot1_Action.Disable();
            slot2_Action.Disable();
            slot3_Action.Disable();

            isPaused = true;
        }

        public void Resume()
        {
            moveAction.Enable();
            lookAction.Enable();
            runAction.Enable();
            reloadAction.Enable();
            mainUsageAction.Enable();
            secondaryUsageAction.Enable();
            slotsScrollAction.Enable();
            slot1_Action.Enable();
            slot2_Action.Enable();
            slot3_Action.Enable();

            isPaused = false;
        }
    }
}