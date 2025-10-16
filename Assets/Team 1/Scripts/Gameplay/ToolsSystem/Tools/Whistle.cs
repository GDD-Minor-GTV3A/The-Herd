using Core.Events;
using Core.Shared;
using UnityEngine;

namespace Gameplay.ToolsSystem
{
    /// <summary>
    /// Tool for dogs controlls.
    /// </summary>
    public class Whistle : MonoBehaviour, IPlayerTool
    {
        private Observable<Vector3> _cursorWorldPosition;


        public void MainUsageFinished()
        {
            _cursorWorldPosition.OnValueChanged -= SendDogMoveCommand;
            _cursorWorldPosition = null;
        }

        public void MainUsageStarted(Observable<Vector3> cursorWorldPosition)
        {
            _cursorWorldPosition = cursorWorldPosition;
            SendDogMoveCommand();
            _cursorWorldPosition.OnValueChanged += SendDogMoveCommand;
        }

        public void Reload()
        {
            return;
        }

        public void SecondaryUsageFinished()
        {
            return;
        }

        public void SecondaryUsageStarted(Observable<Vector3> cursorWorldPosition)
        {
            EventManager.Broadcast(new DogFollowCommandEvent());
        }


        private void SendDogMoveCommand()
        {
            EventManager.Broadcast(new DogMoveCommandEvent(_cursorWorldPosition.Value));
        }
    }
}