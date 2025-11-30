using System.Collections.Generic;

using Core.Events;
using Core.Shared;
using Core.Shared.Utilities;
using Gameplay.Player;
using Gameplay.ToolsSystem.Tools.Rifle;
using Gameplay.ToolsSystem.Tools.Whistle;
using UI.ToolSlots;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.ToolsSystem
{
    /// <summary>
    /// Handles logic of quick slots scrolling.
    /// </summary>
    public class ToolSlotsController : MonoBehaviour, IPausable
    {
        [SerializeField, Tooltip("Reference on whistle component. (Temp solution)"), Required] 
        private Whistle whistle;

        [SerializeField, Tooltip("Reference on rifle component. (Temp solution)"), Required] 
        private Rifle rifle;

        [SerializeField, Tooltip("Reference to UI component."), Required] 
        private ToolSlotsUIController toolSlotsUI;

        [Space, Header("Audio")]
        [SerializeField, Tooltip("Audio source for sfx clips."), Required] 
        private AudioSource sfxSource;


        private List<PlayerTool> toolSlots = new List<PlayerTool>();
        private int currentToolIndex;
        private int slotsAmount;

        private Player.PlayerInput input;

        private bool initializedFirstSlot;
        private bool isInventoryOpen = false;
        

        /// <summary>
        /// Initialization method.
        /// </summary>
        /// <param name="input">Player input class.</param>
        /// <param name="slotsAmount">Max amount of available slots.</param>
        public void Initialize(Player.PlayerInput input, PlayerAnimator animator, int slotsAmount)
        {
            this.input = input;
            this.slotsAmount = slotsAmount;

            for (int i = 0; i < this.slotsAmount; i++)
                toolSlots.Add(null);

            currentToolIndex = -1;

            toolSlotsUI.Initialize();

            // test
            whistle.Initialize(animator);
            SetNewToolToSlotByIndex(whistle, 0);

            rifle.Initialize(animator);
            SetNewToolToSlotByIndex(rifle, 1);

            SetCurrentSlotByIndex(0);
            initializedFirstSlot = true;

            EventManager.Broadcast(new RegisterNewPausableEvent(this));

            Resume();
        }
        
        
        private void UpdateCurrentSlot(InputAction.CallbackContext obj)
        {
            int _inputValue = -Mathf.RoundToInt(obj.action.ReadValue<Vector2>().y);

            SetCurrentSlotByIndex(currentToolIndex + _inputValue);
        }


        private void SetCurrentSlotByIndex(int index)
        {
            if (index == currentToolIndex) 
                return;

            if (currentToolIndex >= 0 && toolSlots[currentToolIndex] != null)
                toolSlots[currentToolIndex].HideTool();

            index = Mathf.Clamp(index, 0, slotsAmount-1);
            currentToolIndex = index;

            PlayerTool _newTool = toolSlots[currentToolIndex];
            if (_newTool != null) _newTool.ShowTool();

            if (initializedFirstSlot && sfxSource != null && _newTool.EquipSound != null)
            {
                if (sfxSource.isPlaying)
                    sfxSource.Stop();

                sfxSource.PlayOneShot(_newTool.EquipSound);
            }

            toolSlotsUI.ChangeHighlightedSlot(currentToolIndex);
        
        }


        private void OnCurrentToolReload(InputAction.CallbackContext obj)
        {
            if (toolSlots[currentToolIndex] != null)
                toolSlots[currentToolIndex].Reload();
        }

        private void OnCurrentToolMainUseStarted(InputAction.CallbackContext obj)
        {
            if (toolSlots[currentToolIndex] != null)
                toolSlots[currentToolIndex].MainUsageStarted(input.Look);
        }

        private void OnCurrentToolMainUseFinished(InputAction.CallbackContext obj)
        {
            if (toolSlots[currentToolIndex] != null)
                toolSlots[currentToolIndex].MainUsageFinished();
        }

        private void OnCurrentToolSecondaryUseStarted(InputAction.CallbackContext obj)
        {
            if (toolSlots[currentToolIndex] != null)
                toolSlots[currentToolIndex].SecondaryUsageStarted(input.Look);
        }

        private void OnCurrentToolSecondaryUseFinished(InputAction.CallbackContext obj)
        {
            if (toolSlots[currentToolIndex] != null)
                toolSlots[currentToolIndex].SecondaryUsageFinished();
        }


        /// <summary>
        /// Sets new tool to specific quick slot.
        /// </summary>
        /// <param name="toolToSet">Tool to add.</param>
        /// <param name="index">Index of slot to add.</param>
        public void SetNewToolToSlotByIndex(PlayerTool toolToSet, int index)
        {
            toolSlots[index] = toolToSet;
        }


        private void OnDestroy()
        {
            Pause();
        }


        public void Pause()
        {
            input.MainUsage.started -= OnCurrentToolMainUseStarted;
            input.MainUsage.canceled -= OnCurrentToolMainUseFinished;

            input.Reload.canceled -= OnCurrentToolReload;

            input.SecondaryUsage.started -= OnCurrentToolSecondaryUseStarted;
            input.SecondaryUsage.canceled -= OnCurrentToolSecondaryUseFinished;

            input.SlotsScroll.started -= UpdateCurrentSlot;

            input.Slot_1.started -= (obj) => SetCurrentSlotByIndex(0);
            input.Slot_2.started -= (obj) => SetCurrentSlotByIndex(1);
            input.Slot_3.started -= (obj) => SetCurrentSlotByIndex(2);

            input.Inventory.canceled -= OnInventoryButtonPressed;
        }

        public void Resume()
        {
            input.MainUsage.started += OnCurrentToolMainUseStarted;
            input.MainUsage.canceled += OnCurrentToolMainUseFinished;

            input.Reload.started += OnCurrentToolReload;

            input.SecondaryUsage.started += OnCurrentToolSecondaryUseStarted;
            input.SecondaryUsage.canceled += OnCurrentToolSecondaryUseFinished;

            input.SlotsScroll.started += UpdateCurrentSlot;

            input.Slot_1.started += (obj) => SetCurrentSlotByIndex(0);
            input.Slot_2.started += (obj) => SetCurrentSlotByIndex(1);
            input.Slot_3.started += (obj) => SetCurrentSlotByIndex(2);

            input.Inventory.canceled += OnInventoryButtonPressed;
        }

        private void OnInventoryButtonPressed(InputAction.CallbackContext obj)
        {
            isInventoryOpen = !isInventoryOpen;
        }
    }
}