using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Core.InputSystem
{
    [CreateAssetMenu(fileName = "Inputs", menuName = "InputHandler/Inputs", order = 0)]
    public class InputHandler : ScriptableObject, GameInputs.IPlayerActions, GameInputs.IUIActions
    {
        private GameInputs inputs;

        #region Events
        public UnityAction<Vector2> MovementEvent;
        public UnityAction<Vector2> LookEvent;
        public UnityAction MainUsageEvent;
        public UnityAction MainUsageCanceledEvent;
        public UnityAction SecondaryUsageEvent;
        public UnityAction SecondaryUsageCanceledEvent;
        public UnityAction InteractEvent;
        public UnityAction<bool> SprintEvent;
        public UnityAction ReloadEvent;
        public UnityAction<int> SlotsEvent;
        public UnityAction<Vector2> SlotsScrollEvent;
        public UnityAction BarkEvent;

        public UnityAction<Vector2> NavigateEvent;
        #endregion Events

        void OnEnable()
        {
            if (inputs == null)
            {
                inputs = new GameInputs();

                inputs.Player.SetCallbacks(this);
                inputs.UI.SetCallbacks(this);

                EnablePlayerInputs();
            }
        }

        void OnDisable()
        {
            DisableAllInputs();
        }

        #region Player Inputs
        public void OnMove(InputAction.CallbackContext context)
        {
            MovementEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnMainUsage(InputAction.CallbackContext context)
        {
            if (context.performed)
                MainUsageEvent?.Invoke();
            else if (context.canceled)
                MainUsageCanceledEvent?.Invoke();
        }

        public void OnSecondaryUsage(InputAction.CallbackContext context)
        {
            if (context.performed)
                SecondaryUsageEvent?.Invoke();
            else if (context.canceled)
                SecondaryUsageCanceledEvent?.Invoke();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed)
                InteractEvent?.Invoke();
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            SprintEvent?.Invoke(context.ReadValueAsButton());
        }

        public void OnReload(InputAction.CallbackContext context)
        {
            if (context.performed)
                ReloadEvent?.Invoke();
        }

        public void OnSlot_1(InputAction.CallbackContext context)
        {
            if (context.performed)
                SlotsEvent?.Invoke(0);
        }

        public void OnSlot_2(InputAction.CallbackContext context)
        {
            if (context.performed)
                SlotsEvent?.Invoke(1);
        }

        public void OnSlot_3(InputAction.CallbackContext context)
        {
            if (context.performed)
                SlotsEvent?.Invoke(2);
        }

        public void OnSlotsScroll(InputAction.CallbackContext context)
        {
            SlotsScrollEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnDogBark(InputAction.CallbackContext context)
        {
            if (context.performed)
                BarkEvent?.Invoke();
        }
        #endregion Player Inputs

        #region UI Inputs
        public void OnNavigate(InputAction.CallbackContext context)
        {
            NavigateEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnSubmit(InputAction.CallbackContext context) { }

        public void OnCancel(InputAction.CallbackContext context) { }

        public void OnPoint(InputAction.CallbackContext context) { }

        public void OnClick(InputAction.CallbackContext context) { }

        public void OnRightClick(InputAction.CallbackContext context) { }

        public void OnMiddleClick(InputAction.CallbackContext context) { }

        public void OnScrollWheel(InputAction.CallbackContext context) { }
        #endregion UI Inputs

        #region Utilities
        public void DisableAllInputs()
        {
            inputs.Player.Disable();
            inputs.UI.Disable();
        }

        public void EnablePlayerInputs()
        {
            DisableAllInputs();
            inputs.Player.Enable();
        }

        public void EnableUIInputs()
        {
            DisableAllInputs();
            inputs.UI.Enable();
        }
        #endregion Utilities
    }
}
