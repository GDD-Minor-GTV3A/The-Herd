using Core.Events;
using Core.Shared;
using Gameplay.Player;
using UnityEngine;

namespace Gameplay.ToolsSystem
{
    /// <summary>
    /// Tool for controlling dogs.
    /// </summary>
    public class Whistle : PlayerTool
    {
        private Observable<Vector3> _cursorWorldPosition;
        private PlayerAnimator _animator;


        public void Initialize(PlayerAnimator animator)
        {
            HideUI();
            _animator = animator;
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
            TryBark();
        }

        public override void Reload()
        {
            EventManager.Broadcast(new DogFollowCommandEvent());
        }

        public override void SecondaryUsageFinished()
        {
            _cursorWorldPosition.OnValueChanged -= SendDogMoveCommand;
            _cursorWorldPosition = null;
        }

        public override void SecondaryUsageStarted(Observable<Vector3> cursorWorldPosition)
        {
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
