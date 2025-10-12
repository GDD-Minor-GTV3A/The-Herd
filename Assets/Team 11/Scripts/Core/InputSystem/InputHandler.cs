using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Core.InputSystem
{
    [CreateAssetMenu(fileName = "InputHandler", menuName = "Input System/Input Handler")]
    public class InputHandler : ScriptableObject, GameInputs.IGameplayActions, GameInputs.IUIActions
    {
        private GameInputs gameInputs;

        #region Gameplay Events
        public UnityAction<Vector2> OnMovementEvent;
        public UnityAction<Vector2> OnLookEvent;
        public UnityAction OnInteractEvent;
        public UnityAction<bool> OnSprintEvent;
        public UnityAction OnReloadEvent;
        public UnityAction OnMainUsageStartedEvent;
        public UnityAction OnMainUsageCanceledEvent;
        public UnityAction OnSecondaryUsageStartedEvent;
        public UnityAction OnSecondaryUsageCanceledEvent;
        public UnityAction OnSlot1Event;
        public UnityAction OnSlot2Event;
        public UnityAction OnSlot3Event;
        public UnityAction<Vector2> OnSlotsScrollEvent;
        public UnityAction OnPauseEvent;
        #endregion Gameplay Events

        #region UI Events
        public UnityAction<Vector2> OnNavigateEvent;
        public UnityAction OnResumeEvent;
        #endregion UI Events

        private void OnEnable()
        {
            if (gameInputs == null)
            {
                // Initialize inputs from the generated class
                gameInputs = new GameInputs();

                // Set this class to handle the input callbacks
                gameInputs.Gameplay.SetCallbacks(this);
                gameInputs.UI.SetCallbacks(this);
            }

            EnableGameplayInput();
        }

        #region Gameplay Inputs
        public void OnMovement(InputAction.CallbackContext ctx)
        {
            OnMovementEvent?.Invoke(ctx.ReadValue<Vector2>());
        }

        public void OnLook(InputAction.CallbackContext ctx)
        {
            OnLookEvent?.Invoke(ctx.ReadValue<Vector2>());
        }

        public void OnInteract(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
                OnInteractEvent?.Invoke();
        }

        public void OnSprint(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
                OnSprintEvent?.Invoke(true);
            else if (ctx.canceled)
                OnSprintEvent?.Invoke(false);
        }

        public void OnReload(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
                OnReloadEvent?.Invoke();
        }

        public void OnMainUsage(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
                OnMainUsageStartedEvent?.Invoke();
            else if (ctx.canceled)
                OnMainUsageCanceledEvent?.Invoke();
        }

        public void OnSecondaryUsage(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
                OnSecondaryUsageStartedEvent?.Invoke();
            else if (ctx.canceled)
                OnSecondaryUsageCanceledEvent?.Invoke();
        }

        public void OnSlot1(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
                OnSlot1Event?.Invoke();
        }

        public void OnSlot2(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
                OnSlot2Event?.Invoke();
        }

        public void OnSlot3(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
                OnSlot3Event?.Invoke();
        }

        public void OnSlotsScroll(InputAction.CallbackContext ctx)
        {
            OnSlotsScrollEvent?.Invoke(ctx.ReadValue<Vector2>());
        }

        public void OnPause(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
                OnPauseEvent?.Invoke();
        }
        #endregion Gameplay Inputs

        #region UI Inputs
        public void OnNavigate(InputAction.CallbackContext ctx)
        {
            OnNavigateEvent?.Invoke(ctx.ReadValue<Vector2>());
        }

        public void OnSubmit(InputAction.CallbackContext ctx)
        {
            // Implement if needed
        }

        public void OnCancel(InputAction.CallbackContext ctx)
        {
            // Implement if needed
        }

        public void OnPoint(InputAction.CallbackContext ctx)
        {
            // Implement if needed
        }

        public void OnClick(InputAction.CallbackContext ctx)
        {
            // Implement if needed
        }

        public void OnRightClick(InputAction.CallbackContext ctx)
        {
            // Implement if needed
        }

        public void OnMiddleClick(InputAction.CallbackContext ctx)
        {
            // Implement if needed
        }

        public void OnScrollWheel(InputAction.CallbackContext ctx)
        {
            // Implement if needed
        }

        public void OnResume(InputAction.CallbackContext ctx)
        {
            OnResumeEvent?.Invoke();
        }
        #endregion UI Inputs

        /// <summary>
        /// Disables all inputs.
        /// </summary>
        public void DisableInputs()
        {
            gameInputs.Gameplay.Disable();
            gameInputs.UI.Disable();
        }
        /// <summary>
        /// Enables Gameplay input and disables UI input.
        /// </summary>
        public void EnableGameplayInput()
        {
            DisableInputs();
            gameInputs.Gameplay.Enable();
        }
        /// <summary>
        /// Enables UI input and disables Gameplay input.
        /// </summary>
        public void EnableUIInput()
        {
            DisableInputs();
            gameInputs.UI.Enable();
        }
    }
}
