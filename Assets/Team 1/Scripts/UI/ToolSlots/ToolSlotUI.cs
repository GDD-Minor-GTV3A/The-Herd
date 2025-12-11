using Core.Shared.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ToolSlots 
{
    /// <summary>
    /// Handles logic of slot tool UI.
    /// </summary>
    public class ToolSlotUI : MonoBehaviour
    {
        [SerializeField, Tooltip("Image component for the slot."), Required] 
        private Image slotImage;

        [SerializeField, Tooltip("Text component for the slot."), Required] 
        private TMP_Text slotIndexText;


       // private readonly float highlightedWidth = 240f;
       // private readonly float commonWidth = 185f;

       // private readonly float commonAlpha = 0.5f;

       // private CanvasGroup canvasGroup;
      //  private RectTransform rectTransform;


        /// <summary>
        /// Initialization method.
        /// </summary>
        /// <param name="slotIndex">Index of this slot.</param>
        public void Initialize(int slotIndex)
        {
           // rectTransform = GetComponent<RectTransform>();

           // if (!TryGetComponent(out canvasGroup))
           // {
           //     canvasGroup = gameObject.AddComponent<CanvasGroup>();
           // }

            slotIndexText.text = (slotIndex + 1).ToString();

           SetVisible(false);
        }


        /// <summary>
        /// Changes highlighted state of the slot.
        /// </summary>
        /// <param name="highlighted">Highlighted or not.</param>
       // public void SetSlotHighlight(bool highlighted)
       // {
           // if (highlighted)
          //  {
          //      rectTransform.sizeDelta = new Vector2(highlightedWidth, rectTransform.sizeDelta.y);
          //      canvasGroup.alpha = 1;
          //  }
          //  else
          //  {
          //      rectTransform.sizeDelta = new Vector2(commonWidth, rectTransform.sizeDelta.y);
          //     canvasGroup.alpha = commonAlpha;
          //  }
      // }

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible); 
        }
    }
}