using UnityEngine;
using TMPro;

public class SheepPenPromptUI : MonoBehaviour
{
    [Header("References")]
    public SheepPenController penController;
    public TextMeshProUGUI promptText;

    private void Start()
    {
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (penController == null || promptText == null) return;

        if (penController.PlayerInRange)
        {
            promptText.gameObject.SetActive(true);

            promptText.text = penController.SheepOutside
                ? "Press E to call sheep back"
                : "Press E to release sheep";
        }
        else
        {
            promptText.gameObject.SetActive(false);
        }
    }
}
