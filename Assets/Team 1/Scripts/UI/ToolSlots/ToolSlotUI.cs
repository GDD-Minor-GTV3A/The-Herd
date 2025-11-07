using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolSlotUI : MonoBehaviour
{
    [SerializeField] private Image slotImage;
    [SerializeField] private TMP_Text slotIndexText;


    private readonly float highlightedWidth = 240f;
    private readonly float commonWidth = 185f;

    private readonly float commonAlpha = 0.5f;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;


    public void Initilaize(int slotIndex)
    {
        rectTransform = GetComponent<RectTransform>();

        if (!TryGetComponent(out canvasGroup))
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        slotIndexText.text = (slotIndex + 1).ToString();

        SetSlotHighlight(false);
    }


    /// <summary>
    /// Changes highlighted state of the slot.
    /// </summary>
    /// <param name="highlighted">Highlighted or not.</param>
    public void SetSlotHighlight(bool highlighted)
    {
        if (highlighted)
        {
            rectTransform.sizeDelta = new Vector2(highlightedWidth, rectTransform.sizeDelta.y);
            canvasGroup.alpha = 1;
        }
        else
        {
            rectTransform.sizeDelta = new Vector2(commonWidth, rectTransform.sizeDelta.y);
            canvasGroup.alpha = commonAlpha;
        }
    }
}
