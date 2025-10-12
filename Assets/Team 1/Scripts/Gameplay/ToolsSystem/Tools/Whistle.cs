using Core.Events;
using Core.Shared;
using UnityEngine;

namespace Gameplay.ToolsSystem
{
    /// <summary>
    /// Tool for controlling dogs.
    /// </summary>
    public class Whistle : MonoBehaviour, IPlayerTool
    {
        private Observable<Vector3> _cursorWorldPosition;

        // Track herd mode state
        private bool _isHerdModeActive = false;

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
            Debug.Log("Testing secondary functionality.");
        }

        public void SecondaryUsageFinished()
        {
            // Not needed
        }

        public void SecondaryUsageStarted(Observable<Vector3> cursorWorldPosition)
        {
            // Toggle herd mode
            _isHerdModeActive = !_isHerdModeActive;

            // Broadcast event with the new herd state
            EventManager.Broadcast(new DogHerdModeToggleEvent(_isHerdModeActive));
            
            Debug.Log($"Dog herd mode is now {(_isHerdModeActive ? "ON" : "OFF")}");
        }

        private void SendDogMoveCommand()
        {
            EventManager.Broadcast(new DogMoveCommandEvent(_cursorWorldPosition.Value));
        }
    }
}
