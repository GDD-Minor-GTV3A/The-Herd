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
        #endregion Events

        void OnEnable()
        {
            if (inputs == null)
            {
                inputs = new GameInputs();

                inputs.Player.AddCallbacks(this);
                inputs.UI.AddCallbacks(this);
            }

            EnablePlayerInputs();
        }

        public void OnCancel(InputAction.CallbackContext context) { }

        public void OnClick(InputAction.CallbackContext context) { }

        public void OnDogBark(InputAction.CallbackContext context) { }

        public void OnInteract(InputAction.CallbackContext context) { }

        public void OnLook(InputAction.CallbackContext context) { }

        public void OnMainUsage(InputAction.CallbackContext context) { }

        public void OnMiddleClick(InputAction.CallbackContext context) { }

        public void OnMove(InputAction.CallbackContext context) { }

        public void OnNavigate(InputAction.CallbackContext context) { }

        public void OnPoint(InputAction.CallbackContext context) { }

        public void OnReload(InputAction.CallbackContext context) { }

        public void OnRightClick(InputAction.CallbackContext context) { }

        public void OnScrollWheel(InputAction.CallbackContext context) { }

        public void OnSecondaryUsage(InputAction.CallbackContext context) { }

        public void OnSlotsScroll(InputAction.CallbackContext context) { }

        public void OnSlot_1(InputAction.CallbackContext context) { }

        public void OnSlot_2(InputAction.CallbackContext context) { }

        public void OnSlot_3(InputAction.CallbackContext context) { }

        public void OnSprint(InputAction.CallbackContext context) { }

        public void OnSubmit(InputAction.CallbackContext context) { }

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
