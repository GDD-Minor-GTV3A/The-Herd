using Core.Events;
using Core.Shared;
using Core.Shared.Utilities;
using Gameplay.Dog;
using Gameplay.Effects;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Player
{
    /// <summary>
    /// Tool for controlling dogs.
    /// </summary>
    public class Whistle : MonoBehaviour
    {
        [SerializeField, Tooltip("Dog config for initializing bark cooldown."), Required] 
        private DogConfig dogConfig;

        [SerializeField, Tooltip("Dog config for initializing bark cooldown."), Required] 
        private RectTransform whistleUI;


        [SerializeField, Tooltip("Prefab of VFX for dog move command."), Required] 
        private DogCommandMarker markerPrefab;


        private Observable<Vector3> cursorWorldPosition;
        private PlayerAnimator playerAnimator;
        private PlayerInput input;
        private DogCommandMarker markerObject;


        [Space]
        [Header("Events")]
        [Tooltip("Invokes when player uses main action(LMB) of the tool.")]
        public UnityEvent OnMainUse;
        [Tooltip("Invokes when player uses reload(R) of the tool.")]
        public UnityEvent OnReload;
        [Tooltip("Invokes when player uses secondary action(RMB) of the tool.")]
        public UnityEvent OnSecondaryUse;


        /// <summary>
        /// Initialization method.
        /// </summary>
        /// <param name="animator">Player animator.</param>
        public void Initialize(PlayerAnimator animator, PlayerInput input)
        {
            playerAnimator = animator;

            this.input = input;

            CooldownUI _cooldown = whistleUI.GetComponentInChildren<CooldownUI>(true);
            if (_cooldown != null) _cooldown.Initialize(dogConfig.BarkCooldown);

            markerObject = Instantiate(markerPrefab);
            markerObject.Initialize();

            this.input.MainUsage.started += MainUsageStarted;
            this.input.MainUsage.canceled += MainUsageFinished;

            this.input.Reload.started += Reload;

            this.input.SecondaryUsage.started += SecondaryUsageStarted;
            this.input.SecondaryUsage.canceled += SecondaryUsageFinished;
        }


        public void MainUsageFinished(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
        }


        public void MainUsageStarted(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            OnMainUse?.Invoke();
            TryBark();
        }

        public void Reload(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            OnReload?.Invoke();
            EventManager.Broadcast(new DogFollowCommandEvent());
        }

        public void SecondaryUsageFinished(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            cursorWorldPosition.OnValueChanged -= SendDogMoveCommand;
            cursorWorldPosition = null;
        }
        public void SecondaryUsageStarted(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            this.cursorWorldPosition = input.Look;
            OnSecondaryUse?.Invoke();
            SendDogMoveCommand();
            this.cursorWorldPosition.OnValueChanged += SendDogMoveCommand;
        }


        private void SendDogMoveCommand()
        {
            markerObject.StartEffect(cursorWorldPosition.Value);
            EventManager.Broadcast(new DogMoveCommandEvent(cursorWorldPosition.Value));
        }

        private void TryBark()
        {
            EventManager.Broadcast(new DogBarkEvent());
        }
    }
}