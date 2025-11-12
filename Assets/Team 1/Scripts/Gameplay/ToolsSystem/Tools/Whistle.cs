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


        private Observable<Vector3> _cursorWorldPosition;
        private PlayerAnimator _animator;


        public void Initialize(PlayerAnimator animator)
        {
            HideUI();
            _animator = animator;

            CooldownUI cooldown = toolUI.GetComponentInChildren<CooldownUI>(true);
            if (cooldown != null) cooldown.Initialize(dogConfig.BarkCooldown);
        }


        public override void HideTool()
        {
            base.HideTool();
            _animator.RemoveHands();
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
        }

        private void SendDogMoveCommand()
        {
            EventManager.Broadcast(new DogMoveCommandEvent(_cursorWorldPosition.Value));
        }

        private void TryBark()
        {
            EventManager.Broadcast(new DogBarkEvent());
        }
    }
}