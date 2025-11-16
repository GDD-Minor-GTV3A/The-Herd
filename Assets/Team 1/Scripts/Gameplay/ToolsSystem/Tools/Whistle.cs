using Core.Events;
using Core.Shared;
using Gameplay.Dog;
using Gameplay.Player;
using UnityEngine;

namespace Gameplay.ToolsSystem
{
    /// <summary>
    /// Tool for controlling dogs.
    /// </summary>
    public class Whistle : PlayerTool
    {
        [SerializeField] private DogConfig dogConfig;
        [SerializeField] private DogCommandMarker markerPrefab;


        private Observable<Vector3> _cursorWorldPosition;
        private PlayerAnimator playerAnimator;
        private DogCommandMarker markerObject;


        public void Initialize(PlayerAnimator animator)
        {
            HideUI();
            playerAnimator = animator;

            CooldownUI cooldown = toolUI.GetComponentInChildren<CooldownUI>(true);
            if (cooldown != null) cooldown.Initialize(dogConfig.BarkCooldown);

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
            _cursorWorldPosition.OnValueChanged -= SendDogMoveCommand;
            _cursorWorldPosition = null;
        }

        public override void SecondaryUsageStarted(Observable<Vector3> cursorWorldPosition)
        {
            OnSecondaryUse?.Invoke();
            _cursorWorldPosition = cursorWorldPosition;
            SendDogMoveCommand();
            _cursorWorldPosition.OnValueChanged += SendDogMoveCommand;
        }

        public override void ShowTool()
        {
            base.ShowTool();
            playerAnimator.GetTool(keyPoints);
        }

        private void SendDogMoveCommand()
        {
            markerObject.StartEffect(_cursorWorldPosition.Value);
            EventManager.Broadcast(new DogMoveCommandEvent(_cursorWorldPosition.Value));
        }

        private void TryBark()
        {
            EventManager.Broadcast(new DogBarkEvent());
        }
    }
}