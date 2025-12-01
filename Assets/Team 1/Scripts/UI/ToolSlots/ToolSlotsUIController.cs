using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI.ToolSlots
{
    /// <summary>
    /// Controls all tool slots.
    /// </summary>
    public class ToolSlotsUIController : MonoBehaviour
    {
        private List<ToolSlotUI> slots;
        private ToolSlotUI currentHighlightedSlot;


        /// <summary>
        /// Initialization method.
        /// </summary>
        public void Initialize()
        {
            slots = GetComponentsInChildren<ToolSlotUI>(true).ToList();

            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].Initialize(i);
            }

            
           
        }


        /// <summary>
        /// Change which slot is highlighted by index.
        /// </summary>
        /// <param name="slotIndex">Index of slot to highlight.</param>
        public void ChangeHighlightedSlot(int slotIndex)
        {
            if (currentHighlightedSlot != null)
                currentHighlightedSlot.SetVisible(false);

            currentHighlightedSlot = slots[slotIndex];
            currentHighlightedSlot.SetVisible(true);
        }
    }
}