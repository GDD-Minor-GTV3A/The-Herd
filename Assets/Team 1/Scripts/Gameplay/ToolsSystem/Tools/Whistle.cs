using Core.Events;
using Core.Shared;
using Core.Shared.Utilities;
using Gameplay.Dog;
using Gameplay.Effects;
using Gameplay.Player;
using UI;
using UnityEngine;

namespace Gameplay.ToolsSystem.Tools.Whistle
{
    /// <summary>
    /// Tool for controlling dogs.
    /// </summary>
    public class Whistle : PlayerTool
    {
        [SerializeField, Tooltip("Dog config for initializing bark cooldown."), Required] 
        private DogConfig dogConfig;

        [SerializeField, Tooltip("Prefab of VFX for dog move command."), Required] 
        private DogCommandMarker markerPrefab;


        private Observable<Vector3> cursorWorldPosition;
        private PlayerAnimator playerAnimator;
        private DogCommandMarker markerObject;

        /// <summary>
        /// Initialization method.
        /// </summary>
        /// <param name="animator">Player animator.</param>
        public void Initialize(PlayerAnimator animator)
        {
            HideUI();
            playerAnimator = animator;

            CooldownUI _cooldown = toolUI.GetComponentInChildren<CooldownUI>(true);
            if (_cooldown != null) _cooldown.Initialize(dogConfig.BarkCooldown);

            markerObject = Instantiate(markerPrefab);
            markerObject.Initialize();
        }


        public override void HideTool()
        {
            base.HideTool();
            playerAnimator.RemoveHands();
        }


        public override void MainUsageFinished()
        {
        }
        public override void MainUsageStarted(Observable<Vector3> cursorWorldPosition)
        {
            OnMainUse?.Invoke();
            TryBark();
        }

        public override void Reload()
        {
            OnReload?.Invoke();
            EventManager.Broadcast(new DogFollowCommandEvent());
        }

        public override void SecondaryUsageFinished()
        {
            cursorWorldPosition.OnValueChanged -= SendDogMoveCommand;
            cursorWorldPosition = null;
        }
        public override void SecondaryUsageStarted(Observable<Vector3> cursorWorldPosition)
        {
            OnSecondaryUse?.Invoke();
            this.cursorWorldPosition = cursorWorldPosition;
            SendDogMoveCommand();
            this.cursorWorldPosition.OnValueChanged += SendDogMoveCommand;
        }


        public override void ShowTool()
        {
            base.ShowTool();
            playerAnimator.GetTool(keyPoints);
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