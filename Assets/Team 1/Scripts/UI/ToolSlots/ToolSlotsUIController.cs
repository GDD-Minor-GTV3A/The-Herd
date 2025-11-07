using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ToolSlotsUIController : MonoBehaviour
{
    private List<ToolSlotUI> slots;

    private ToolSlotUI currentHighlightedSlot;


    public void Initilaize()
    {
        slots = GetComponentsInChildren<ToolSlotUI>().ToList();

        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].Initilaize(i);
        }
    }


    public void ChangeHighlightedSlot(int slotIndex)
    {
        if (currentHighlightedSlot != null)
            currentHighlightedSlot.SetSlotHighlight(false);

        currentHighlightedSlot = slots[slotIndex];
        currentHighlightedSlot.SetSlotHighlight(true);
    }
}