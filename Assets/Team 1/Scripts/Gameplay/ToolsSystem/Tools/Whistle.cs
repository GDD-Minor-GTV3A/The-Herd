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
    public class Whistle : MonoBehaviour, IPlayerTool
    {
        private Observable<Vector3> _cursorWorldPosition;
        private PlayerAnimator _animator;
        // Add a reference to a DogBark instance
        [SerializeField] private DogBark _dogBark;



        public void Initialize(PlayerAnimator animator)
        {
            _animator = animator;
            
        }


        public void HideTool()
        {
            _animator.RemoveHands();
        }


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
            Debug.Log("Bark!!!");
        }

        public void SecondaryUsageFinished()
        {
        }

        public void SecondaryUsageStarted(Observable<Vector3> cursorWorldPosition)
        {
            EventManager.Broadcast(new DogFollowCommandEvent());
        }

        public void ShowTool()
        {
        }

        private void SendDogMoveCommand()
        {
            EventManager.Broadcast(new DogMoveCommandEvent(_cursorWorldPosition.Value));
        }

        public void TryBark()
        {
            Debug.Log(_dogBark);
            if (_dogBark != null)
            {
                _dogBark.TryBark();
            }

        }
    }
}
