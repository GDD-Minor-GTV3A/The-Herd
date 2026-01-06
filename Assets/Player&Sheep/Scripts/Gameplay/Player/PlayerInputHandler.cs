using System.Collections.Generic;
using Core.Events;
using Core.Shared;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.Player
{
    /// <summary>
    /// Handles all input from the player.
    /// </summary>
    public class PlayerInputHandler : MonoBehaviour, IPausable
    {
        [SerializeField, Tooltip("Layers of the ground for ray casting.")]
        LayerMask groundLayers;


        private InputActionAsset inputActions;

        private InputActionMap currentMap;
        private InputActionMap secondaryMap;
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
        #endregion InputActionProps


        public static PlayerInputHandler Instance { get; private set; }

        /// <summary>
        /// Initialization method.
        /// </summary>
        /// <param name="inputActions">Asset with input actions.</param>
        public void Initialize(InputActionAsset inputActions)
        {
            if (Instance != null)
                Destroy(Instance);

            Instance = this;

            this.inputActions = inputActions;
            mainCamera = Camera.main;
            currentMap = this.inputActions.FindActionMap("Player");
            secondaryMap = this.inputActions.FindActionMap("UI");


            // Get Input Actions
            moveAction = currentMap.FindAction("Move");

            lookAction = currentMap.FindAction("Look");

            runAction = currentMap.FindAction("Sprint");

            reloadAction = currentMap.FindAction("Reload");

            mainUsageAction = currentMap.FindAction("MainUsage");

            secondaryUsageAction = currentMap.FindAction("SecondaryUsage");

            EventManager.Broadcast(new RegisterNewPausableEvent(this));

            currentMap.Enable();

            // Enable input actions
            Resume();
        }


        private void LateUpdate()
        {
            if (isPaused) return;
            if (!lookAction.enabled) return;
            Ray ray = mainCamera.ScreenPointToRay(lookAction.ReadValue<Vector2>());

            Vector3 worldCursorPosition;

            Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, groundLayers);
            worldCursorPosition = hitInfo.point;

            cursorWorldPosition.Value = worldCursorPosition;
        }


        public void Pause()
        {
            currentMap.Disable();


            isPaused = true;
        }

        public void Resume()
        {
            currentMap.Enable();

            isPaused = false;
        }


        public static void SwitchControlMap()
        {
            if (Instance == null)
            {
                Debug.LogWarning("PlayerInputHandler: Instance was not initialized!!!");
                return;
            }
            Instance.SwitchControlMapImpl();
        }

        private void SwitchControlMapImpl()
        {
            (currentMap, secondaryMap) = (secondaryMap, currentMap);

            currentMap.Enable();
            secondaryMap.Disable();
        }

        
        public static void OnlyPlayerActionEnable(string enabledAction)
        {
            if (Instance == null)
            {
                Debug.LogWarning("PlayerInputHandler: Instance was not initialized!!!");
                return;
            }
            Instance.OnlyPlayerActionEnableImpl(enabledAction);
        }

        private void OnlyPlayerActionEnableImpl(string enabledAction)
        {
            if (currentMap.name != "Player")
            {
                Debug.LogWarning("PlayerInputHandler: Wrong input map is active, cant invoke PlayerOnlyActionEnable() !!!");
                return;
            }

            currentMap.Disable();
            currentMap.FindAction(enabledAction).Enable();
        }


        public static void EnablePlayerAction(string enabledAction)
        {
            if (Instance == null)
            {
                Debug.LogWarning("PlayerInputHandler: Instance was not initialized!!!");
                return;
            }
            Instance.EnablePlayerActionImpl(enabledAction);
        }

        private void EnablePlayerActionImpl(string enabledAction)
        {
            if (currentMap.name != "Player")
            {
                Debug.LogWarning("PlayerInputHandler: Wrong input map is active, cant invoke EnablePlayerAction() !!!");
                return;
            }

            currentMap.FindAction(enabledAction).Enable();
        }


        public static void DisablePlayerAction(string disabledAction)
        {
            if (Instance == null)
            {
                Debug.LogWarning("PlayerInputHandler: Instance was not initialized!!!");
                return;
            }
            Instance.DisablePlayerActionImpl(disabledAction);
        }

        private void DisablePlayerActionImpl(string disabledAction)
        {
            if (currentMap.name != "Player")
            {
                Debug.LogWarning("PlayerInputHandler: Wrong input map is active, cant invoke DisablePlayerAction() !!!");
                return;
            }

            currentMap.FindAction(disabledAction).Disable();
        }


        public static void EnableAllPlayerActions()
        {
            if (Instance == null)
            {
                Debug.LogWarning("PlayerInputHandler: Instance was not initialized!!!");
                return;
            }
            Instance.EnableAllPlayerActionsImpl();
        }

        private void EnableAllPlayerActionsImpl()
        {
            if (currentMap.name != "Player")
            {
                Debug.LogWarning("PlayerInputHandler: Wrong input map is active, cant invoke EnableAllPlayerActions() !!!");
                return;
            }

            currentMap.Enable();
        }


        public static void DisableAllPlayerActions()
        {
            if (Instance == null)
            {
                Debug.LogWarning("PlayerInputHandler: Instance was not initialized!!!");
                return;
            }
            Instance.DisableAllPlayerActionsImpl();
        }

        private void DisableAllPlayerActionsImpl()
        {
            if (currentMap.name != "Player")
            {
                Debug.LogWarning("PlayerInputHandler: Wrong input map is active, cant invoke DisableAllPlayerActions() !!!");
                return;
            }

            currentMap.Disable();
        }
    }
}