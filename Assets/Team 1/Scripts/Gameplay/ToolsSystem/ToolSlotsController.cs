using System.Collections.Generic;
using Core.Shared;
using Core.Shared.Utilities;

using Gameplay.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.ToolsSystem
{
    /// <summary>
    /// Handles logic of quick slots scrolling.
    /// </summary>
    public class ToolSlotsController : MonoBehaviour
    {
        [SerializeField] private Whistle whistle;
        [SerializeField] private Rifle rifle;
        [SerializeField] private ToolSlotsUIController toolSlotsUI;

        [Space]
        [Header("Audio")]
        [SerializeField, Required] private AudioSource sfxSource;


        private List<PlayerTool> _toolSlots = new List<PlayerTool>();
        private int _currentToolIndex;
        private int _slotsAmount;

        private Player.PlayerInput _input;

        private bool _initializedFirstSlot;
        



        /// <summary>
        /// Initialization method.
        /// </summary>
        /// <param name="input">Player input class.</param>
        /// <param name="slotsAmount">Max amount of available slots.</param>
        public void Initialize(Player.PlayerInput input, PlayerAnimator animator, int slotsAmount)
        {
            _input = input;
            _slotsAmount = slotsAmount;

            for (int i = 0; i < _slotsAmount; i++)
                _toolSlots.Add(null);

            _currentToolIndex = -1;


            _input.MainUsage.started += OnCurrentToolMainUseStarted;
            _input.MainUsage.canceled += OnCurrentToolMainUseFinished;

            _input.Reload.started += OnCurrentToolReload;

            _input.SecondaryUsage.started += OnCurrentToolSecondaryUseStarted;
            _input.SecondaryUsage.canceled += OnCurrentToolSecondaryUseFinished;

            _input.SlotsScroll.started += UpdateCurrentSlot;

            _input.Slot_1.started += (obj) => SetCurrentSlotByIndex(0);
            _input.Slot_2.started += (obj) => SetCurrentSlotByIndex(1);
            _input.Slot_3.started += (obj) => SetCurrentSlotByIndex(2);

            toolSlotsUI.Initilaize();

            // test
            whistle.Initialize(animator);
            SetNewToolToSlotByIndex(whistle, 0);

            rifle.Initialize(animator);
            SetNewToolToSlotByIndex(rifle, 1);

            SetCurrentSlotByIndex(0);
            _initializedFirstSlot = true;
        }
        private void UpdateCurrentSlot(InputAction.CallbackContext obj)
        {
            int inputValue = -Mathf.RoundToInt(obj.action.ReadValue<Vector2>().y);

            SetCurrentSlotByIndex(_currentToolIndex + inputValue);
        }

        private void SetCurrentSlotByIndex(int index)
        {
            if (index == _currentToolIndex) 
                return;

            if (_currentToolIndex >= 0 && _toolSlots[_currentToolIndex] != null)
                _toolSlots[_currentToolIndex].HideTool();

            index = Mathf.Clamp(index, 0, _slotsAmount-1);
            _currentToolIndex = index;

            var newTool = _toolSlots[_currentToolIndex];
            if (newTool != null) newTool.ShowTool();

            if (_initializedFirstSlot && sfxSource != null && newTool.EquipSound != null)
            {
                if (sfxSource.isPlaying)
                    sfxSource.Stop();

                sfxSource.PlayOneShot(newTool.EquipSound);
            }

            toolSlotsUI.ChangeHighlightedSlot(_currentToolIndex);
        
        }

        


        private void OnCurrentToolReload(InputAction.CallbackContext obj)
        {
            if (_toolSlots[_currentToolIndex] != null)
                _toolSlots[_currentToolIndex].Reload();
        }

        private void OnCurrentToolMainUseStarted(InputAction.CallbackContext obj)
        {
            if (_toolSlots[_currentToolIndex] != null)
                _toolSlots[_currentToolIndex].MainUsageStarted(_input.Look);
        }

        private void OnCurrentToolMainUseFinished(InputAction.CallbackContext obj)
        {
            if (_toolSlots[_currentToolIndex] != null)
                _toolSlots[_currentToolIndex].MainUsageFinished();
        }


        private void OnCurrentToolSecondaryUseStarted(InputAction.CallbackContext obj)
        {
            if (_toolSlots[_currentToolIndex] != null)
                _toolSlots[_currentToolIndex].SecondaryUsageStarted(_input.Look);
        }

        private void OnCurrentToolSecondaryUseFinished(InputAction.CallbackContext obj)
        {
            if (_toolSlots[_currentToolIndex] != null)
                _toolSlots[_currentToolIndex].SecondaryUsageFinished();
        }


        /// <summary>
        /// Sets new tool to specific quick slot.
        /// </summary>
        /// <param name="toolToSet">Tool to add.</param>
        /// <param name="index">Index of slot to add.</param>
        public void SetNewToolToSlotByIndex(PlayerTool toolToSet, int index)
        {
            _toolSlots[index] = toolToSet;
        }


        private void OnDestroy()
        {
            _input.MainUsage.started -= OnCurrentToolMainUseStarted;
            _input.MainUsage.canceled -= OnCurrentToolMainUseFinished;

            _input.Reload.canceled -= OnCurrentToolReload;

            _input.SecondaryUsage.started -= OnCurrentToolSecondaryUseStarted;
            _input.SecondaryUsage.canceled -= OnCurrentToolSecondaryUseFinished;

            _input.SlotsScroll.started -= UpdateCurrentSlot;

            _input.Slot_1.started -= (obj) => SetCurrentSlotByIndex(0);
            _input.Slot_2.started -= (obj) => SetCurrentSlotByIndex(1);
            _input.Slot_3.started -= (obj) => SetCurrentSlotByIndex(2);
        }
    }
}