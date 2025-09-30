using System.Collections.Generic;
using Core.Shared;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.ToolsSystem
{
    /// <summary>
    /// Handles logic of quick slots scrolling.
    /// </summary>
    public class ToolSlotsController : MonoBehaviour
    {
        private List<IPlayerTool> _toolSlots = new List<IPlayerTool>();
        private int _currentToolIndex;
        private int _slotsAmount;

        private Gameplay.Player.PlayerInput _input;


        /// <summary>
        /// Initialization method.
        /// </summary>
        /// <param name="input">Player input class.</param>
        /// <param name="slotsAmount">Max amount of available slots.</param>
        public void Initialize(Gameplay.Player.PlayerInput input, int slotsAmount)
        {
            _input = input;
            _slotsAmount = slotsAmount;

            for (int i = 0; i < _slotsAmount; i++)
                _toolSlots.Add(null);

            _currentToolIndex = 0;


            _input.MainUsage.started += OnCurrentToolMainUseStarted;
            _input.MainUsage.canceled += OnCurrentToolMainUseFinished;

            _input.SecondaryUsage.started += OnCurrentToolSecondaryUseStarted;
            _input.SecondaryUsage.canceled += OnCurrentToolSecondaryUseFinished;

            _input.SlotsScroll.started += UpdateCurrentSlot;

            _input.Slot_1.started += (obj) => SetCurrentSlotByIndex(0);
            _input.Slot_2.started += (obj) => SetCurrentSlotByIndex(1);
            _input.Slot_3.started += (obj) => SetCurrentSlotByIndex(2);


            // test
            Whistle whistle = GetComponent<Whistle>();
            SetNewToolToSlotByIndex(whistle, 0);
        }


        private void UpdateCurrentSlot(InputAction.CallbackContext obj)
        {
            int inputValue = Mathf.RoundToInt(obj.action.ReadValue<Vector2>().y);

            SetCurrentSlotByIndex(_currentToolIndex + inputValue);
        }

        private void SetCurrentSlotByIndex(int index)
        {
            index = Mathf.Clamp(index, 0, _slotsAmount);
            _currentToolIndex = index;
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
        public void SetNewToolToSlotByIndex(IPlayerTool toolToSet, int index)
        {
            _toolSlots[index] = toolToSet;
        }


        private void OnDestroy()
        {
            _input.MainUsage.started -= OnCurrentToolMainUseStarted;
            _input.MainUsage.canceled -= OnCurrentToolMainUseFinished;

            _input.SecondaryUsage.started -= OnCurrentToolSecondaryUseStarted;
            _input.SecondaryUsage.canceled -= OnCurrentToolSecondaryUseFinished;

            _input.SlotsScroll.started -= UpdateCurrentSlot;

            _input.Slot_1.started -= (obj) => SetCurrentSlotByIndex(0);
            _input.Slot_2.started -= (obj) => SetCurrentSlotByIndex(1);
            _input.Slot_3.started -= (obj) => SetCurrentSlotByIndex(2);
        }
    }
}